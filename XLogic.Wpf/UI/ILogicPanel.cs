namespace XLogic.Wpf.UI
{
    public interface ILogicPanel
    {
        /// <summary>面板名称</summary>
        string PanelName { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        void Init();

        /// <summary>
        /// 重置
        /// </summary>
        void Reset();

        /// <summary>
        /// 清理
        /// </summary>
        void Clear();
    }
}