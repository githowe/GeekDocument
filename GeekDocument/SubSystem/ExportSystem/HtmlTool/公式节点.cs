using GeekDocument.SubSystem.LayoutEngine.Element;
using System.Net;

namespace GeekDocument.SubSystem.ExportSystem.HtmlTool
{
    public class 公式节点 : HtmlNode
    {
        public 公式节点()
        {
            Markup = "span";
            Class = "formula";
        }

        public 公式 Element { get; set; } = null!;

        public override string ToLine()
        {
            string startTag = GenerateStartTag();
            string endTag = GenerateEndTag();

            string innerText = Element.Latex;
            innerText = WebUtility.HtmlEncode(innerText);

            return $"{startTag}{innerText}{endTag}";
        }
    }
}