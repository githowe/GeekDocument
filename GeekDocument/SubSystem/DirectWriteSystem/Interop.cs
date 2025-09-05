using System.Runtime.InteropServices;

namespace GeekDocument.SubSystem.DirectWriteSystem
{
    public class Interop
    {
        [DllImport("DWriteCore.dll")]
        public static extern nint CreateDWriteTool();

        [DllImport("DWriteCore.dll", CharSet = CharSet.Unicode)]
        public static extern FontMetrics GetFontMetrics(nint tool, string fontFamilyName, bool bold, bool italic);

        [DllImport("DWriteCore.dll")]
        public static extern void ReleaseDWriteTool(nint tool);
    }
}