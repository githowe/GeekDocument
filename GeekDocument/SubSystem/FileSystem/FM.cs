using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLogic.Base;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

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

        /// <summary>图片文件</summary>
        private readonly FileFilter _image = new FileFilter();
    }
}