using GeekDocument.SubSystem.ExportSystem.BdocTool;
using GeekDocument.SubSystem.ExportSystem.HtmlTool;
using GeekDocument.SubSystem.ImageSystem;
using GeekDocument.SubSystem.LayoutEngine;
using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace GeekDocument.SubSystem.ExportSystem.Exporter
{
    public class Bdoc
    {
        public string Summary { get; set; } = "";

        public string Html { get; set; } = "";
    }

    public class BdocExporter : IExporter
    {
        public void Export(页面 page, string path, string name)
        {
            // 创建博客文档
            Bdoc bdoc = new Bdoc();
            // 生成摘要
            bdoc.Summary = WebUtility.HtmlEncode(生成摘要(page));
            // 创建生成器
            BdocGenerater generater = new BdocGenerater();
            // 添加段落
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
            // 获取日期
            DateTime dateTime = DateTime.Now;
            // string baseLink = "http://localhost:5285/";
            string baseLink = "http://www.gjiang.club/";
            string imageUrl = $"{baseLink}index?Action=GetImage&Name={dateTime.Year:0000}/{dateTime.Month:00}/";
            ExportImageManager.Instance.ImageUrl = imageUrl;
            // 生成行列表
            List<string> lines = generater.GenerateLineList();
            bdoc.Html = string.Join("\n", lines);
            // 生成文件内容
            string jsonData = JsonConvert.SerializeObject(bdoc, Formatting.Indented);
            File.WriteAllText(Path.Combine(path, name + ".bdoc"), jsonData);
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
            ExportImageManager.Instance.ImageUrl = "";
        }

        private string 生成摘要(页面 page)
        {
            List<string> lineList = new List<string>();
            foreach (var item in page.段落列表)
            {
                string itemText = item.获取文本().Replace("\u200b", "");
                if (itemText.Trim() == "") continue;
                lineList.Add(itemText);
            }
            string text = string.Join(" ", lineList);
            if (text.Length > 160) text = text.Substring(0, 160);
            return text;
        }
    }
}