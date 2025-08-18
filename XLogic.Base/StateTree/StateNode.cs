using XLogic.Base.Ex;

namespace XLogic.Base.StateTree
{
    /// <summary>
    /// 状态节点
    /// </summary>
    public class StateNode
    {
        public string Name { get; set; } = "";

        public Func<bool>? Matched { get; set; } = null;

        public Action? Action { get; set; } = null;

        public StateNode? Parent { get; set; } = null;

        public List<StateNode> NodeList { get; set; } = new List<StateNode>();

        public override string ToString() => Name;

        public void CheckNode()
        {
            // 检查当前节点
            if (Parent != null && Matched == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("节点“" + GetNodePath() + "”没有匹配函数");
                Console.ResetColor();
            }
            // 检查子节点
            foreach (var node in NodeList) node.CheckNode();
        }

        public void Execute()
        {
            // 如果有子节点，则执行子节点
            if (NodeList.Count > 0)
            {
                foreach (var node in NodeList)
                {
                    // 没有匹配函数，跳过
                    if (node.Matched == null) continue;
                    // 只执行第一个匹配的节点
                    if (node.Matched())
                    {
                        node.Execute();
                        break;
                    }
                }
            }
            // 否则执行当前节点
            else if (Action != null)
            {
                Console.WriteLine("执行路径：" + GetNodePath());
                Action.Invoke();
            }
        }

        public string GetNodePath()
        {
            List<string> path = new List<string>();
            StateNode? node = this;
            while (node != null)
            {
                path.Add(node.Name);
                node = node.Parent;
            }
            path.Reverse();
            return path.ToListString(" > ");
        }
    }
}