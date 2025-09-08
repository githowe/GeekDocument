using System.Windows.Media;

namespace GeekDocument.SubSystem.WindowSystem.ColorPick.Tool
{
    public interface IColorHandler
    {
        public void InitColor(Color color);

        public void SyncColor(Color color, ColorElement element);
    }
}