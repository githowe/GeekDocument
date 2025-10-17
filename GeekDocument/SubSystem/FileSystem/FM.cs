using GeekDocument.SubSystem.CacheSystem;
using GeekDocument.SubSystem.DocumentSystem;
using Microsoft.Win32;
using System.IO;
using System.Windows;
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
            _document.TypeList.Add(new TypeInfo("极客文档", "gdoc"));
            // 图片格式
            _image.TypeList.Add(new TypeInfo("图片", "png,bmp,jpg,jpeg,gif,webp,jfif,tif,tiff"));
            _image.TypeList.Add(new TypeInfo("便携网络图片", "png"));
            _image.TypeList.Add(new TypeInfo("位图", "bmp"));
            _image.TypeList.Add(new TypeInfo("照片", "jpg,jpeg"));
            _image.TypeList.Add(new TypeInfo("Gif", "gif"));
            _image.TypeList.Add(new TypeInfo("Webp", "webp"));
            _image.TypeList.Add(new TypeInfo("Jfif", "jfif"));
            _image.TypeList.Add(new TypeInfo("Tif", "tif,tiff"));
            // 导出格式
            _export.TypeList.Add(new TypeInfo("PDF", "pdf"));
            _export.TypeList.Add(new TypeInfo("Word文档", "docx"));
            _export.TypeList.Add(new TypeInfo("Markdown", "md"));
            _export.TypeList.Add(new TypeInfo("网页", "html"));
            _export.TypeList.Add(new TypeInfo("纯文本", "txt"));
            _export.TypeList.Add(new TypeInfo("图片", "png"));
            _export.TypeList.Add(new TypeInfo("博客文档", "bdoc"));
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

        /// <summary>
        /// 打开导出文件对话框
        /// </summary>
        public string OpenExportFileDialog(string fileName)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                InitialDirectory = GetExportFilePath(),
                FileName = fileName,
                Filter = _export.ToString(),
                FilterIndex = CacheManager.Instance.Cache.FileManager.RecentExportType,
            };
            if (dialog.ShowDialog() == true)
            {
                CacheManager.Instance.Cache.FileManager.RecentExportPath = Path.GetDirectoryName(dialog.FileName);
                CacheManager.Instance.Cache.FileManager.RecentExportType = dialog.FilterIndex;
                CacheManager.Instance.SaveCache();
                return dialog.FileName;
            }
            return "";
        }

        private string GetImagePath()
        {
            string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string path = CacheManager.Instance.Cache.FileManager.RecentImagePath;
            return CheckFolderPath(path, defaultPath);
        }

        private string GetExportFilePath()
        {
            string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\GeekDocument\\Export";
            if (!Directory.Exists(defaultPath)) Directory.CreateDirectory(defaultPath);
            string recentPath = CacheManager.Instance.Cache.FileManager.RecentExportPath;
            return CheckFolderPath(recentPath, defaultPath);
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
        /// <summary>导出文件</summary>
        private readonly FileFilter _export = new FileFilter();
    }
}