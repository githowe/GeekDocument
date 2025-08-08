using System.Windows.Media.Imaging;

namespace GeekDocument.SubSystem.ResourceSystem
{
    public class ImageResManager
    {
        #region 单例

        private ImageResManager() { }
        public static ImageResManager Instance { get; } = new ImageResManager();

        #endregion

        #region 公开方法

        /// <summary>
        /// 获取图片
        /// </summary>
        public BitmapImage GetImage(string path)
        {
            // 已加载过此图片，直接返回
            if (_imageResDict.ContainsKey(path))
                return _imageResDict[path].CloneCurrentValue();

            // 创建图片实例
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            // 设置加载图片后释放文件
            image.CacheOption = BitmapCacheOption.OnLoad;
            // 设置图片源
            image.UriSource = new Uri(path);
            image.EndInit();
            // 保存图片引用
            _imageResDict.Add(path, image);

            // 返回图片实例
            return image;
        }

        /// <summary>
        /// 获取小图标
        /// </summary>
        public BitmapImage? GetIcon15(string path)
        {
            if (path == "") return null;
            return GetImage($"pack://application:,,,/Assets/Icon15/{path}");
        }

        /// <summary>
        /// 获取图标
        /// </summary>
        public BitmapImage? GetIcon16(string path)
        {
            if (path == "") return null;
            return GetImage($"pack://application:,,,/Assets/Icon16/{path}");
        }

        public BitmapImage? GetIcon24(string path)
        {
            if (path == "") return null;
            return GetImage($"pack://application:,,,/Assets/Icon24/{path}");
        }

        #endregion

        #region 字段

        private readonly Dictionary<string, BitmapImage> _imageResDict = new Dictionary<string, BitmapImage>();

        #endregion
    }
}