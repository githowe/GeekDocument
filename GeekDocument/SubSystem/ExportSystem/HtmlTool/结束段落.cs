namespace GeekDocument.SubSystem.ExportSystem.HtmlTool
{
    public class 结束段落 : HtmlNode
    {
        public 结束段落()
        {
            Markup = "p";
            Class = "endParagraph";
        }

        public override string ToLine() => "<p class=\"endParagraph\"></p>";
    }
}