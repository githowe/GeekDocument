using System.Windows.Media;

namespace GeekDocument.SubSystem.WindowSystem.ColorPick.Tool
{
    /// <summary>
    /// 颜色同步工具
    /// </summary>
    public class ColorSyncTool
    {
        /// <summary>
        /// 注册同步处理器
        /// </summary>
        public void RegisterSyncHandler(IColorHandler handler)
        {
            _handlerList.Add(handler);
        }

        /// <summary>
        /// 初始化颜色
        /// </summary>
        public void InitColor(Color color)
        {
            foreach (var handler in _handlerList) handler.InitColor(color);
        }

        /// <summary>
        /// 更新颜色
        /// </summary>
        public void UpdateColor(IColorHandler sender, Color color, ColorElement element = ColorElement.Full)
        {
            foreach (var handler in _handlerList)
            {
                if (handler != sender)
                    handler.SyncColor(color, element);
            }
        }

        private readonly List<IColorHandler> _handlerList = new List<IColorHandler>();
    }
}