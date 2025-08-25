namespace GeekDocument.SubSystem.ImageSystem
{
    public class ImageInfo
    {
        public int Width { get; set; } = 0;

        public int Height { get; set; } = 0;

        public List<ImageFrame> FrameList { get; set; } = new List<ImageFrame>();

        public int Duration { get; set; } = 0;
    }
}