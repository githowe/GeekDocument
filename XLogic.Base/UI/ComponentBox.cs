namespace XLogic.Base.UI
{
    /// <summary>
    /// 组件箱
    /// </summary>
    public class ComponentBox<THost> where THost : class
    {
        public Action<string>? ExceptionHandler { get; set; } = null;

        /// <summary>
        /// 添加组件
        /// </summary>
        public TComponent AddComponent<TComponent>(THost host, string name = "未命名组件") where TComponent : Component<THost>, new()
        {
            // 宿主不能为空
            if (host == null) throw new ArgumentNullException(nameof(host));
            // 防止重复添加
            if (_componentDict.ContainsKey(typeof(TComponent)))
                throw new Exception($"已包含类型为“{typeof(TComponent)}”的组件");
            // 创建并添加组件实例
            TComponent instance = new TComponent
            {
                Box = this,
                Host = host,
                Name = name
            };
            _componentList.Add(instance);
            _componentDict.Add(typeof(TComponent), instance);
            // 返回组件实例
            return instance;
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        /// <typeparam name="TComponent">组件类型</typeparam>
        public TComponent GetComponent<TComponent>() where TComponent : Component<THost>
        {
            if (_componentDict.ContainsKey(typeof(TComponent)))
                return _componentDict[typeof(TComponent)] as TComponent;
            throw new Exception($"组件类型“{typeof(TComponent)}”未注册");
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        public void Init()
        {
            foreach (var component in _componentList)
            {
                try
                {
                    component.ReqInit();
                }
                catch (Exception ex)
                {
                    ExceptionHandler?.Invoke($"“{component.Name}”初始化失败：" + ex.Message);
                    continue;
                }
            }
        }

        /// <summary>
        /// 启用组件
        /// </summary>
        public void Enable()
        {
            foreach (var component in _componentList)
            {
                try
                {
                    component.ReqEnable();
                }
                catch (Exception ex)
                {
                    ExceptionHandler?.Invoke($"“{component.Name}”启用失败：" + ex.Message);
                    continue;
                }
            }
        }

        /// <summary>
        /// 重置组件
        /// </summary>
        public void Reset()
        {
            foreach (var component in _componentList)
            {
                try
                {
                    component.ReqReset();
                }
                catch (Exception ex)
                {
                    ExceptionHandler?.Invoke($"“{component.Name}”重置失败：" + ex.Message);
                    continue;
                }
            }
        }

        /// <summary>
        /// 禁用组件
        /// </summary>
        public void Disable()
        {
            foreach (var component in _componentList)
            {
                try
                {
                    component.ReqDisable();
                }
                catch (Exception ex)
                {
                    ExceptionHandler?.Invoke($"“{component.Name}”禁用失败：" + ex.Message);
                    continue;
                }
            }
        }

        /// <summary>
        /// 清空组件
        /// </summary>
        public void Clear()
        {
            foreach (var component in _componentList)
            {
                try
                {
                    // 先禁用组件再移除
                    component.ReqDisable();
                    component.ReqRemove();
                }
                catch (Exception ex)
                {
                    ExceptionHandler?.Invoke($"“{component.Name}”移除失败：" + ex.Message);
                    continue;
                }
            }
            _componentDict.Clear();
        }

        /// <summary>组件列表</summary>
        private readonly List<Component<THost>> _componentList = new List<Component<THost>>();
        /// <summary>组件表</summary>
        private readonly Dictionary<Type, Component<THost>> _componentDict = new Dictionary<Type, Component<THost>>();
    }
}