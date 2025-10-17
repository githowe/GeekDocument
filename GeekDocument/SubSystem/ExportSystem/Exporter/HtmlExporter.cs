using GeekDocument.SubSystem.ExportSystem.HtmlTool;
using GeekDocument.SubSystem.LayoutEngine;
using GeekDocument.SubSystem.ResourceSystem;
using System.IO;

namespace GeekDocument.SubSystem.ExportSystem.Exporter
{
    public class HtmlExporter : IExporter
    {
        public void Export(页面 page, string path, string name)
        {
            // 创建生成器
            HtmlGenerater generater = new HtmlGenerater();
            // 遍历段落，添加段落节点至生成器
            foreach (var item in page.段落列表)
            {
                段落节点 node = new 段落节点
                {
                    Element = item
                };
                node.Init();
                generater.AddParagraph(node);
            }
            // 生成行列表，写入文件
            List<string> lines = generater.GenerateLineList();
            string text = string.Join("\n", lines);
            File.WriteAllText(Path.Combine(path, name + ".html"), text);
            // 复制资源文件
            string cssFilePath = Path.Combine(path, "article.css");
            if (!File.Exists(cssFilePath))
            {
                string cssContent = FileResManager.Instance.GetCssFile("article");
                File.WriteAllText(cssFilePath, cssContent);
            }
            cssFilePath = Path.Combine(path, "Prism.css");
            if (!File.Exists(cssFilePath))
            {
                string cssContent = FileResManager.Instance.GetCssFile("Prism");
                File.WriteAllText(cssFilePath, cssContent);
            }
            string jsFilePath = Path.Combine(path, "Prism.js");
            if (!File.Exists(jsFilePath))
            {
                string jsContent = FileResManager.Instance.GetJsFile("Prism");
                File.WriteAllText(jsFilePath, jsContent);
            }
            jsFilePath = Path.Combine(path, "article.js");
            if (!File.Exists(jsFilePath))
            {
                string jsContent = FileResManager.Instance.GetJsFile("article");
                File.WriteAllText(jsFilePath, jsContent);
            }
        }
    }
}