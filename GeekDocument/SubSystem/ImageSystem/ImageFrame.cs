namespace GeekDocument.SubSystem.ImageSystem
{
    /// <summary>
    /// 图片帧
    /// </summary>
    public class ImageFrame
    {
        /// <summary>像素数据</summary>
        public byte[] PixelData { get; set; } = Array.Empty<byte>();

        public int Timestamp { get; set; } = 0;

        public int Duration { get; set; } = 0;

        public override string ToString() => $"时间：{Timestamp}，时长：{Duration}";
    }
}