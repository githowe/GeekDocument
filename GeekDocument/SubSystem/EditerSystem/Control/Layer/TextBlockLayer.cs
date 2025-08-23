using GeekDocument.SubSystem.EditerSystem.Control.LayerTool;
using GeekDocument.SubSystem.EditerSystem.Define;
using GeekDocument.SubSystem.LayoutSystem;
using GeekDocument.SubSystem.OptionSystem;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using XLogic.Base.Ex;

namespace GeekDocument.SubSystem.EditerSystem.Control.Layer
{
    public class TextBlockLayer : BlockLayer
    {
        #region 属性

        /// <summary>文本块实例</summary>
        public BlockText Block { get; set; }

        public override int BlockHeight => Block.GetViewHeight();

        public override int CharIndex => _charIndex;

        public override int CharIndexMax => Block.Content.Length;

        #endregion

        #region object 方法

        public override string ToString() => Block.Content;

        #endregion

        #region 生命周期

        public override void Init()
        {
            byte red = byte.Parse(Block.Color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte green = byte.Parse(Block.Color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte blue = byte.Parse(Block.Color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            _textColor = Color.FromRgb(red, green, blue);
            _rowLinePen.Freeze();
            _stateTree.Init(this);
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 追加文本
        /// </summary>
        public void AppendText(string text)
        {
            // 拼接文本
            Block.Content = Block.Content + text;
            // 更新视图数据与视图
            Block.UpdateViewData(BlockWidth);
            Update();
        }

        #endregion

        #region BlockLayer 方法

        public override void MoveIBeamToHead()
        {
            _charIndex = 0;
            SyncIBeam();
        }

        public override void MoveIBeamToEnd()
        {
            _charIndex = Block.Content.Length;
            SyncIBeam();
        }

        public override void MoveIBeamToFirstLine(double mouse_x)
        {
            _currentLine = null;
            if (Block.ViewData.Count > 0) _currentLine = Block.ViewData[0];
            double y = GetLineY(_currentLine);
            double x = MoveIBeamToLine(_currentLine, mouse_x);
            Page.移动光标(x.RoundInt(), (int)y, Block.FontSize);
        }

        public override void MoveIBeamToLastLine(double mouse_x)
        {
            _currentLine = null;
            if (Block.ViewData.Count > 0) _currentLine = Block.ViewData.Last();
            double y = GetLineY(_currentLine);
            double x = MoveIBeamToLine(_currentLine, mouse_x);
            Page.移动光标(x.RoundInt(), (int)y, Block.FontSize);
        }

        public override void MoveIBeamToIndex(int index)
        {
            _charIndex = index;
            SyncIBeam();
        }

        public override void HandleEditKey(EditKey key)
        {
            _stateTree.HandleEditKey(key);
        }

        public override void InputText(string text)
        {
            // 插入文本
            Block.Content = Block.Content.Insert(_charIndex, text);
            _charIndex += text.Length;
            // 更新视图数据与视图
            Block.UpdateViewData(BlockWidth);
            Update();
            // 同步光标
            SyncIBeam();
        }

        public override void MoveIBeamToPoint(Point point)
        {
            // 文本块由行构成，所以第一步是判断坐标处于哪一行
            UpdateCurrentLine(point.Y);
            // 第二步是获取当前行的纵坐标，以确定光标的纵坐标
            double y = GetLineY(_currentLine);
            // 第三步是判断鼠标横坐标点在哪个字符上，以确定光标的横坐标
            double x = MoveIBeamToLine(_currentLine, point.X);
            // 第四步是移动光标
            Page.移动光标(x.RoundInt(), (int)y, Block.FontSize);
            // 使用鼠标移动光标后，需要更新光标横坐标
            Page.更新光标横坐标();
        }

        /// <summary>
        /// 同步光标
        /// </summary>
        public override void SyncIBeam()
        {
            _currentLine = null;
            // 获取图层在画布中的纵坐标
            int top = (int)Canvas.GetTop(this);
            // 没有文本时：将光标定位在第一行开头
            if (Block.Content.Length == 0)
            {
                double start_x = Canvas.GetLeft(this);
                Page.移动光标(start_x.RoundInt() + Block.FirstLineIndent, top, Block.FontSize);
                return;
            }

            TextLine? 字符索引所在行 = null;
            List<int> 字符索引列表 = new List<int>();
            int 行索引 = 0;

            if (_charIndex == Block.Content.Length)
            {
                字符索引所在行 = Block.ViewData.Last();
                字符索引列表 = GetCharIndexList(字符索引所在行);
                字符索引列表.Add(_charIndex);
                行索引 = Block.ViewData.Count - 1;
            }
            else
            {
                // 遍历文本行
                foreach (var textLine in Block.ViewData)
                {
                    (int, int) range = textLine.GetCharIndexRange();
                    if (range.Item1 <= _charIndex && _charIndex <= range.Item2)
                    {
                        字符索引列表 = GetCharIndexList(textLine);
                        字符索引所在行 = textLine;
                        break;
                    }
                    行索引++;
                }
            }
            // 如果没有找到，返回
            if (字符索引所在行 == null) throw new Exception("未找到字符索引所在行");

            _currentLine = 字符索引所在行;
            // 获取文本行每个字符横坐标
            List<double> xList = GetXList(字符索引所在行);
            // 获取字符索引在行中的索引
            int indexInLine = 字符索引列表.IndexOf(_charIndex);
            // 获取字符横坐标
            double x = xList[indexInLine];
            // 计算当前行的纵坐标
            double y = Canvas.GetTop(this) + 行索引 * (Block.FontSize + Block.LineSpace);
            // 移动光标
            Page.移动光标(x.RoundInt(), (int)y, Block.FontSize);
        }

        public override List<Rect> GetSelectionRectList(int startCharIndex, int endCharIndex)
        {
            List<Rect> result = new List<Rect>();

            if (Block.Content.Length == 0)
            {
                int x1 = (int)(Canvas.GetLeft(this) + Block.FirstLineIndent);
                int x2 = x1;
                int y1 = (int)Canvas.GetTop(this);
                int y2 = y1 + Block.FontSize;
                result.Add(new Rect(new Point(x1, y1), new Point(x2, y2)));
                return result;
            }

            if (endCharIndex == 30)
            {
                int i = 10;
            }

            // 获取字符索引所在行
            int startLineIndex = GetLineIndex(startCharIndex);
            int endLineIndex = GetLineIndex(endCharIndex);
            // 获取失败
            if (startLineIndex == -1 || endLineIndex == -1) throw new Exception("获取字符索引所在行失败");
            // 一行
            if (endLineIndex == startLineIndex)
            {
                double x1 = GetCharIndexXOnLine(Block.ViewData[startLineIndex], startCharIndex);
                double x2 = GetCharIndexXOnLine(Block.ViewData[startLineIndex], endCharIndex);
                double y1 = GetLineY(Block.ViewData[startLineIndex]);
                double y2 = y1 + Block.FontSize;
                result.Add(new Rect(new Point(x1.Round(), y1), new Point(x2.Round(), y2)));
                return result;
            }
            // 两行
            if (endLineIndex - 1 == startLineIndex)
            {
                // 第一行
                double x1 = GetCharIndexXOnLine(Block.ViewData[startLineIndex], startCharIndex);
                double x2 = GetLineRight(Block.ViewData[startLineIndex]);
                double y1 = GetLineY(Block.ViewData[startLineIndex]);
                double y2 = y1 + Block.FontSize;
                result.Add(new Rect(new Point(x1.Round(), y1), new Point(x2.Round(), y2)));
                // 第二行
                x1 = GetLineLeft(Block.ViewData[endLineIndex]);
                x2 = GetCharIndexXOnLine(Block.ViewData[endLineIndex], endCharIndex);
                y1 = GetLineY(Block.ViewData[endLineIndex]);
                y2 = y1 + Block.FontSize;
                result.Add(new Rect(new Point(x1.Round(), y1), new Point(x2.Round(), y2)));
                return result;
            }
            // 多行
            for (int lineIndex = startLineIndex; lineIndex <= endLineIndex; lineIndex++)
            {
                double x1, x2;
                double y1 = GetLineY(Block.ViewData[lineIndex]);
                double y2 = y1 + Block.FontSize;
                // 首行
                if (lineIndex == startLineIndex)
                {
                    x1 = GetCharIndexXOnLine(Block.ViewData[lineIndex], startCharIndex);
                    x2 = GetLineRight(Block.ViewData[lineIndex]);
                    result.Add(new Rect(new Point(x1.Round(), y1), new Point(x2.Round(), y2)));
                }
                // 中间行
                else if (lineIndex > startLineIndex && lineIndex < endLineIndex)
                {
                    x1 = GetLineLeft(Block.ViewData[lineIndex]);
                    x2 = GetLineRight(Block.ViewData[lineIndex]);
                    result.Add(new Rect(new Point(x1.Round(), y1), new Point(x2.Round(), y2)));
                }
                // 尾行
                else
                {
                    x1 = GetLineLeft(Block.ViewData[lineIndex]);
                    x2 = GetCharIndexXOnLine(Block.ViewData[lineIndex], endCharIndex);
                    result.Add(new Rect(new Point(x1.Round(), y1), new Point(x2.Round(), y2)));
                }
            }

            return result;
        }

        #endregion

        #region 状态树接口

        public bool IsEmpty => Block.Content == "";

        public int TextLength => Block.Content.Length;

        public bool HasPrevLine
        {
            get
            {
                if (_currentLine == null) return false;
                int index = Block.ViewData.IndexOf(_currentLine);
                return index > 0;
            }
        }

        public bool HasNextLine
        {
            get
            {
                if (_currentLine == null) return false;
                int index = Block.ViewData.IndexOf(_currentLine);
                return index < Block.ViewData.Count - 1;
            }
        }

        public bool 光标在行首
        {
            get
            {
                if (_currentLine == null) return true;
                (int, int) range = _currentLine.GetCharIndexRange();
                return _charIndex == range.Item1;
            }
        }

        public bool 光标在行尾
        {
            get
            {
                if (_currentLine == null) return true;
                (int, int) range = _currentLine.GetCharIndexRange();
                return _charIndex == range.Item2 + 1;
            }
        }

        public void 上移光标()
        {
            // 将当前行更新为上一行
            int index = Block.ViewData.IndexOf(_currentLine);
            _currentLine = Block.ViewData[index - 1];
            // 获取命中行的纵坐标
            double y = GetLineY(_currentLine);
            // 模拟鼠标点击以确定光标横坐标
            double x = MoveIBeamToLine(_currentLine, Page.获取光标横坐标());
            // 移动光标
            Page.移动光标(x.RoundInt(), (int)y, Block.FontSize);
        }

        public void 下移光标()
        {
            // 将当前行更新为下一行
            int index = Block.ViewData.IndexOf(_currentLine);
            _currentLine = Block.ViewData[index + 1];
            double y = GetLineY(_currentLine);
            double x = MoveIBeamToLine(_currentLine, Page.获取光标横坐标());
            Page.移动光标(x.RoundInt(), (int)y, Block.FontSize);
        }

        public void 左移光标()
        {
            _charIndex--;
            SyncIBeam();
            Page.更新光标横坐标();
        }

        public void 右移光标()
        {
            _charIndex++;
            SyncIBeam();
            Page.更新光标横坐标();
        }

        public void 移动光标至行首()
        {
            // 获取当前行的横坐标列表
            List<double> xList = GetXList(_currentLine);
            // 获取当前行的字符索引列表
            List<int> charIndexList = GetCharIndexList(_currentLine);
            // 添加末尾索引，以便将光标定位至末尾
            int lastCharIndex = charIndexList.Last();
            charIndexList.Add(lastCharIndex + 1);

            double y = GetLineY(_currentLine);
            double x = xList.First();
            Page.移动光标(x.RoundInt(), (int)y, Block.FontSize);
            Page.更新光标横坐标();
            _charIndex = charIndexList.First();
        }

        public void 移动光标至行尾()
        {
            // 获取当前行的横坐标列表
            List<double> xList = GetXList(_currentLine);
            // 获取当前行的字符索引列表
            List<int> charIndexList = GetCharIndexList(_currentLine);
            // 添加末尾索引，以便将光标定位至末尾
            int lastCharIndex = charIndexList.Last();
            charIndexList.Add(lastCharIndex + 1);

            double y = GetLineY(_currentLine);
            double x = xList.Last();
            Page.移动光标(x.RoundInt(), (int)y, Block.FontSize);
            Page.更新光标横坐标();
            _charIndex = charIndexList.Last();
        }

        public void 用退格键删除空块()
        {
            // 获取上一个块
            BlockLayer? prevBlock = Page.获取上一个块(this);
            if (prevBlock == null) throw new Exception("获取上一个块失败");
            // 移除当前块
            Page.移除块(this);
            // 将上一个块设为当前块
            Page.设置当前块(prevBlock);
            // 移动光标至上一个块末尾
            prevBlock.MoveIBeamToEnd();
            Page.更新光标横坐标();
        }

        public void 删除字符()
        {
            // 删除字符
            Block.Content = Block.Content.Remove(_charIndex - 1, 1);
            // 前移字符索引
            _charIndex--;
            // 更新视图数据与视图
            Block.UpdateViewData(BlockWidth);
            Update();
            // 同步光标
            SyncIBeam();
            Page.更新光标横坐标();
        }

        public override bool 能否合并()
        {
            // 获取上一个块
            BlockLayer? prevBlock = Page.获取上一个块(this);
            if (prevBlock == null) return false;
            // 与上一个块类型不同
            if (Block.Type != prevBlock.SourceBlock.Type) return false;

            return true;
        }

        public override void 合并块()
        {
            // 获取上一个块
            TextBlockLayer? prevBlock = Page.获取上一个块(this) as TextBlockLayer;
            if (prevBlock == null) return;
            // 记录上一个块的长度
            int prevLength = prevBlock.Block.Content.Length;
            // 将当前块的内容添加至上一个块
            prevBlock.AppendText(Block.Content);
            // 移除当前块实例
            Page.移除块(this);
            // 将上一个块设为当前块
            Page.设置当前块(prevBlock);
            // 移动光标
            prevBlock.MoveIBeamToIndex(prevLength);
            Page.更新光标横坐标();
        }

        public void 创建空文本块()
        {
            // 获取自身索引
            int blockIndex = Page.获取块索引(this);
            // 创建文本块并继承当前块的属性
            BlockText block = new BlockText
            {
                FontFamily = Block.FontFamily,
                FontSize = Block.FontSize,
                Color = Block.Color,
                TStyle = Block.TStyle,
                Align = Block.Align,
                LineSpace = Block.LineSpace,
                FirstLineIndent = Block.FirstLineIndent
            };
            Page.插入块(block, blockIndex + 1);
        }

        public void 创建文本块()
        {
            // 获取并删除从当前光标至末尾的文本
            string tailText = Block.Content.Substring(_charIndex);
            Block.Content = Block.Content.Substring(0, _charIndex);
            // 更新视图数据与视图
            Block.UpdateViewData(BlockWidth);
            Update();

            // 获取自身索引
            int blockIndex = Page.获取块索引(this);
            // 创建文本块并继承当前块的属性
            BlockText block = new BlockText
            {
                Content = tailText,
                FontFamily = Block.FontFamily,
                FontSize = Block.FontSize,
                Color = Block.Color,
                TStyle = Block.TStyle,
                Align = Block.Align,
                LineSpace = Block.LineSpace,
                FirstLineIndent = Block.FirstLineIndent
            };
            Page.插入块(block, blockIndex + 1);
        }

        #endregion

        #region 内部方法

        protected override void OnUpdate()
        {
            int y = 0;
            // 没有行时，绘制空行线
            if (Block.ViewData.Count == 0 && Options.Instance.View.ShowRowLine)
            {
                double y1 = y + 0.5;
                double y2 = y + Block.FontSize - 0.5;
                _dc.DrawLine(_rowLinePen, new Point(0, y1), new Point(BlockWidth, y1));
                _dc.DrawLine(_rowLinePen, new Point(0, y2), new Point(BlockWidth, y2));
                return;
            }
            // 遍历行
            foreach (var line in Block.ViewData)
            {
                // 显示行线
                if (Options.Instance.View.ShowRowLine)
                {
                    double y1 = y + 0.5;
                    double y2 = y + Block.FontSize - 0.5;
                    _dc.DrawLine(_rowLinePen, new Point(0, y1), new Point(BlockWidth, y1));
                    _dc.DrawLine(_rowLinePen, new Point(0, y2), new Point(BlockWidth, y2));
                }
                // 绘制文本行
                DrawTextLine(line, y);
                y += Block.FontSize + Block.LineSpace;
            }
        }

        /// <summary>
        /// 绘制文本行
        /// </summary>
        protected void DrawTextLine(TextLine line, int y)
        {
            int index = 0;
            foreach (var word in line.WordList)
            {
                // 不绘制空格
                if (word.WordType == WordType.Space)
                {
                    index++;
                    continue;
                }
                // 字横坐标
                double word_x = line.XList[index];
                // 绘制字包含的字形
                foreach (var image in word.GlyphImageList)
                {
                    Point leftTop = new Point((word_x + image.Origin.X).Round(), y + image.Origin.Y);
                    _dc.DrawImage(image.GetBitmap(_textColor.R, _textColor.G, _textColor.B), new Rect(leftTop, new Size(image.RenderWidth, image.RenderHeight)));
                    word_x += image.GlyphWidth;
                }
                // 移动至下一个字
                index++;
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 更新当前行
        /// </summary>
        private void UpdateCurrentLine(double y)
        {
            // 没有文本时，置空当前行
            if (Block.Content.Length == 0)
            {
                _currentLine = null;
                return;
            }
            // 行起始纵坐标 = 块纵坐标 - 行间距的一半
            double start_y = Canvas.GetTop(this) - Block.LineSpace / 2;
            // 行区域高度 = 行高（字体大小） + 行间距
            double lineRectHeight = Block.FontSize + Block.LineSpace;
            // 计算全部行的纵坐标列表
            int lineCount = Block.ViewData.Count;
            List<double> yList = new List<double>();
            for (int index = 0; index < lineCount; index++)
                yList.Add(start_y + index * lineRectHeight);
            yList.Add(start_y + lineCount * lineRectHeight);
            // 计算命中区间的索引
            int hitedIndex = yList.GetHitedRange(y);
            // 更新当前行
            _currentLine = Block.ViewData[hitedIndex];
        }

        /// <summary>
        /// 获取行的纵坐标
        /// </summary>
        private double GetLineY(TextLine? line)
        {
            if (line == null) return Canvas.GetTop(this);

            int lineIndex = Block.ViewData.IndexOf(line);
            return Canvas.GetTop(this) + lineIndex * (Block.FontSize + Block.LineSpace);
        }

        /// <summary>
        /// 移动光标至行
        /// </summary>
        private double MoveIBeamToLine(TextLine? line, double x)
        {
            // 获取块横坐标
            double block_x = Canvas.GetLeft(this);
            // 行为空时，返回第一行缩进位置
            if (line == null) return block_x + Block.FirstLineIndent;

            // 获取行的每个字符的横坐标
            List<double> xList = GetXList(line);
            // 获取行的字符索引列表
            List<int> charIndexList = GetCharIndexList(line);
            // 添加末尾索引，以便将光标定位至末尾
            int lastCharIndex = charIndexList.Last();
            charIndexList.Add(lastCharIndex + 1);
            // 计算命中区间索引
            int hitedIndex = xList.GetHitedRange(x);
            // 计算命中横坐标
            double left = xList[hitedIndex];
            double right = xList[hitedIndex + 1];
            double center = left + (right - left) / 2;
            double hitedx = x < center ? left : right;
            // 过半时定位至右索引
            if (x >= center) hitedIndex++;
            // 更新字符索引
            _charIndex = charIndexList[hitedIndex];
            // 返回命中横坐标
            return hitedx;
        }

        /// <summary>
        /// 获取文本行的字符索引列表
        /// </summary>
        private List<int> GetCharIndexList(TextLine textLine)
        {
            List<int> result = new List<int>();
            foreach (var item in textLine.WordList)
                result.AddRange(item.CharIndexList);
            return result;
        }

        /// <summary>
        /// 获取文本行的字符横坐标列表
        /// </summary>
        private List<double> GetXList(TextLine textLine)
        {
            // 起始横坐标 = 图层左侧坐标
            double start_x = Canvas.GetLeft(this);
            List<double> xList = new List<double>();
            // 遍历字
            int x_index = 0;
            foreach (var item in textLine.WordList)
            {
                // 获取字的起始坐标
                double word_x = start_x + textLine.XList[x_index];
                // 添加横坐标
                if (item.MultiChar) xList.AddRange(item.GetXList(word_x));
                else xList.Add(word_x);
                x_index++;
            }
            // 添加末尾横坐标：末尾字横坐标 + 末尾字宽度
            double last_x = start_x + textLine.XList.Last() + textLine.WordList.Last().Width;
            xList.Add(last_x);

            return xList;
        }

        /// <summary>
        /// 根据字符索引获取所在行索引
        /// </summary>
        private int GetLineIndex(int charIndex)
        {
            // 当前块没有任何内容时，返回第一行
            if (Block.Content.Length == 0) return 0;

            int lineIndex = 0;
            // 遍历行
            foreach (var line in Block.ViewData)
            {
                // 获取该行的字符索引范围
                (int, int) range = line.GetCharIndexRange();
                // 末尾索引应该放到行尾，所以要向后移动一位
                range.Item2 += 1;
                // 判断字符索引是否在该范围内
                if (range.Item1 <= charIndex && charIndex <= range.Item2)
                    return lineIndex;
                lineIndex++;
            }
            return -1;
        }

        /// <summary>
        /// 获取行中的字符索引的横坐标
        /// </summary>
        private double GetCharIndexXOnLine(TextLine textLine, int charIndex)
        {
            double start_x = Canvas.GetLeft(this);
            int wordIndex = 0;
            // 遍历字
            foreach (var word in textLine.WordList)
            {
                // 获取字的起始坐标
                double word_x = start_x + textLine.XList[wordIndex];
                // 判断字符索引是否在该字中
                int indexInWord = word.CharIndexList.IndexOf(charIndex);
                if (indexInWord != -1)
                {
                    List<double> xList = word.GetXList(word_x);
                    return xList[indexInWord];
                }
                wordIndex++;
            }
            // 获取最后一个字的最后一个字符索引
            int lastIndex = textLine.WordList.Last().CharIndexList.Last();
            // 如果字符索引处于行尾，返回行的右端坐标
            if (lastIndex + 1 == charIndex) return GetLineRight(textLine);
            return -1;
        }

        private double GetLineLeft(TextLine textLine)
        {
            // 获取块横坐标
            double block_x = Canvas.GetLeft(this);
            if (textLine.WordList.Count == 0) return block_x + Block.FirstLineIndent;
            // 获取第一个字的横坐标
            double firstWordX = textLine.XList.First();
            return block_x + firstWordX;
        }

        private double GetLineRight(TextLine textLine)
        {
            // 获取块横坐标
            double block_x = Canvas.GetLeft(this);
            if (textLine.WordList.Count == 0) return block_x + Block.FirstLineIndent;
            // 获取最后一个字的横坐标
            double lastWordX = textLine.XList.Last() + textLine.WordList.Last().Width;
            return block_x + lastWordX;
        }

        #endregion

        #region 字段

        /// <summary>文本笔刷</summary>
        private Color _textColor = Color.FromRgb(255, 255, 255);
        /// <summary>行线画笔</summary>
        private readonly Pen _rowLinePen = new Pen(new SolidColorBrush(Color.FromArgb(32, 255, 255, 255)), 1);

        /// <summary>当前行</summary>
        private TextLine? _currentLine = null;
        /// <summary>当前字符索引</summary>
        private int _charIndex = 0;

        private readonly STTextBlock _stateTree = new STTextBlock();

        #endregion
    }
}