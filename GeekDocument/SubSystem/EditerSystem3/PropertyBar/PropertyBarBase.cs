using System.Windows.Controls;
using System.Windows.Media;

namespace GeekDocument.SubSystem.EditerSystem3.PropertyBar
{
    public class PropertyBarBase : UserControl
    {
        public string Title { get; set; } = "";

        protected SolidColorBrush _default = new SolidColorBrush(Color.FromRgb(140, 140, 140));
        protected SolidColorBrush _hovered = new SolidColorBrush(Color.FromRgb(255, 255, 255));
    }
}