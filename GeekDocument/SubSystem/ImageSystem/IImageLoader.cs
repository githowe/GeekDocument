namespace GeekDocument.SubSystem.ImageSystem
{
    public interface IImageLoader
    {
        public int Width { get; }

        public int Height { get; }

        public List<ImageFrame> FrameList { get; }

        /// <summary>总持续时间。单位：毫秒</summary>
        public int Duration { get; }

        /// <summary>
        /// 加载图片文件
        /// </summary>
        public void LoadImageFile(string path);

        /// <summary>
        /// 重置加载器
        /// </summary>
        public void Reset();
    }
}