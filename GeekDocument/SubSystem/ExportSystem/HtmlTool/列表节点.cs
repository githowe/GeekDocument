using GeekDocument.SubSystem.LayoutEngine.Element;
using System.Text;

namespace GeekDocument.SubSystem.ExportSystem.HtmlTool
{
    public class 列表项节点 : HtmlNode
    {
        public 列表项节点()
        {
            Markup = "li";
        }

        public 项信息 Element { get; set; } = null!;

        public override void Init()
        {

        }

        public override string ToLine()
        {
            string startTag = GenerateStartTag();
            string endTag = GenerateEndTag();
            段落节点 node = new 段落节点
            {
                Element = Element.段落,
                ListItemContent = true
            };
            node.Init();
            return $"{startTag}{node.ToLine()}{endTag}";
        }
    }

    public class 列表节点 : HtmlNode
    {
        public 列表节点()
        {
            Markup = "ul";
        }

        public 列表 Element { get; set; } = null!;

        public override void Init()
        {
            Class = "inlineElement custom-square-list";
            Element.更新项信息列表();
            // 遍历项信息
            foreach (var item in Element.项信息列表)
            {
                列表项节点 node = new 列表项节点
                {
                    Element = item
                };
                node.Init();
                _listItemList.Add(node);
            }
        }

        public override string ToLine()
        {
            string startTag = GenerateStartTag();
            string endTag = GenerateEndTag();
            StringBuilder builder = new StringBuilder();
            foreach (var item in _listItemList)
            {
                builder.Append(item.ToLine());
            }

            return $"{startTag}{builder}{endTag}";
        }

        private readonly List<HtmlNode> _listItemList = new List<HtmlNode>();
    }
}