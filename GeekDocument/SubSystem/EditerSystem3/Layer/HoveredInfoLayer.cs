using GeekDocument.SubSystem.LayoutEngine;
using System.Windows.Media;
using XLogic.Wpf.Drawing;

namespace GeekDocument.SubSystem.EditerSystem3.Layer
{
    public class HoveredInfoLayer : SingleBoard
    {
        public 布局元素? HoveredElement { get; set; } = null;

        public override void Init()
        {
            _brush.Freeze();
        }

        protected override void OnUpdate()
        {
            if (HoveredElement == null) return;
            _dc.DrawRectangle(_brush, null, HoveredElement.GetViewRect());
        }

        private readonly Brush _brush = new SolidColorBrush(Color.FromArgb(128, 85, 111, 181));
    }
}