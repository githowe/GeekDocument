using GeekDocument.SubSystem.LayoutEngine;
using GeekDocument.SubSystem.LayoutEngine.Element;
using System.Net;
using System.Text;

namespace GeekDocument.SubSystem.ExportSystem.HtmlTool
{
    public class 段落节点 : HtmlNode
    {
        public 段落节点()
        {
            Markup = "p";
            Class = "paragraph";
        }

        public 段落 Element { get; set; } = null!;

        public bool ListItemContent { get; set; } = false;

        public override void Init()
        {
            // 对齐方式
            Style style = new Style();
            switch (Element.水平对齐)
            {
                case 水平对齐方式.Left:
                    style.StyleItemList.Add(new Item_Enum("text-align", "left"));
                    break;
                case 水平对齐方式.Center:
                    style.StyleItemList.Add(new Item_Enum("text-align", "center"));
                    break;
                case 水平对齐方式.Right:
                    style.StyleItemList.Add(new Item_Enum("text-align", "right"));
                    break;
            }
            // 首行缩进
            double indent = Element.获取真实首行缩进();
            if (indent > 0 && Element.水平对齐 != 水平对齐方式.Center) style.StyleItemList.Add(new Item_Double("text-indent", indent));
            if (style.StyleItemList.Count > 0)
                PropertyList.Add(new NodeProperty { Name = "style", Value = style.ToLine() });
            _textAlign = style.ToLine();
            // 获取内嵌元素列表
            List<行内元素> elementList = Element.获取内嵌元素();
            // 没有内嵌元素
            if (elementList.Count == 0)
            {
                InnerText = Element.获取文本();
                InnerText = WebUtility.HtmlEncode(InnerText);
                return;
            }
            // 遍历内嵌元素
            foreach (var element in elementList)
            {
                if (element is 图片 图片)
                {
                    图片节点 node = new 图片节点
                    {
                        Element = 图片
                    };
                    node.Init();
                    _innerElementList.Add(node);
                }
                else if (element is 列表 列表)
                {
                    列表节点 node = new 列表节点
                    {
                        Element = 列表
                    };
                    node.Init();
                    _innerElementList.Add(node);
                }
                else if (element is 公式 公式)
                {

                }
                else if (element is 表格 表格)
                {

                }
                else if (element is 代码 代码)
                {
                    代码节点 node = new 代码节点
                    {
                        Element = 代码
                    };
                    node.Init();
                    _innerElementList.Add(node);
                }
            }
        }

        public override string ToLine()
        {
            if (Element.全部行内元素.Count == 0) Class = "emptyParagraph";
            else
            {
                switch (Element.字号)
                {
                    case 24:
                        Class = "header_01";
                        break;
                    case 22:
                        Class = "header_02";
                        break;
                    case 20:
                        Class = "header_03";
                        break;
                    case 18:
                        Class = "header_04";
                        break;
                }
            }

            string startTag = GenerateStartTag();
            string endTag = GenerateEndTag();

            // 没有内嵌元素
            if (_innerElementList.Count == 0)
            {
                if (ListItemContent) return GenerateIndent() + InnerText;
                else return $"{GenerateIndent()}{startTag}{InnerText}{endTag}";
            }
            else
            {
                // 获取段落文本
                string innerText = Element.获取文本();
                // 转义网页特殊字符
                innerText = WebUtility.HtmlEncode(innerText);
                // 分割文本
                string[] textArray = innerText.Split('\u200b');
                // 遍历内嵌元素，并转换为行
                List<string> lineList = new List<string>();
                foreach (var item in _innerElementList)
                {
                    lineList.Add(item.ToLine());
                }
                if (lineList.Count != textArray.Length - 1)
                    throw new Exception("内嵌元素数量与文本片段数量不匹配");
                // 拼接文本与内嵌元素
                StringBuilder builder = new StringBuilder();
                for (int index = 0; index < textArray.Length; index++)
                {
                    // 添加文本
                    builder.Append(textArray[index]);
                    // 添加内嵌元素
                    if (index < lineList.Count) builder.Append(lineList[index]);
                }
                // 返回结果
                if (_textAlign != "")
                    return $"{GenerateIndent()}<div style=\"{_textAlign}\">{builder}</div>";
                return $"{GenerateIndent()}<div>{builder}</div>";
            }
        }

        private readonly List<HtmlNode> _innerElementList = new List<HtmlNode>();
        private string _textAlign = "";
    }
}