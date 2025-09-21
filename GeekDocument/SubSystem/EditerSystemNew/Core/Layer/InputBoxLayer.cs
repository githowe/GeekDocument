using GeekDocument.SubSystem.LayoutEngine.Tool;
using System.Windows;
using System.Windows.Media;
using XLogic.Wpf.Drawing;

namespace GeekDocument.SubSystem.EditerSystemNew.Core.Layer
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

            Rect lineRect = Line.GetViewRect();
            if (lineRect.Width == 0) return;
            lineRect.X += 0.5;
            lineRect.Y += 0.5;
            lineRect.Width -= 1;
            lineRect.Height -= 1;
            _dc.DrawRectangle(null, _pen, lineRect);
        }

        private readonly Pen _pen = new Pen(new SolidColorBrush(Color.FromArgb(64, 255, 255, 255)), 1);
    }
}