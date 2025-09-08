using GeekDocument.SubSystem.WindowSystem.ColorPick.Tool;
using System.Windows.Media;
using XLogic.Base.UI;

namespace GeekDocument.SubSystem.WindowSystem.ColorPick.Component
{
    public abstract class Base : Component<ColorPicker>, IColorHandler
    {
        public abstract void InitColor(Color color);

        public abstract void SyncColor(Color color, ColorElement element);

        protected Color _color;
        protected bool _updateOnly = false;
    }
}