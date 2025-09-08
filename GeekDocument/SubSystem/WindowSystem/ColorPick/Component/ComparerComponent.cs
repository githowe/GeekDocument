using GeekDocument.SubSystem.WindowSystem.ColorPick.Tool;
using System.Windows.Media;

namespace GeekDocument.SubSystem.WindowSystem.ColorPick.Component
{
    public class ComparerComponent : Base
    {
        public Color Color => _color;

        public Action<Color>? ColorChanged { get; set; }

        public override void InitColor(Color color)
        {
            _color = color;
            _host.Grid_Old.Background = new SolidColorBrush(color);
            _host.Grid_New.Background = new SolidColorBrush(color);
        }

        public override void SyncColor(Color color, ColorElement element)
        {
            _color = color;
            _host.Grid_New.Background = new SolidColorBrush(_color);
            ColorChanged?.Invoke(_color);
        }
    }
}