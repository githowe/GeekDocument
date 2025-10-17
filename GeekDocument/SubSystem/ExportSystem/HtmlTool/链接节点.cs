namespace GeekDocument.SubSystem.ExportSystem.HtmlTool
{
    public class 链接节点 : HtmlNode
    {
        public string Rel { get; set; } = "stylesheet";

        public string Type { get; set; } = "text/css";

        public string Href { get; set; } = "";

        public override List<string> ToLineList()
        {
            return new List<string> { $"{GenerateIndent()}<link rel=\"{Rel}\" type=\"{Type}\" href=\"{Href}\" />" };
        }
    }
}