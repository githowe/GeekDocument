namespace XLogic.Wpf.UI
{
    /// <summary>
    /// 编辑器接口
    /// </summary>
    public interface IEditer
    {
        /// <summary>编辑器名称</summary>
        public string EditerName { get; }

        /// <summary>已保存</summary>
        public bool Saved { get; }

        /// <summary>初始化</summary>
        public void Init();

        /// <summary>重置</summary>
        public void Reset();

        /// <summary>关闭</summary>
        public void Close();

        /// <summary>保存</summary>
        public void Save();
    }
}