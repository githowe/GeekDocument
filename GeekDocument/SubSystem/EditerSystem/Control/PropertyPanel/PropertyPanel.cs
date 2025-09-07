using GeekDocument.SubSystem.ResourceSystem;
using System.Windows.Controls;
using System.Windows.Media;

namespace GeekDocument.SubSystem.EditerSystem.Control.PropertyPanel
{
    public class PropertyPanel : UserControl
    {
        public Action? PropertyChanged { get; set; } = null;

        public virtual void Init() { }

        protected ImageSource? GetIcon(string name) => ImageResManager.Instance.GetIcon15($"{name}.png");
    }
}