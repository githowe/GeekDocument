using GeekDocument.SubSystem.CacheSystem;
using GeekDocument.SubSystem.DocumentSystem;
using Microsoft.Win32;
using System.IO;
using XLogic.Base;

namespace GeekDocument.SubSystem.FileSystem
{
    public class FM
    {
        #region 单例

        private FM() { }
        public static FM Instance { get; } = new FM();

        #endregion

        public void Init()
        {
            // 文档格式
            _document.TypeList.Add(new TypeInfo("极客文档", "gocx"));
            // 图片格式
            _image.TypeList.Add(new TypeInfo("图片", "png,bmp,jpg,jpeg,gif,webp,jfif,tif,tiff"));
            _image.TypeList.Add(new TypeInfo("便携网络图片", "png"));
            _image.TypeList.Add(new TypeInfo("位图", "bmp"));
            _image.TypeList.Add(new TypeInfo("照片", "jpg,jpeg"));
            _image.TypeList.Add(new TypeInfo("Gif", "gif"));
            _image.TypeList.Add(new TypeInfo("Webp", "webp"));
            _image.TypeList.Add(new TypeInfo("Jfif", "jfif"));
            _image.TypeList.Add(new TypeInfo("Tif", "tif,tiff"));
        }

        /// <summary>
        /// 打开浏览文件夹对话框
        /// </summary>
        public string OpenFolderExplorerDialog(string initialDirectory)
        {
            OpenFolderDialog dialog = new OpenFolderDialog
            {
                InitialDirectory = initialDirectory,
            };
            if (dialog.ShowDialog() == true) return dialog.FolderName;
            return "";
        }

        /// <summary>
        /// 打开读取文档对话框
        /// </summary>
        public List<string> OpenReadDocumentDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "打开文档",
                InitialDirectory = DocManager.Instance.GetRecentDocumentPath(),
                Multiselect = true,
                Filter = _document.ToString(),
            };
            if (dialog.ShowDialog() == true) return dialog.FileNames.ToList();
            return [];
        }

        /// <summary>
        /// 打开读取图片对话框
        /// </summary>
        public List<string> OpenReadImageDialog(string title)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = title,
                InitialDirectory = GetImagePath(),
                Filter = _image.ToString(),
                FilterIndex = CacheManager.Instance.Cache.FileManager.RecentImageType,
                Multiselect = true,
            };
            if (dialog.ShowDialog() == true)
            {
                CacheManager.Instance.Cache.FileManager.RecentImagePath = Path.GetDirectoryName(dialog.FileName);
                CacheManager.Instance.Cache.FileManager.RecentImageType = dialog.FilterIndex;
                CacheManager.Instance.SaveCache();
                return dialog.FileNames.ToList();
            }
            return [];
        }

        private string GetImagePath()
        {
            string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string path = CacheManager.Instance.Cache.FileManager.RecentImagePath;
            return CheckFolderPath(path, defaultPath);
        }

        private string CheckFolderPath(string path, string defaultPath)
        {
            if (!Directory.Exists(path)) return defaultPath;
            return path;
        }

        /// <summary>文档文件</summary>
        private readonly FileFilter _document = new FileFilter();
        /// <summary>图片文件</summary>
        private readonly FileFilter _image = new FileFilter();
    }
}