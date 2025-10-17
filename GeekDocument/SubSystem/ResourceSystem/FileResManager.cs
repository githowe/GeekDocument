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

        #endregion
    }
}