using System.Windows;
using System.Windows.Media;

namespace GeekDocument.SubSystem.LayoutEngine
{
    public class ElementLayer : FrameworkElement
    {
        public ElementLayer()
        {
            AddVisualChild(_visual);
            AddLogicalChild(_visual);
        }

        public DrawingContext Open() => _visual.RenderOpen();

        public void Clear() => _visual.RenderOpen().Close();

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index) => _visual;

        private readonly DrawingVisual _visual = new DrawingVisual();
    }
}