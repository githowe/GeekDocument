using System.Windows.Input;

namespace XLogic.Wpf
{
    /// <summary>
    /// 按键处理器
    /// </summary>
    public interface IKeyHandler
    {
        /// <summary>
        /// 处理键盘按下
        /// </summary>
        public void HandleKeyDown(KeyEventArgs e);

        /// <summary>
        /// 处理键盘松开
        /// </summary>
        public void HandleKeyUp(KeyEventArgs e);
    }
}