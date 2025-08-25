using System.Runtime.InteropServices;

namespace GeekDocument.SubSystem.ImageSystem.Webp
{
    public class Interop
    {
        [DllImport("WebpCore.dll")]
        public static extern nint CreateWebpReader();

        [DllImport("WebpCore.dll", CharSet = CharSet.Unicode)]
        public static extern int LoadImageFile(nint reader, string path);

        [DllImport("WebpCore.dll")]
        public static extern int GetImageWidth(nint reader);

        [DllImport("WebpCore.dll")]
        public static extern int GetImageHeight(nint reader);

        [DllImport("WebpCore.dll")]
        public static extern int GetFrameCount(nint reader);

        [DllImport("WebpCore.dll")]
        public static extern nint GetFrame(nint reader);

        [DllImport("WebpCore.dll")]
        public static extern nint ClearFrame(nint reader);

        [DllImport("WebpCore.dll")]
        public static extern nint GetFrameData(nint frame);

        [DllImport("WebpCore.dll")]
        public static extern int GetFrameTimestamp(nint frame);
    }
}