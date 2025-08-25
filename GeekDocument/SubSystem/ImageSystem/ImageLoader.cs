using GeekDocument.SubSystem.ImageSystem.Bitmap;

namespace GeekDocument.SubSystem.ImageSystem
{
    public class ImageLoader
    {
        #region 单例

        private ImageLoader() { }
        public static ImageLoader Instance { get; } = new ImageLoader();

        #endregion

        public void Init()
        {
            // 位图
            BitmapReader bitmapReader = new BitmapReader();
            _imageLoaderDict.Add("bmp", bitmapReader);
            _imageLoaderDict.Add("jpg", bitmapReader);
            _imageLoaderDict.Add("jpeg", bitmapReader);
            _imageLoaderDict.Add("jfif", bitmapReader);
            _imageLoaderDict.Add("png", bitmapReader);
            _imageLoaderDict.Add("tif", bitmapReader);
            _imageLoaderDict.Add("tiff", bitmapReader);
            // 动图
            _imageLoaderDict.Add("gif", new Gif.GifReader());
            _imageLoaderDict.Add("webp", new Webp.WebpReader());
        }

        public ImageInfo? LoadImageFile(string path)
        {
            ImageInfo result = new ImageInfo();

            // 获取文件扩展名
            string extension = System.IO.Path.GetExtension(path).ToLowerInvariant().TrimStart('.');
            // 尝试获取对应的加载器
            if (_imageLoaderDict.TryGetValue(extension, out IImageLoader? loader))
            {
                // 加载图片
                loader.LoadImageFile(path);
                // 设置图片信息
                result.Width = loader.Width;
                result.Height = loader.Height;
                result.FrameList.AddRange(loader.FrameList);
                result.Duration = loader.Duration;
                // 重置加载器
                loader.Reset();
                // 返回结果
                return result;
            }
            else
            {
                Console.WriteLine($"不支持的图片格式：{extension}");
                return null;
            }
        }

        /// <summary>加载器表</summary>
        private readonly Dictionary<string, IImageLoader> _imageLoaderDict = new Dictionary<string, IImageLoader>();
    }
}