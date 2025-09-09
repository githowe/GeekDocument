using System.Drawing.Text;

namespace GeekDocument.AppTool
{
    public class FontManager
    {
        public static List<string> FontList { get; set; } = new List<string>();

        public static void Init()
        {
            InstalledFontCollection collection = new InstalledFontCollection();
            foreach (var font in collection.Families) FontList.Add(font.Name);
        }
    }
}