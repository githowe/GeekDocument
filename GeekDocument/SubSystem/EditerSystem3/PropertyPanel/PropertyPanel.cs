using GeekDocument.SubSystem.ResourceSystem;
using System.Windows.Controls;
using System.Windows.Media;

namespace GeekDocument.SubSystem.EditerSystem3.PropertyPanel
{
    public class PropertyPanel : UserControl
    {
        public int SelectStartIndex { get; set; } = -1;

        public int SelectEndIndex { get; set; } = -1;

        public Action? PropertyChanged { get; set; } = null;

        public virtual void Init() { }

        protected ImageSource? GetIcon(string name) => ImageResManager.Instance.GetIcon15($"{name}.png");
    }
}