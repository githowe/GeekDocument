using System.Runtime.InteropServices;

namespace GeekDocument.SubSystem.DirectWriteSystem
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FontMetrics
    {
        public int UnitsPerEm;
        public short TypoAscender;
        public short TypoDescender;
    }
}