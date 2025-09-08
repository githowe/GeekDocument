using System.Windows.Media;

namespace GeekDocument.AppTool.Ex
{
    public static class ClassExtension
    {
        public static XLogic.Base.Color ToUtilColor(this Color color) => new XLogic.Base.Color(color.R, color.G, color.B);

        public static Color ToMediaColor(this XLogic.Base.Color color) => Color.FromRgb(color.Red, color.Green, color.Blue);
    }
}