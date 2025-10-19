namespace GeekDocument.SubSystem.ExportSystem.HtmlTool
{
    public class 脚本节点 : HtmlNode
    {
        public string Src { get; set; } = string.Empty;

        public override List<string> ToLineList()
        {
            return new List<string> { $"{GenerateIndent()}<script defer src=\"{Src}\"></script>" };
        }
    }
}