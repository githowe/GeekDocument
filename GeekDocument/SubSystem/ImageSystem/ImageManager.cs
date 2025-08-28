using GeekDocument.SubSystem.WindowSystem;
using System.IO;
using System.Security.Cryptography;

namespace GeekDocument.SubSystem.ImageSystem
{
    /// <summary>
    /// 图片管理器。管理文档中的图片源数据
    /// </summary>
    public class ImageManager
    {
        #region 单例

        private ImageManager() { }
        public static ImageManager Instance { get; } = new ImageManager();

        #endregion

        #region 公开方法

        /// <summary>
        /// 获取图片文件数据
        /// </summary>
        public ImageFileData GetImageFileData(string filePath)
        {
            // 读取文件的字节数据
            byte[] imageData = File.ReadAllBytes(filePath);
            // 计算哈希值作为唯一标识
            MD5 md5 = MD5.Create();
            byte[] hashBytes = md5.ComputeHash(imageData);
            string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            md5.Dispose();
            // 返回图片文件数据
            return new ImageFileData
            {
                Type = Path.GetExtension(filePath).ToLowerInvariant().TrimStart('.'),
                Hash = hashString,
                Data = imageData
            };
        }

        /// <summary>
        /// 添加图片文件数据
        /// </summary>
        public void AddFileData(ImageFileData fileData)
        {
            _imageFileData.TryAdd(fileData.Hash, fileData);
        }

        /// <summary>
        /// 查找图片文件数据
        /// </summary>
        public ImageFileData? FindFileData(string hash)
        {
            if (_imageFileData.TryGetValue(hash, out ImageFileData? fileData)) return fileData;
            return null;
        }

        /// <summary>
        /// 解码图片
        /// </summary>
        public void DecodeImage(string hash)
        {
            // 已存在解码后的数据
            if (_imageInfoDict.ContainsKey(hash)) return;
            // 不存在文件数据
            if (!_imageFileData.ContainsKey(hash)) return;

            ImageFileData fileData = _imageFileData[hash];
            ImageInfo? imageInfo = ImageLoader.Instance.LoadImageFile(fileData.Data, fileData.Type);
            if (imageInfo == null)
            {
                WM.ShowErrorTip($"解码图片失败。哈希值：{hash}");
                return;
            }
            _imageInfoDict.Add(hash, imageInfo);
        }

        /// <summary>
        /// 添加图片信息
        /// </summary>
        public void AddImageInfo(string hash, ImageInfo imageInfo)
        {
            _imageInfoDict.TryAdd(hash, imageInfo);
        }

        /// <summary>
        /// 查找图片信息
        /// </summary>
        public ImageInfo? FindImageInfo(string hash)
        {
            if (_imageInfoDict.TryGetValue(hash, out ImageInfo? info)) return info;
            return null;
        }

        #endregion

        #region 字段

        /// <summary>图片文件数据：哈希值 - 文件数据</summary>
        private readonly Dictionary<string, ImageFileData> _imageFileData = new Dictionary<string, ImageFileData>();
        /// <summary>图片信息：哈希值 - 图片信息</summary>
        private readonly Dictionary<string, ImageInfo> _imageInfoDict = new Dictionary<string, ImageInfo>();

        #endregion
    }
}