using GeekDocument.SubSystem.EditerSystem.Core;
using GeekDocument.SubSystem.EditerSystem.Define;
using GeekDocument.SubSystem.LayoutSystem;
using GeekDocument.SubSystem.OptionSystem;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using XLogic.Base.Ex;

namespace GeekDocument.SubSystem.EditerSystem.Control.Layer
{
    public class TextBlockLayer : BlockLayer
    {
        #region 属性

        /// <summary>块实例</summary>
        public BlockText Block { get; set; }

        public override int BlockHeight => Block.GetViewHeight();

        #endregion

        #region 生命周期

        public override void Init()
        {
            byte red = byte.Parse(Block.Color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte green = byte.Parse(Block.Color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte blue = byte.Parse(Block.Color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            _textColor = Color.FromRgb(red, green, blue);
            _rowLinePen.Freeze();
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

        #endregion

        #region 内部方法

        protected override void OnUpdate()
        {
            int y = 0;
            // 遍历行
            foreach (var line in Block.ViewData)
            {
                // 显示行线
                if (Options.Instance.View.ShowRowLine)
                {
                    double y1 = y + 0.5;
                    double y2 = y + Block.FontSize - 0.5;
                    _dc.DrawLine(_rowLinePen, new Point(0, y1), new Point(Options.Instance.Page.PageWidth, y1));
                    _dc.DrawLine(_rowLinePen, new Point(0, y2), new Point(Options.Instance.Page.PageWidth, y2));
                }
                // 绘制文本行
                DrawTextLine(line, y);
                y += Block.FontSize + Block.LineSpace;
            }
        }

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
        /// 同步光标
        /// </summary>
        private void SyncIBeam()
        {
            // 获取图层在画布中的纵坐标
            int top = (int)Canvas.GetTop(this);
            // 没有文本时：将光标定位在第一行开头
            if (Block.Content.Length == 0)
            {
                Editer.MoveIBeam(Block.FirstLineIndent, top, Block.FontSize);
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
            if (字符索引所在行 == null) return;

            // 获取文本行每个字符横坐标
            List<double> xList = GetXList(字符索引所在行);
            // 获取字符索引在行中的索引
            int indexInLine = 字符索引列表.IndexOf(_charIndex);
            // 获取字符横坐标
            double x = xList[indexInLine];

            // 计算当前行的纵坐标
            double y1 = 行索引 * (Block.FontSize + Block.LineSpace) + Canvas.GetTop(this);
            double y2 = y1 + Block.FontSize;
            // 移动光标
            Editer.MoveIBeam((int)x, (int)y1, (int)(y2 - y1));
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
            double start_x = Canvas.GetLeft(this) + textLine.Indent;
            double x = start_x;
            List<double> xList = new List<double>();

            foreach (var item in textLine.WordList)
            {
                if (item.MultiChar) xList.AddRange(item.GetXList(x));
                else xList.Add(x);
                x += item.Width + item.Interval;
            }
            xList.Add(x);

            return xList;
        }

        #endregion

        #region 字段

        /// <summary>文本笔刷</summary>
        private Color _textColor = Color.FromRgb(255, 255, 255);
        /// <summary>行线画笔</summary>
        private readonly Pen _rowLinePen = new Pen(new SolidColorBrush(Color.FromArgb(32, 255, 255, 255)), 1);

        private int _charIndex = 0;

        #endregion
    }
}