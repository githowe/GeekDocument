using GeekDocument.SubSystem.ImageSystem;
using GeekDocument.SubSystem.LayoutEngine.Element;

namespace GeekDocument.SubSystem.ExportSystem.HtmlTool
{
    public class 图片节点 : HtmlNode
    {
        public 图片节点()
        {
            Markup = "img";
        }

        public 图片 Element { get; set; } = null!;

        public override void Init()
        {
            ExportImageManager.Instance.ImageHashList.Add(Element.SourceHash);
        }

        public override string ToLine()
        {
            ImageFileData? imageData = ImageManager.Instance.FindFileData(Element.SourceHash);
            if (imageData == null) return "";

            // 生成图片源地址
            string src = $"{Element.SourceHash}.{imageData.Type}";
            // 生成样式
            Style style = new Style();
            style.StyleItemList.Add(new Item_Enum("vertical-align", "middle"));
            if (Element.ImageWidth != -1)
                style.StyleItemList.Add(new Item_Double("width", Element.ImageWidth));
            if (Element.ImageHeight != -1)
                style.StyleItemList.Add(new Item_Double("height", Element.ImageHeight));
            // 返回图片标签
            return $"<img src={src} style=\"{style.ToLine()}\" alt=\"\">";
        }
    }
}