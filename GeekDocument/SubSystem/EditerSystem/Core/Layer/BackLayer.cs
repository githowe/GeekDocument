using System.Windows;
using System.Windows.Media;
using XLogic.Wpf.Drawing;

namespace GeekDocument.SubSystem.EditerSystem.Core.Layer
{
    /// <summary>
    /// 背景图层。绘制边框与裁剪标记
    /// </summary>
    public class BackLayer : SingleBoard
    {
        public int PageWidth { get; set; } = 0;

        public int PageHeight { get; set; } = 0;

        public int TopMargin { get; set; } = 32;

        public int BottomMargin { get; set; } = 32;

        public int LeftMargin { get; set; } = 32;

        public int RightMargin { get; set; } = 32;

        public int MarkSize { get; set; } = 24;

        public bool ShowClipMark { get; set; } = true;

        public override void Init()
        {
            _borderPen.Freeze();
            _markLine.Freeze();
        }

        protected override void OnUpdate()
        {
            // 绘制边框
            if (PageWidth > 0 && PageHeight > 0)
            {
                _dc.DrawRectangle(null, _borderPen, new Rect(0.5, 0.5, PageWidth - 1, PageHeight - 1));
            }
            // 绘制裁剪标记
            if (ShowClipMark)
            {
                int left = LeftMargin;
                int right = PageWidth - RightMargin;
                int top = TopMargin;
                int bottom = PageHeight - BottomMargin;
                int size = MarkSize;

                // 左上
                _dc.DrawLine(_markLine, new Point(left - size, top - 0.5), new Point(left, top - 0.5));
                _dc.DrawLine(_markLine, new Point(left - 0.5, top - size), new Point(left - 0.5, top));
                // 右上
                _dc.DrawLine(_markLine, new Point(right, top - 0.5), new Point(right + size, top - 0.5));
                _dc.DrawLine(_markLine, new Point(right + 0.5, top - size), new Point(right + 0.5, top));
                // 左下
                _dc.DrawLine(_markLine, new Point(left - size, bottom + 0.5), new Point(left, bottom + 0.5));
                _dc.DrawLine(_markLine, new Point(left - 0.5, bottom), new Point(left - 0.5, bottom + size));
                // 右下
                _dc.DrawLine(_markLine, new Point(right, bottom + 0.5), new Point(right + size, bottom + 0.5));
                _dc.DrawLine(_markLine, new Point(right + 0.5, bottom), new Point(right + 0.5, bottom + size));
            }
        }

        private readonly Pen _borderPen = new Pen(new SolidColorBrush(Color.FromArgb(255, 128, 128, 128)), 1);
        private readonly Pen _markLine = new Pen(new SolidColorBrush(Color.FromArgb(255, 128, 128, 128)), 1);
    }
}