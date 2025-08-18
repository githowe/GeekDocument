namespace XLogic.Base.StateTree
{
    /// <summary>
    /// 状态树
    /// </summary>
    public class StateTree
    {
        public StateTree(string name = "") => Root.Name = name;

        public StateNode Root { get; set; } = new StateNode();

        /// <summary>
        /// 新建节点
        /// </summary>
        public StateNode NewNode(string name, Func<bool>? match, Action? action, StateNode? parent = null)
        {
            // 创建节点
            StateNode newNode = new StateNode
            {
                Name = name,
                Matched = match,
                Action = action,
            };
            // 添加至根节点
            if (parent == null)
            {
                Root.NodeList.Add(newNode);
                newNode.Parent = Root;
            }
            // 添加至父节点
            else
            {
                parent.NodeList.Add(newNode);
                newNode.Parent = parent;
            }
            // 返回新节点
            return newNode;
        }

        public void CheckTree() => Root.CheckNode();

        /// <summary>
        /// 打印节点树
        /// </summary>
        public void PrintNodeTree()
        {
            foreach (var node in Root.NodeList) PrintNode(node, 0);
        }

        private void PrintNode(StateNode node, int depth)
        {
            // 打印当前节点
            Console.WriteLine(new string(' ', depth * 4) + node.Name);
            // 遍历子节点
            foreach (var subNode in node.NodeList) PrintNode(subNode, depth + 1);
        }

        public void Execute()
        {
            // 遍历节点
            foreach (var node in Root.NodeList)
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
    }
}