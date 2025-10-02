using GeekDocument.SubSystem.LayoutEngine;
using System.Windows;
using System.Windows.Media;
using XLogic.Wpf.Drawing;

namespace GeekDocument.SubSystem.EditerSystem3.Layer
{
    public class HighlightLayer : SingleBoard
    {
        public 行内元素? HighlightElement { get; set; } = null;

        public override void Init()
        {
            _brush.Freeze();
            _pen.Freeze();
        }

        protected override void OnUpdate()
        {
            if (HighlightElement == null) return;
            Rect rect = HighlightElement.GetViewRect();
            double left = Math.Round(rect.Left);
            double right = Math.Round(rect.Right);
            double top = Math.Round(rect.Top);
            double bottom = Math.Round(rect.Bottom);
            rect = new Rect(left + 0.5, top + 0.5, right - left - 1, bottom - top - 1);
            _dc.DrawRectangle(_brush, _pen, rect);
        }

        private readonly Brush _brush = new SolidColorBrush(Color.FromArgb(64, 73, 122, 255));
        private readonly Pen _pen = new Pen(new SolidColorBrush(Color.FromArgb(255, 73, 122, 255)), 1);
    }
}