using System.Windows.Controls;

namespace GeekDocument.SubSystem.EditerSystem.Control.PropertyPanel
{
    public class PropertyPanel : UserControl
    {
        public Action? PropertyChanged { get; set; } = null;

        public virtual void Init() { }
    }
}