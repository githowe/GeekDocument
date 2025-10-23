using GeekDocument.SubSystem.ExportSystem.HtmlTool;

namespace GeekDocument.SubSystem.ExportSystem.BdocTool
{
    public class BdocGenerater
    {
        public void AddParagraph(段落节点 node)
        {
            _paragraphList.Add(node);
        }

        public void AddEndParagraph()
        {
            _paragraphList.Add(new 结束段落());
        }

        public List<string> GenerateLineList()
        {
            List<string> result = new List<string>();
            foreach (var item in _paragraphList) result.Add(item.ToLine());
            return result;
        }

        private readonly List<HtmlNode> _paragraphList = new List<HtmlNode>();
    }
}