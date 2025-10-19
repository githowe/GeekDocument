using System.IO;
using System.Windows;

namespace GeekDocument.SubSystem.ResourceSystem
{
    public class FileResManager
    {
        #region 单例

        private FileResManager() { }
        public static FileResManager Instance { get; } = new FileResManager();

        #endregion

        #region 公开方法

        public string GetCssFile(string name)
        {
            Uri uri = new Uri($"pack://application:,,,/Assets/File/Css/{name}.css");
            using (Stream stream = Application.GetResourceStream(uri).Stream)
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public string GetJsFile(string name)
        {
            Uri uri = new Uri($"pack://application:,,,/Assets/File/Js/{name}.js");
            using (Stream stream = Application.GetResourceStream(uri).Stream)
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public void CopyFolder(string sourceFolder, string targetPath)
        {
            // 获取应用程序在磁盘上的目录
            string appDirectory = AppContext.BaseDirectory;
            string sourcePath = Path.Combine(appDirectory, "Resource", sourceFolder);
            if (!Directory.Exists(sourcePath)) return;

            string targetFullPath = Path.Combine(targetPath, sourceFolder);
            if (!Directory.Exists(targetFullPath)) Directory.CreateDirectory(targetFullPath);

            CopySub(sourcePath, targetFullPath);
        }

        private void CopySub(string sourcePath, string targetPath)
        {
            // 创建目标文件夹
            if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
            // 复制所有文件
            foreach (string file in Directory.GetFiles(sourcePath))
            {
                string destFile = Path.Combine(targetPath, Path.GetFileName(file));
                if (File.Exists(destFile)) continue;
                File.Copy(file, destFile, true);
            }
            // 递归复制所有子文件夹
            foreach (string dir in Directory.GetDirectories(sourcePath))
            {
                string destDir = Path.Combine(targetPath, Path.GetFileName(dir));
                CopySub(dir, destDir);
            }
        }

        #endregion
    }
}