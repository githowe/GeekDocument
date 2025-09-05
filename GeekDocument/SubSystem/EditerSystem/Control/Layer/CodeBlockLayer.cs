using GeekDocument.SubSystem.EditerSystem.Control.LayerTool;
using GeekDocument.SubSystem.EditerSystem.Define;
using GeekDocument.SubSystem.EditerSystem.Define.BlockDerive;
using GeekDocument.SubSystem.LayoutSystem;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using XLogic.Base.Ex;

namespace GeekDocument.SubSystem.EditerSystem.Control.Layer
{
    public class CodeBlockLayer : BlockLayer
    {
        #region 属性

        public BlockCode Block { get; set; }

        public override int BlockHeight => _blockHeight;

        public override int CharIndex => _charIndex;

        public override int CharIndexMax
        {
            get
            {
                int indexMax = 0;
                foreach (var item in Block.LineList)
                    indexMax += item.Length;
                indexMax += Block.LineList.Count - 1;
                return indexMax;
            }
        }

        public override bool IsEmpty => Block.IsEmpty;

        #endregion

        #region SingleBoard 方法

        public override void Init()
        {
            _代码背景.Freeze();
            _行号背景.Freeze();
            _stateTree.Init(this);
        }

        protected override void OnUpdate()
        {
            // 代码块高度 = 代码区高度 + 上下边距
            _blockHeight = Block.GetViewHeight() + _padding * 2;
            // 行号区宽度 = 最长行号宽度 + 双倍边距
            _numberAreaWidth = Block.NumberList.Last().GetWidth().RoundInt() + _padding * 2;
            // 不显示行号
            if (!Block.ShowLineNumber) _numberAreaWidth = 0;
            // 绘制底框
            if (Block.ShowLineNumber) _dc.DrawRectangle(_行号背景, null, new Rect(0, 0, _numberAreaWidth, _blockHeight));
            _dc.DrawRectangle(_代码背景, null, new Rect(_numberAreaWidth, 0, BlockWidth - _numberAreaWidth, _blockHeight));
            // 绘制行号
            int y = _padding;
            if (Block.ShowLineNumber)
            {
                foreach (var line in Block.NumberList)
                {
                    DrawLine(line, (_numberAreaWidth - _padding - line.GetWidth()).RoundInt(), y, 128, 128, 128);
                    y += Block.FontSize + Block.LineSpace;
                }
            }
            // 绘制代码
            y = _padding;
            foreach (var line in Block.LineList)
            {
                DrawLine(line, _numberAreaWidth + _padding, y, 255, 255, 255, true);
                y += Block.FontSize + Block.LineSpace;
            }
            if (Block.ShowLanguage)
            {
                // 语言区大小
                double languageAreaWidth = (Block.LanguageLine.GetWidth() + _padding).RoundInt();
                double languageAreaHeight = Block.FontSize + _padding;
                _dc.DrawRectangle(_行号背景, null, new Rect(BlockWidth - languageAreaWidth, 0, languageAreaWidth, languageAreaHeight));
                DrawLine(Block.LanguageLine, (BlockWidth - languageAreaWidth).RoundInt() + _padding / 2, _padding / 2, 255, 221, 103);
            }
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
            _charIndex = CharIndexMax;
            SyncIBeam();
        }

        public override void MoveIBeamToFirstLine(double mouse_x)
        {
            _currentLine = Block.LineList[0];
            double y = GetLineY(_currentLine);
            double x = MoveIBeamToLine(_currentLine, mouse_x);
            Page.移动光标(x.RoundInt(), (int)y, Block.FontSize);
        }

        public override void MoveIBeamToLastLine(double mouse_x)
        {
            _currentLine = Block.LineList.Last();
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
            if (!text.Contains('\n'))
            {
                int lineIndex = Block.LineList.IndexOf(_currentLine);
                int charIndexInLine = _charIndex - GetLineStartIndex(_currentLine);
                Block.插入文本(lineIndex, charIndexInLine, text);
                Update();
                _charIndex += text.Length;
                SyncIBeam();
                Page.更新光标横坐标();
            }
            else
            {
                // 分割成行列表
                List<string> lineList = text.Split('\n').ToList();
                // 获取当前行索引和行内字符索引
                int lineIndex = Block.LineList.IndexOf(_currentLine);
                int charIndexInLine = _charIndex - GetLineStartIndex(_currentLine);
                // 获取当前行光标后文本
                string tailText = _currentLine.Text.Substring(charIndexInLine);
                // 更新当前行对应的源代码行：移除光标后文本并加上第一行文本
                Block.SourceLineList[lineIndex] = Block.SourceLineList[lineIndex].Substring(0, charIndexInLine) + lineList[0];
                // 只有两行
                if (lineList.Count == 2)
                {
                    // 插入新行：第二行文本 + 光标后文本
                    Block.SourceLineList.Insert(lineIndex + 1, lineList[1] + tailText);
                    Block.UpdateSouceCode();
                    Update();
                    // 更新当前行
                    _currentLine = Block.LineList[lineIndex + 1];
                    int lineStartIndex = GetLineStartIndex(_currentLine);
                    _charIndex = lineStartIndex + lineList[1].Length;
                    SyncIBeam();
                    Page.更新光标横坐标();
                }
                else
                {
                    // 生成中间行列表
                    List<string> middleLineList = new List<string>();
                    for (int index = 1; index < lineList.Count - 1; index++)
                        middleLineList.Add(lineList[index]);
                    // 插入中间行
                    Block.SourceLineList.InsertRange(lineIndex + 1, middleLineList);
                    // 插入最后一行：最后一行文本 + 光标后文本
                    int lastIndex = lineIndex + 1 + middleLineList.Count;
                    Block.SourceLineList.Insert(lastIndex, lineList.Last() + tailText);
                    // 更新块
                    Block.UpdateSouceCode();
                    Update();
                    // 更新当前行
                    _currentLine = Block.LineList[lastIndex];
                    int lineStartIndex = GetLineStartIndex(_currentLine);
                    _charIndex = lineStartIndex + lineList.Last().Length;
                    SyncIBeam();
                    Page.更新光标横坐标();
                }
            }
        }

        public override void MoveIBeamToPoint(Point point)
        {
            UpdateCurrentLine(point.Y);
            if (_currentLine == null) throw new Exception("当前行为空");
            double y = GetLineY(_currentLine);
            double x = MoveIBeamToLine(_currentLine, point.X);
            Page.移动光标(x.RoundInt(), (int)y, Block.FontSize);
            Page.更新光标横坐标();
        }

        public override void SyncIBeam()
        {
            _currentLine = null;
            int left = (int)Canvas.GetLeft(this);
            int top = (int)Canvas.GetTop(this);
            int codeLeft = left + _numberAreaWidth + _padding;
            int codeTop = top + _padding;

            CodeLine? 字符索引所在行 = null;
            List<int> 字符索引列表 = new List<int>();
            int 行索引 = 0;

            int 起始索引 = 0;
            int 结束索引;
            // 遍历代码行，找到字符索引所在行以及填充该行的字符索引列表
            foreach (var item in Block.LineList)
            {
                结束索引 = 起始索引 + item.Length;
                if (起始索引 <= _charIndex && _charIndex <= 结束索引)
                {
                    字符索引所在行 = item;
                    for (int index = 起始索引; index <= 结束索引; index++) 字符索引列表.Add(index);
                    break;
                }
                起始索引 = 结束索引 + 1;
                行索引++;
            }
            // 如果没有找到，抛出异常
            if (字符索引所在行 == null) throw new Exception("未找到字符索引所在行");
            _currentLine = 字符索引所在行;
            // 获取每个字符的横坐标
            List<double> xList = 字符索引所在行.GetXList(codeLeft);
            // 获取字符横坐标
            double x = xList[字符索引列表.IndexOf(_charIndex)];
            // 移动光标
            int y = codeTop + 行索引 * (Block.FontSize + Block.LineSpace);
            Page.移动光标(x.RoundInt(), y, Block.FontSize);
        }

        #endregion

        #region 状态树接口

        public int TextLength => CharIndexMax;

        public bool HasPrevLine
        {
            get
            {
                if (_currentLine == null) throw new Exception("当前行为空");
                int index = Block.LineList.IndexOf(_currentLine);
                return index > 0;
            }
        }

        public bool HasNextLine
        {
            get
            {
                if (_currentLine == null) throw new Exception("当前行为空");
                int index = Block.LineList.IndexOf(_currentLine);
                return index < Block.LineList.Count - 1;
            }
        }

        public bool 光标在行首
        {
            get
            {
                if (_currentLine == null) throw new Exception("当前行为空");
                int startIndex = GetLineStartIndex(_currentLine);
                return _charIndex == startIndex;
            }
        }

        public bool 光标在行尾
        {
            get
            {
                if (_currentLine == null) throw new Exception("当前行为空");
                int startIndex = GetLineStartIndex(_currentLine);
                return _charIndex == startIndex + _currentLine.Length;
            }
        }

        public int LineCount => Block.LineList.Count;

        public bool EmptyLine
        {
            get
            {
                if (_currentLine == null) throw new Exception("当前行为空");
                return _currentLine.Length == 0;
            }
        }

        public bool 光标前有字符
        {
            get
            {
                if (_currentLine == null) throw new Exception("当前行为空");
                int startIndex = GetLineStartIndex(_currentLine);
                return _charIndex > startIndex;
            }
        }

        public bool 光标处于块头 => _charIndex == 0;

        public void 上移光标()
        {
            int index = Block.LineList.IndexOf(_currentLine);
            _currentLine = Block.LineList[index - 1];
            double y = GetLineY(_currentLine);
            double x = MoveIBeamToLine(_currentLine, Page.获取光标横坐标());
            Page.移动光标(x.RoundInt(), (int)y, Block.FontSize);
        }

        public void 下移光标()
        {
            int index = Block.LineList.IndexOf(_currentLine);
            _currentLine = Block.LineList[index + 1];
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
            // 获取代码块左侧起始位置
            double codeLeft = Canvas.GetLeft(this) + _numberAreaWidth + _padding;
            // 获取当前行的横坐标列表
            List<double> xList = _currentLine.GetXList(codeLeft);
            // 移动至最左侧
            double x = xList[0];
            double y = GetLineY(_currentLine);
            Page.移动光标(x.RoundInt(), (int)y, Block.FontSize);
            Page.更新光标横坐标();
            // 更新字符索引
            _charIndex = GetLineStartIndex(_currentLine);
        }

        public void 移动光标至行尾()
        {
            double codeLeft = Canvas.GetLeft(this) + _numberAreaWidth + _padding;
            List<double> xList = _currentLine.GetXList(codeLeft);
            double x = xList.Last();
            double y = GetLineY(_currentLine);
            Page.移动光标(x.RoundInt(), (int)y, Block.FontSize);
            Page.更新光标横坐标();
            int lineStartIndex = GetLineStartIndex(_currentLine);
            _charIndex = lineStartIndex + _currentLine.Length;
        }

        public void 用退格键删除块()
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

        public void 替换为文本块()
        {
            // 移除当前块
            Page.移除块(this);
            // 插入文本块
            BlockText block = new BlockText { FirstLineIndent = Page.FirstLineIndent };
            Page.插入块(block, 0);
        }

        public void 删除字符()
        {
            int lineStartIndex = GetLineStartIndex(_currentLine);
            Block.RemoveChar(_currentLine, _charIndex - lineStartIndex - 1);
            Update();
            _charIndex--;
            SyncIBeam();
            Page.更新光标横坐标();
        }

        public void 删除当前行()
        {
            // 获取上一行索引
            int prevLineIndex = Block.LineList.IndexOf(_currentLine) - 1;
            // 删除当前行
            Block.RemoveLine(_currentLine);
            Update();
            // 更新当前行
            _currentLine = Block.LineList[prevLineIndex];
            移动光标至行尾();
        }

        public void 删除当前行并移动光标至前块末尾()
        {
            Block.RemoveLine(_currentLine);
            Update();
            _currentLine = null;
            移动光标至前块末尾();
        }

        public void 合并当前行至上一行()
        {
            // 获取上一行末尾字符索引
            int prevLineIndex = Block.LineList.IndexOf(_currentLine) - 1;
            CodeLine prevLine = Block.LineList[prevLineIndex];
            int prevStartIndex = GetLineStartIndex(prevLine);
            int preveEndIndex = prevStartIndex + prevLine.Length;
            // 合并至上一行
            Block.合并至上一行(_currentLine);
            Update();
            // 同步光标
            _charIndex = preveEndIndex;
            SyncIBeam();
            Page.更新光标横坐标();
        }

        public void 在块前插入文本块()
        {
            int index = Page.获取块索引(this);
            BlockText block = new BlockText { FirstLineIndent = Page.FirstLineIndent };
            Page.插入块(block, index);
            Page.设置当前块(this);
            MoveIBeamToHead();
        }

        public void 在块后插入文本块()
        {
            int index = Page.获取块索引(this);
            BlockText block = new BlockText { FirstLineIndent = Page.FirstLineIndent };
            Page.插入块(block, index + 1);
        }

        public void 创建空行()
        {
            int index = Block.LineList.IndexOf(_currentLine);
            Block.插入行(index + 1, "");
            Update();
            _currentLine = Block.LineList[index + 1];
            _charIndex = GetLineStartIndex(_currentLine);
            SyncIBeam();
            Page.更新光标横坐标();
        }

        public void 创建行()
        {
            int lineIndex = Block.LineList.IndexOf(_currentLine);
            int charIndexInLine = _charIndex - GetLineStartIndex(_currentLine);
            Block.分割行(lineIndex, charIndexInLine);
            Update();
            _currentLine = Block.LineList[lineIndex + 1];
            _charIndex = GetLineStartIndex(_currentLine);
            SyncIBeam();
            Page.更新光标横坐标();
        }

        #endregion

        #region 私有方法

        private void DrawLine(CodeLine line, int left, int top, byte r, byte g, byte b, bool isCode = false)
        {
            int index = 0;
            int charIndex = GetLineStartIndex(line);
            List<double> xList = line.GetXList(left);
            foreach (var item in line.GlyphImageList)
            {
                Color color = Block.HighlightResult.GetColor(charIndex);
                double x = xList[index];
                Point leftTop = new Point((x + item.Origin.X).Round(), top + item.Origin.Y);
                if (isCode)
                    _dc.DrawImage(item.GetBitmap(color.R, color.G, color.B), new Rect(leftTop, new Size(item.RenderWidth, item.RenderHeight)));
                else
                    _dc.DrawImage(item.GetBitmap(r, g, b), new Rect(leftTop, new Size(item.RenderWidth, item.RenderHeight)));
                index++;
                charIndex++;
            }
        }

        /// <summary>
        /// 更新当前行
        /// </summary>
        private void UpdateCurrentLine(double y)
        {
            _currentLine = null;
            // 行起始纵坐标 = 块顶端 + 上边距 - 行间距 / 2
            double start_y = Canvas.GetTop(this) + _padding - Block.LineSpace / 2;
            double lineRectHeight = Block.FontSize + Block.LineSpace;
            // 计算全部行的纵坐标列表
            int lineCount = Block.LineList.Count;
            List<double> yList = new List<double>();
            for (int index = 0; index < lineCount; index++)
                yList.Add(start_y + index * lineRectHeight);
            yList.Add(start_y + lineCount * lineRectHeight);
            // 计算命中区间的索引并更新当前行
            int hitedIndex = yList.GetHitedRange(y);
            _currentLine = Block.LineList[hitedIndex];
        }

        /// <summary>
        /// 获取行的纵坐标
        /// </summary>
        private double GetLineY(CodeLine? line)
        {
            if (line == null) return Canvas.GetTop(this) + _padding;

            int lineIndex = Block.LineList.IndexOf(line);
            return Canvas.GetTop(this) + _padding + lineIndex * (Block.FontSize + Block.LineSpace);
        }

        /// <summary>
        /// 移动光标至行
        /// </summary>
        private double MoveIBeamToLine(CodeLine line, double x)
        {
            double codeLeft = Canvas.GetLeft(this) + _numberAreaWidth + _padding;
            List<double> xList = line.GetXList(codeLeft);
            // 获取命中位置
            (int, double) posotion = xList.GetHitedPosition(x);
            // 获取起始索引
            int lineStartIndex = GetLineStartIndex(line);
            // 更新字符索引
            _charIndex = lineStartIndex + posotion.Item1;
            // 返回命中横坐标
            return posotion.Item2;
        }

        /// <summary>
        /// 获取行起始索引
        /// </summary>
        private int GetLineStartIndex(CodeLine line)
        {
            int lineIndex = Block.LineList.IndexOf(line);
            int startIndex = 0;
            for (int index = 0; index < lineIndex; index++)
                startIndex += Block.LineList[index].Length + 1;
            return startIndex;
        }

        #endregion

        #region 字段

        private readonly Brush _代码背景 = new SolidColorBrush(Color.FromArgb(255, 24, 24, 24));
        private readonly Brush _行号背景 = new SolidColorBrush(Color.FromArgb(255, 16, 16, 16));

        private int _blockHeight = 0;
        private readonly int _padding = 16;

        /// <summary>行号区宽度</summary>
        private int _numberAreaWidth = 0;

        /// <summary>当前行</summary>
        private CodeLine? _currentLine = null;
        private int _charIndex = 0;

        private readonly STCodeBlock _stateTree = new STCodeBlock();

        #endregion
    }
}