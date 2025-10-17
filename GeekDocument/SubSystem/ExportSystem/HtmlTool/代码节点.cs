using GeekDocument.SubSystem.LayoutEngine.Element;
using System.Net;

namespace GeekDocument.SubSystem.ExportSystem.HtmlTool
{
    public class 代码节点 : HtmlNode
    {
        public 代码节点()
        {
            Markup = "pre";
        }

        public 代码 Element { get; set; } = null!;

        public override void Init()
        {
            switch (Element.语言)
            {
                case "Xml":
                case "Html":
                case "Xaml":
                    Class = "language-markup";
                    break;
                case "C#":
                case "CSharp":
                    Class = "language-cs";
                    break;
                case "CSS":
                    Class = "language-css";
                    break;
                case "JavaScript":
                    Class = "language-javascript";
                    break;
                case "Bash":
                    Class = "language-bash";
                    break;
                case "C":
                    Class = "language-c";
                    break;
                case "CPP":
                case "C++":
                    Class = "language-cpp";
                    break;
                case "Java":
                    Class = "language-java";
                    break;
                case "Regex":
                    Class = "language-regex";
                    break;
            }
        }

        public override string ToLine()
        {
            Element.更新源代码();
            string code = WebUtility.HtmlEncode(Element.源码);
            string startTag = GenerateStartTag();
            string endTag = GenerateEndTag();
            return $"{startTag}<code class=\"{Class}\">{code}</code>{endTag}";
        }
    }
}