namespace XLogic.Base.UI
{
    public class Component<THost> where THost : class
    {
        #region 属性

        /// <summary>组件箱</summary>
        public ComponentBox<THost> Box
        {
            get => _box;
            set => _box ??= value;
        }

        /// <summary>宿主</summary>
        public THost Host
        {
            get => _host;
            set => _host ??= value;
        }

        /// <summary>名称</summary>
        public string Name { get; set; } = "未命名组件";

        /// <summary>已启用</summary>
        public bool IsEnabled { get; set; } = false;

        #endregion

        #region 生命周期方法

        /// <summary>
        /// 请求初始化
        /// </summary>
        internal void ReqInit() => Init();

        /// <summary>
        /// 请求启用
        /// </summary>
        public void ReqEnable()
        {
            if (IsEnabled) return;
            Enable();
            IsEnabled = true;
        }

        /// <summary>
        /// 请求重置
        /// </summary>
        public void ReqReset() => Reset();

        /// <summary>
        /// 请求禁用
        /// </summary>
        public void ReqDisable()
        {
            if (!IsEnabled) return;
            Disable();
            IsEnabled = false;
        }

        /// <summary>
        /// 请求移除
        /// </summary>
        internal void ReqRemove() => Remove();

        #endregion

        #region 生命周期

        /// <summary>添加全部组件后调用：初始化此组件。只调用一次</summary>
        protected virtual void Init() { }

        /// <summary>请求启用时调用：启用组件功能</summary>
        protected virtual void Enable() { }

        /// <summary>请求重置时调用：恢复至启用时状态</summary>
        protected virtual void Reset() { }

        /// <summary>请求禁用时调用：恢复至启用前状态</summary>
        protected virtual void Disable() { }

        /// <summary>移除全部组件前调用：恢复至启用前状态，然后执行一些清理工作。只调用一次</summary>
        protected virtual void Remove() { }

        #endregion

        #region 内部方法

        /// <summary>
        /// 获取组件
        /// </summary>
        protected TComponent GetComponent<TComponent>() where TComponent : Component<THost>
        {
            return _box.GetComponent<TComponent>();
        }

        #endregion

        #region 属性字段

        private ComponentBox<THost> _box;
        protected THost _host;

        #endregion
    }
}