using GeekDocument.SubSystem.CacheSystem;
using GeekDocument.SubSystem.DocLibSystem;
using GeekDocument.SubSystem.OptionSystem;
using System.IO;
using System.Windows;
using XLogic.Windows.Kernel32;

namespace GeekDocument
{
    public partial class App : Application
    {
        public App()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            Kernel32Interop.AllocConsole();

            // 创建必要文件夹
            string requiredFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer) + "\\GeekDocument\\";
            if (!Directory.Exists(requiredFolder)) Directory.CreateDirectory(requiredFolder);
            // 初始化系统数据
            Options.Instance.Init();
            DocumentTree.Instance.Init();
            CacheManager.Instance.Init();
        }
    }
}