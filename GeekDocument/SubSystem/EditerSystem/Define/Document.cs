using GeekDocument.SubSystem.ArchiveSystem.Define;
using GeekDocument.SubSystem.EditerSystem.Define.BlockDerive;
using GeekDocument.SubSystem.ImageSystem;
using GeekDocument.SubSystem.OptionSystem;
using Newtonsoft.Json;

namespace GeekDocument.SubSystem.EditerSystem.Define;

public class Document
{
    /// <summary>作者</summary>
    public string Author { get; set; } = "";

    /// <summary>简介</summary>
    public string Summary { get; set; } = "";

    /// <summary>创建日期</summary>
    public DateTime Create { get; set; } = DateTime.Now;

    /// <summary>备注</summary>
    public string Note { get; set; } = "";

    /// <summary>标签</summary>
    public List<string> TagList { get; set; } = new List<string>();

    /// <summary>块列表</summary>
    public List<Block> BlockList { get; set; } = new List<Block>();

    /// <summary>页宽度</summary>
    public int PageWidth { get; set; } = 0;

    /// <summary>内边距</summary>
    public PageThickness Padding { get; set; } = new PageThickness();

    /// <summary>首行缩进</summary>
    public int FirstLineIndent { get; set; } = 0;

    /// <summary>段间距</summary>
    public int ParagraphInterval { get; set; } = 0;

    /// <summary>
    /// 加载存档
    /// </summary>
    public void LoadArchive(ArchiveFile archive)
    {
        // 加载元数据
        Author = archive.MetaData.Author;
        Summary = archive.MetaData.Summary;
        Create = DateTime.ParseExact(archive.MetaData.Create, "yyyy.MM.dd", null);
        Note = archive.MetaData.Note;
        TagList = archive.MetaData.Tag.Split(",").ToList();
        for (int index = 0; index < TagList.Count; index++)
            TagList[index] = TagList[index].Trim();
        // 加载页面信息
        PageWidth = int.Parse(archive.PageData.PageWidth);
        Padding = new PageThickness(archive.PageData.Padding);
        FirstLineIndent = archive.PageData.FirstLineIndent;
        ParagraphInterval = archive.PageData.ParagraphInterval;
        // 加载资源数据
        {
            int offset = 0;
            // 遍历资源列表
            foreach (var resInfo in archive.ResList.List)
            {
                byte[] sourceData = new byte[resInfo.ResSize];
                // 复制资源数据
                Array.Copy(archive.ResData, offset, sourceData, 0, resInfo.ResSize);
                offset += resInfo.ResSize;
                // 添加到图片管理器
                ImageManager.Instance.AddFileData(new ImageFileData
                {
                    Hash = resInfo.Hash,
                    Type = resInfo.ResType,
                    Data = sourceData
                });
                // 解码图片
                ImageManager.Instance.DecodeImage(resInfo.Hash);
            }
        }
        // 加载块列表
        LoadBlockList(archive.BlockData.DataList);
    }

    private void LoadBlockList(List<string> blockInfoList)
    {
        foreach (var blockInfoJson in blockInfoList)
        {
            // 解析块信息
            BlockInfo? blockInfo = JsonConvert.DeserializeObject<BlockInfo>(blockInfoJson);
            if (blockInfo == null) continue;
            // 实例化块
            Block? block = null;
            switch (blockInfo.Type)
            {
                case "Text":
                    block = new BlockText
                    {
                        FirstLineIndent = FirstLineIndent,
                    };
                    break;
                case "Image":
                    block = new BlockImage();
                    break;
            }
            if (block == null) continue;
            // 加载块数据
            block.LoadJson(blockInfo.SourceData);
            // 添加块
            BlockList.Add(block);
        }
    }
}