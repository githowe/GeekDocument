using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GeekDocument.SubSystem.ImageSystem.Bitmap
{
    public class BitmapReader : IImageLoader
    {
        #region 属性

        public int Width { get; private set; } = 0;

        public int Height { get; private set; } = 0;

        /// <summary>动画持续时间。单位：毫秒</summary>
        public int Duration { get; set; } = 0;

        /// <summary>帧列表</summary>
        public List<ImageFrame> FrameList => _frameList;

        #endregion

        public void LoadImageFile(string path)
        {
            // 创建图片实例
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            // 设置加载图片后释放文件
            image.CacheOption = BitmapCacheOption.OnLoad;
            // 设置图片源
            image.UriSource = new Uri(path);
            image.EndInit();

            // 设置图片大小
            Width = image.PixelWidth;
            Height = image.PixelHeight;
            // 创建并添加帧
            ImageFrame frame = new ImageFrame { PixelData = new byte[Width * Height * 4] };
            _frameList.Add(frame);

            // 非目标像素格式
            if (image.Format != PixelFormats.Bgra32)
            {
                // 转换像素格式
                FormatConvertedBitmap convertedBitmap = new FormatConvertedBitmap();
                convertedBitmap.BeginInit();
                convertedBitmap.Source = image;
                convertedBitmap.DestinationFormat = PixelFormats.Bgra32;
                convertedBitmap.EndInit();
                // 复制像素数据
                convertedBitmap.CopyPixels(frame.PixelData, Width * 4, 0);
            }
            // 直接复制像素数据
            else image.CopyPixels(frame.PixelData, Width * 4, 0);
        }

        public void Reset()
        {
            Width = 0;
            Height = 0;
            Duration = 0;
            _frameList.Clear();
        }

        private readonly List<ImageFrame> _frameList = new List<ImageFrame>();
    }
}