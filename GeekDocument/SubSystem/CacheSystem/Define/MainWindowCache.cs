using System.Windows;

namespace GeekDocument.SubSystem.CacheSystem.Define
{
    /// <summary>
    /// 主窗口缓存
    /// </summary>
    public class MainWindowCache
    {
        public WindowState State { get; set; } = WindowState.Normal;
        public int Width { get; set; } = 1600;
        public int Height { get; set; } = 900;

        public bool LeftPanelHided { get; set; } = false;
        public bool RightPanelHided { get; set; } = true;

        public int LeftPanelWidth { get; set; } = 300;
        public int RightPanelWidth { get; set; } = 300;
    }
}