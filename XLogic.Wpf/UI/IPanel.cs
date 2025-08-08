namespace XLogic.Wpf.UI
{
    /// <summary>
    /// 面板接口
    /// </summary>
    public interface IPanel : IKeyHandler
    {
        /// <summary>
        /// 添加焦点
        /// </summary>
        public void AddFocus();

        /// <summary>
        /// 移除焦点
        /// </summary>
        public void RemoveFocus();
    }
}