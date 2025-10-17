namespace GeekDocument.SubSystem.ExportSystem.HtmlTool
{
    public class HtmlNode
    {
        #region 构造方法

        public HtmlNode() { }

        public HtmlNode(string markup) => Markup = markup;

        #endregion

        #region 属性

        public string Markup { get; set; } = string.Empty;

        public string InnerText { get; set; } = string.Empty;

        public HtmlNode? Parent { get; set; } = null;

        public List<HtmlNode> SubNodeList { get; set; } = new List<HtmlNode>();

        public int Deep
        {
            get
            {
                if (Parent == null) return 0;
                return Parent.Deep + 1;
            }
        }

        public string Class { get; set; } = string.Empty;

        #endregion

        #region 公开方法

        public virtual void Init() { }

        public void AddSubNode(HtmlNode node)
        {
            node.Parent = this;
            SubNodeList.Add(node);
        }

        public virtual List<string> ToLineList()
        {
            List<string> result = new List<string>();

            // 无子节点
            if (SubNodeList.Count == 0)
            {
                result.Add(GenerateIndent() + GenerateStartTag() + InnerText + GenerateEndTag());
            }
            // 有子节点
            else
            {
                result.Add(GenerateIndent() + GenerateStartTag());
                foreach (HtmlNode node in SubNodeList)
                {
                    if (node is 段落节点 paragraphNode) result.Add(paragraphNode.ToLine());
                    else result.AddRange(node.ToLineList());
                }
                result.Add(GenerateIndent() + GenerateEndTag());
            }

            return result;
        }

        public virtual string ToLine() => "";

        #endregion

        #region 内部方法

        /// <summary>
        /// 生成缩进
        /// </summary>
        protected string GenerateIndent() => new string(' ', Deep * 4);

        /// <summary>
        /// 生成开始标签
        /// </summary>
        protected string GenerateStartTag()
        {
            if (!string.IsNullOrEmpty(Class)) return $"<{Markup} class=\"{Class}\">";
            return $"<{Markup}>";
        }

        /// <summary>
        /// 生成结束标签
        /// </summary>
        protected string GenerateEndTag() => $"</{Markup}>";

        #endregion
    }
}