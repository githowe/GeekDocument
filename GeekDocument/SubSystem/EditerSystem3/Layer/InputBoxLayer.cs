using GeekDocument.SubSystem.LayoutEngine;
using System.Windows;
using System.Windows.Media;
using XLogic.Wpf.Drawing;

namespace GeekDocument.SubSystem.EditerSystem3.Layer
{
    /// <summary>
    /// 输入框图层。高亮当前接收输入的行
    /// </summary>
    public class InputBoxLayer : SingleBoard
    {
        public 元素行? Line { get; set; } = null;

        public override void Init()
        {
            _pen.Freeze();
        }

        protected override void OnUpdate()
        {
            if (Line == null) return;

            Rect lineRect = new Rect(Line.Left, Line.Top, Line.ActualWidth, Line.ActualHeight);
            if (lineRect.Width == 0) return;
            lineRect.X += 0.5;
            lineRect.Y += 0.5;
            lineRect.Width -= 1;
            lineRect.Height -= 1;
            _dc.DrawRectangle(null, _pen, lineRect);
        }

        private readonly Pen _pen = new Pen(new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)), 1);
    }
}