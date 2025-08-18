using GeekDocument.SubSystem.ArchiveSystem.Define;
using GeekDocument.SubSystem.OptionSystem;
using Newtonsoft.Json;

namespace GeekDocument.SubSystem.EditerSystem.Define
{
    /// <summary>
    /// 文档资源
    /// </summary>
    public class DocumentRes
    {
        /// <summary>类型</summary>
        public string ResType { get; set; } = "";

        /// <summary>源数据</summary>
        public byte[] SourceData { get; set; } = Array.Empty<byte>();
    }

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

        public Dictionary<int, DocumentRes> ResourceDict { get; set; } = new Dictionary<int, DocumentRes>();

        /// <summary>已保存</summary>
        public bool Saved { get; set; } = true;

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
            // 加载块列表
            LoadBlockList(archive.BlockData.DataList);
            // 加载页面信息
            PageWidth = int.Parse(archive.PageData.PageWidth);
            Padding = new PageThickness(archive.PageData.Padding);
            // 加载资源数据
            {
                int offset = 0;
                // 遍历资源列表
                foreach (var resInfo in archive.ResList.List)
                {
                    // 构建一个文档资源
                    DocumentRes documentRes = new DocumentRes
                    {
                        ResType = resInfo.ResType,
                        SourceData = new byte[resInfo.ResSize]
                    };
                    // 复制资源数据
                    Array.Copy(archive.ResData, offset, documentRes.SourceData, 0, resInfo.ResSize);
                    offset += resInfo.ResSize;
                    // 添加到资源字典
                    ResourceDict.Add(resInfo.ResID, documentRes);
                }
            }
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
                        block = new BlockText();
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
}