using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLogic.Base;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using GeekDocument.SubSystem.DocumentSystem;

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
            _document.TypeList.Add(new TypeInfo("文本文件", "txt"));
            // 图片格式
            _image.TypeList.Add(new TypeInfo("便携网络图片", "png"));
            _image.TypeList.Add(new TypeInfo("位图", "bmp"));
            _image.TypeList.Add(new TypeInfo("照片", "jpg"));
            _image.TypeList.Add(new TypeInfo("Gif动画", "gif"));
            _image.TypeList.Add(new TypeInfo("Webp", "webp"));
        }

        /// <summary>
        /// 打开浏览文件夹对话框
        /// </summary>
        public string OpenFolderExplorerDialog(string initialDirectory)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = initialDirectory,
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }
            return "";
        }

        /// <summary>
        /// 打开读取文档对话框
        /// </summary>
        public List<string> OpenReadDocumentDialog()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                Title = "打开文档",
                InitialDirectory = DocManager.Instance.GetRecentDocumentPath(),
                Multiselect = true,
            };
            dialog.Filters.Add(new CommonFileDialogFilter("极客文档", "gdoc"));
            dialog.Filters.Add(new CommonFileDialogFilter("纯文本", "txt;ini;cfg;json;mk"));
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                return dialog.FileNames.ToList();
            return [];
        }

        /// <summary>文档文件</summary>
        private readonly FileFilter _document = new FileFilter();
        /// <summary>图片文件</summary>
        private readonly FileFilter _image = new FileFilter();
    }
}