using GeekDocument.SubSystem.ExportSystem.HtmlTool;
using GeekDocument.SubSystem.ImageSystem;
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
            generater.AddEndParagraph();
            // 生成行列表，写入文件
            List<string> lines = generater.GenerateLineList();
            string text = string.Join("\n", lines);
            File.WriteAllText(Path.Combine(path, name + ".html"), text);
            // 复制资源文件
            FileResManager.Instance.CopyFolder("HtmlRes", path);
            // 生成图片文件
            foreach (var imageHash in ExportImageManager.Instance.ImageHashList)
            {
                // 查找图片数据
                ImageFileData? imageData = ImageManager.Instance.FindFileData(imageHash);
                if (imageData == null) continue;
                // 写入图片文件
                string imageFilePath = Path.Combine(path, imageHash + "." + imageData.Type);
                if (File.Exists(imageFilePath)) continue;
                File.WriteAllBytes(imageFilePath, imageData.Data);
            }
            ExportImageManager.Instance.Clear();
        }
    }
}