namespace XLogic.Wpf.Behavior
{
    /// <summary>
    /// 行为节点
    /// </summary>
    public class BehaviorNode
    {
        public BehaviorNode(string name, Action<BehaviorArgs?>? action)
        {
            Name = name;
            Action = action;
        }

        /// <summary>名称</summary>
        public string Name { get; set; }

        /// <summary>父节点</summary>
        public BehaviorNode? Parent { get; set; }

        /// <summary>动作</summary>
        public Action<BehaviorArgs?>? Action { get; set; }

        /// <summary>键盘按下</summary>
        public Action<string>? KeyDown { get; set; }

        /// <summary>键盘松开</summary>
        public Action<string>? KeyUp { get; set; }

        /// <summary>鼠标滚轮</summary>
        public Action<int>? MouseWheel { get; set; }

        /// <summary>子节点列表</summary>
        public List<BehaviorNode> SubNodeList { get; set; } = new List<BehaviorNode>();

        /// <summary>启用</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>可重复引发</summary>
        public bool RepeatInvoke { get; set; } = true;

        /// <summary>已引发</summary>
        public bool Invoked { get; set; } = false;

        /// <summary>能否引发</summary>
        public bool CanInvoke
        {
            get
            {
                // 不可重复引发且已引发，返回不可引发
                if (!RepeatInvoke && Invoked) return false;
                return true;
            }
        }

        /// <summary>
        /// 查找子节点
        /// </summary>
        public BehaviorNode? FindSubNode(string name) => SubNodeList.Find(node => node.Name == name);

        /// <summary>
        /// 重置节点
        /// </summary>
        public void Reset()
        {
            Invoked = false;
            foreach (var item in SubNodeList) item.Reset();
        }

        public override string ToString() => Name;
    }
}