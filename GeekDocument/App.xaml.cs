using GeekDocument.AppTool;
using GeekDocument.SubSystem.CacheSystem;
using GeekDocument.SubSystem.DirectWriteSystem;
using GeekDocument.SubSystem.DocLibSystem;
using GeekDocument.SubSystem.EditerSystem.Tool;
using GeekDocument.SubSystem.EventSystem;
using GeekDocument.SubSystem.FileSystem;
using GeekDocument.SubSystem.ImageSystem;
using GeekDocument.SubSystem.OptionSystem;
using GeekDocument.SubSystem.ResourceSystem;
using GeekDocument.SubSystem.StyleSystem;
using GeekDocument.SubSystem.TimeSystem;
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
#if DEBUG
            Kernel32Interop.AllocConsole();
#endif

            // 创建必要文件夹
            string requiredFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\GeekDocument\\";
            if (!Directory.Exists(requiredFolder)) Directory.CreateDirectory(requiredFolder);
            // 初始化系统数据
            Options.Instance.Init();
            DocumentTree.Instance.Init();
            CacheManager.Instance.Init();
            StyleManager.Instance.Init();
            CursorManager.Instance.Init();
            // 初始化系统服务
            FontManager.Init();
            DWriteTool.Instance.Init();
            EM.Instance.Init();
            FM.Instance.Init();
            ImageLoader.Instance.Init();
            AppWatch.Instance.Start();
            CodeTool.Instance.Init();
        }
    }
}