namespace GeekDocument.SubSystem.ExportSystem.HtmlTool
{
    public class HtmlGenerater
    {
        public HtmlGenerater()
        {
            // 添加头元素
            HtmlNode head = new HtmlNode("head");
            _rootNode.AddSubNode(head);
            // 添加标题元素至头元素
            HtmlNode title = new HtmlNode("title");
            head.AddSubNode(title);
            // 添加链接元素至头元素
            链接节点 link = new 链接节点
            {
                Href = "article.css"
            };
            head.AddSubNode(link);
            link = new 链接节点
            {
                Href = "Prism.css"
            };
            head.AddSubNode(link);
            // 添加脚本元素至头元素
            脚本节点 script = new 脚本节点
            {
                Src = "Prism.js"
            };
            head.AddSubNode(script);
            script = new 脚本节点
            {
                Src = "article.js"
            };
            head.AddSubNode(script);
            // 添加主体元素
            HtmlNode body = new HtmlNode("body");
            body.Class = "back";
            body.PropertyList.Add(new NodeProperty
            {
                Name = "onload",
                Value = "pageLoaded()"
            });
            _rootNode.AddSubNode(body);
            // 添加段落盒子至主体元素
            body.AddSubNode(_paragraphBox);
            _paragraphBox.Class = "paper";
        }

        /// <summary>
        /// 添加段落
        /// </summary>
        public void AddParagraph(HtmlNode node)
        {
            node.Parent = _rootNode;
            _paragraphBox.AddSubNode(node);
        }

        /// <summary>
        /// 添加结束段落。该段落没有任何内容，仅用于修复页面底端的间距问题，没有此段落，页面底端会多出一段间距
        /// </summary>
        public void AddEndParagraph()
        {
            结束段落 endParagraph = new 结束段落
            {
                Parent = _rootNode
            };
            _paragraphBox.AddSubNode(endParagraph);
        }

        public List<string> GenerateLineList()
        {
            List<string> result = new List<string>();

            result.Add("<!DOCTYPE html>");
            result.AddRange(_rootNode.ToLineList());

            return result;
        }

        private readonly HtmlNode _rootNode = new HtmlNode("html");
        private readonly HtmlNode _paragraphBox = new HtmlNode("div");
    }
}