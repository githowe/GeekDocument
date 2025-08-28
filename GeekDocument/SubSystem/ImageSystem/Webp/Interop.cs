using System.Runtime.InteropServices;

namespace GeekDocument.SubSystem.ImageSystem.Webp
{
    public class Interop
    {
        [DllImport("webp/WebpCore.dll")]
        public static extern nint CreateWebpReader();

        [DllImport("webp/WebpCore.dll")]
        public static extern int LoadImageFile(nint reader, byte[] sourceData, int size);

        [DllImport("webp/WebpCore.dll")]
        public static extern int GetImageWidth(nint reader);

        [DllImport("webp/WebpCore.dll")]
        public static extern int GetImageHeight(nint reader);

        [DllImport("webp/WebpCore.dll")]
        public static extern int GetFrameCount(nint reader);

        [DllImport("webp/WebpCore.dll")]
        public static extern nint GetFrame(nint reader);

        [DllImport("webp/WebpCore.dll")]
        public static extern nint ClearFrame(nint reader);

        [DllImport("webp/WebpCore.dll")]
        public static extern nint GetFrameData(nint frame);

        [DllImport("webp/WebpCore.dll")]
        public static extern int GetFrameTimestamp(nint frame);
    }
}