using GeekDocument.SubSystem.ExportSystem.HtmlTool;
using GeekDocument.SubSystem.LayoutEngine;
using GeekDocument.SubSystem.ResourceSystem;
using System.IO;

namespace GeekDocument.SubSystem.ExportSystem.Exporter
{
    public class BdocExporter : IExporter
    {
        public void Export(页面 page, string path, string name)
        {
            HtmlGenerater generater = new HtmlGenerater();

            // 遍历段落
            foreach (var item in page.段落列表)
            {
                段落节点 node = new 段落节点
                {
                    Element = item
                };
                node.Init();
                generater.AddParagraph(node);
            }

            List<string> lines = generater.GenerateLineList();
            string text = string.Join("\n", lines);
            File.WriteAllText(Path.Combine(path, name + ".html"), text);

            string cssFilePath = Path.Combine(path, "article.css");
            if (!File.Exists(cssFilePath))
            {
                string cssContent = FileResManager.Instance.GetCssFile("article");
                File.WriteAllText(cssFilePath, cssContent);
            }
        }
    }
}