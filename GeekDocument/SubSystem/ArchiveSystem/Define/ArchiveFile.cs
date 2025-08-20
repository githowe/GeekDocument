using Newtonsoft.Json;
using System.Text;

namespace GeekDocument.SubSystem.ArchiveSystem.Define
{
    /// <summary>
    /// 文件头
    /// </summary>
    public class FileHead
    {
        /// <summary>元数据大小</summary>
        public int MetaDataSize { get; set; } = 0;

        /// <summary>块数据大小</summary>
        public int BlockDataSize { get; set; } = 0;

        /// <summary>页面数据大小</summary>
        public int PageDataSize { get; set; } = 0;

        /// <summary>资源表大小</summary>
        public int ResDictSize { get; set; } = 0;

        /// <summary>资源数据大小</summary>
        public int ResDataSize { get; set; } = 0;

        /// <summary>
        /// 加载字节数组
        /// </summary>
        public void LoadByteArray(byte[] byteData)
        {
            if (byteData.Length < 20) throw new Exception("无效文件头");
            MetaDataSize = BitConverter.ToInt32(byteData, 0);
            BlockDataSize = BitConverter.ToInt32(byteData, 4);
            PageDataSize = BitConverter.ToInt32(byteData, 8);
            ResDictSize = BitConverter.ToInt32(byteData, 12);
            ResDataSize = BitConverter.ToInt32(byteData, 16);
        }

        /// <summary>
        /// 转字节数组
        /// </summary>
        public byte[] ToByteArray()
        {
            byte[] data = new byte[20];
            BitConverter.GetBytes(MetaDataSize).CopyTo(data, 0);
            BitConverter.GetBytes(BlockDataSize).CopyTo(data, 4);
            BitConverter.GetBytes(PageDataSize).CopyTo(data, 8);
            BitConverter.GetBytes(ResDictSize).CopyTo(data, 12);
            BitConverter.GetBytes(ResDataSize).CopyTo(data, 16);
            return data;
        }
    }

    /// <summary>
    /// 元数据
    /// </summary>
    public class MetaData
    {
        public string Name { get; set; } = "";

        public string Data { get; set; } = "";
    }

    /// <summary>
    /// 文档元数据
    /// </summary>
    public class DocMetaData
    {
        /// <summary>作者</summary>
        public string Author { get; set; } = "";

        /// <summary>简介</summary>
        public string Summary { get; set; } = "";

        /// <summary>创建日期</summary>
        public string Create { get; set; } = "";

        /// <summary>备注</summary>
        public string Note { get; set; } = "";

        /// <summary>标签</summary>
        public string Tag { get; set; } = "";

        public void LoadMetaData(byte[] byteData)
        {
            string jsonData = Encoding.UTF8.GetString(byteData);
            List<MetaData>? metaDataList = JsonConvert.DeserializeObject<List<MetaData>>(jsonData);
            if (metaDataList == null) throw new Exception("无效的元数据");
            Author = metaDataList[0].Data;
            Summary = metaDataList[1].Data;
            Create = metaDataList[2].Data;
            Note = metaDataList[3].Data;
            Tag = metaDataList[4].Data;
        }

        public byte[] ToByteArray()
        {
            List<MetaData> metaDataList = new List<MetaData>
            {
                new MetaData { Name = "Author", Data = Author },
                new MetaData { Name = "Summary", Data = Summary },
                new MetaData { Name = "Create", Data = Create },
                new MetaData { Name = "Note", Data = Note },
                new MetaData { Name = "Tag", Data = Tag }
            };
            string jsonData = JsonConvert.SerializeObject(metaDataList);
            return Encoding.UTF8.GetBytes(jsonData);
        }
    }

    public class BlockInfo
    {
        public string Type { get; set; } = "";

        public string SourceData { get; set; } = "";
    }

    public class BlockData
    {
        public List<string> DataList { get; set; } = new List<string>();

        public void LoadBlockData(byte[] byteData)
        {
            string jsonData = Encoding.UTF8.GetString(byteData);
            List<string>? listData = JsonConvert.DeserializeObject<List<string>>(jsonData);
            if (listData == null) throw new Exception("无效的块数据");
            DataList = listData;
        }

        public byte[] ToByteArray()
        {
            string jsonData = JsonConvert.SerializeObject(DataList);
            return Encoding.UTF8.GetBytes(jsonData);
        }
    }

    public class PageData
    {
        public string PageWidth { get; set; } = "0";

        public string Padding { get; set; } = "0";

        public int FirstLineIndent { get; set; } = 0;

        public int ParagraphInterval { get; set; } = 0;

        public void LoadPageData(byte[] byteData)
        {
            string jsonData = Encoding.UTF8.GetString(byteData);
            List<string>? listData = JsonConvert.DeserializeObject<List<string>>(jsonData);
            if (listData == null || listData.Count < 4) throw new Exception("无效的页面数据");
            PageWidth = listData[0];
            Padding = listData[1];
            FirstLineIndent = int.Parse(listData[2]);
            ParagraphInterval = int.Parse(listData[3]);
        }

        public byte[] ToByteArray()
        {
            List<string> listData = new List<string>
            {
                PageWidth,
                Padding,
                FirstLineIndent.ToString(),
                ParagraphInterval.ToString()
            };
            string jsonData = JsonConvert.SerializeObject(listData);
            return Encoding.UTF8.GetBytes(jsonData);
        }
    }

    /// <summary>
    /// 资源信息
    /// </summary>
    public class ResInfo
    {
        /// <summary>编号</summary>
        public int ResID { get; set; } = -1;

        /// <summary>类型</summary>
        public string ResType { get; set; } = "";

        /// <summary>大小</summary>
        public int ResSize { get; set; } = 0;
    }

    /// <summary>
    /// 资源列表
    /// </summary>
    public class ResList
    {
        public List<ResInfo> List { get; set; } = new List<ResInfo>();

        public void LoadResDict(byte[] byteData)
        {
            string jsonData = Encoding.UTF8.GetString(byteData);
            List<ResInfo>? resList = JsonConvert.DeserializeObject<List<ResInfo>>(jsonData);
            if (resList == null) throw new Exception("无效的资源表");
            List = resList;
        }

        public byte[] ToByteArray()
        {
            string jsonData = JsonConvert.SerializeObject(List);
            return Encoding.UTF8.GetBytes(jsonData);
        }
    }

    /// <summary>
    /// 存档文件
    /// </summary>
    public class ArchiveFile
    {
        public string Version { get; set; } = "1.0";

        public FileHead Head { get; set; } = new FileHead();

        public DocMetaData MetaData { get; set; } = new DocMetaData();

        public BlockData BlockData { get; set; } = new BlockData();

        public PageData PageData { get; set; } = new PageData();

        public ResList ResList { get; set; } = new ResList();

        public byte[] ResData { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// 加载存档
        /// </summary>
        public void LoadArchive(byte[] byteData)
        {
            int offset = 0;
            // 版本号
            byte[] versionByteData = new byte[16];
            Array.Copy(byteData, offset, versionByteData, 0, versionByteData.Length);
            Version = Encoding.ASCII.GetString(versionByteData).TrimEnd('\0');
            offset += versionByteData.Length;
            // 文件头
            byte[] headByteData = new byte[20];
            Array.Copy(byteData, offset, headByteData, 0, headByteData.Length);
            Head.LoadByteArray(headByteData);
            offset += headByteData.Length;
            // 元数据
            byte[] metaDataByteData = new byte[Head.MetaDataSize];
            Array.Copy(byteData, offset, metaDataByteData, 0, Head.MetaDataSize);
            MetaData.LoadMetaData(metaDataByteData);
            offset += Head.MetaDataSize;
            // 块数据
            byte[] blockDataByteData = new byte[Head.BlockDataSize];
            Array.Copy(byteData, offset, blockDataByteData, 0, Head.BlockDataSize);
            BlockData.LoadBlockData(blockDataByteData);
            offset += Head.BlockDataSize;
            // 页面数据
            byte[] pageDataByteData = new byte[Head.PageDataSize];
            Array.Copy(byteData, offset, pageDataByteData, 0, Head.PageDataSize);
            PageData.LoadPageData(pageDataByteData);
            offset += Head.PageDataSize;
            // 资源表
            byte[] resDictByteData = new byte[Head.ResDictSize];
            Array.Copy(byteData, offset, resDictByteData, 0, Head.ResDictSize);
            ResList.LoadResDict(resDictByteData);
            offset += Head.ResDictSize;
            // 资源数据
            ResData = new byte[Head.ResDataSize];
            Array.Copy(byteData, offset, ResData, 0, Head.ResDataSize);
        }

        public byte[] ToByteData()
        {
            int totalSize = 0;

            // 版本号
            byte[] versionByteData = new byte[16];
            byte[] asciiData = Encoding.ASCII.GetBytes(Version);
            Array.Copy(asciiData, versionByteData, asciiData.Length);
            totalSize += versionByteData.Length;
            // 元数据
            byte[] metaDataByteData = MetaData.ToByteArray();
            totalSize += metaDataByteData.Length;
            Head.MetaDataSize = metaDataByteData.Length;
            // 块数据
            byte[] blockDataByteData = BlockData.ToByteArray();
            totalSize += blockDataByteData.Length;
            Head.BlockDataSize = blockDataByteData.Length;
            // 页面数据
            byte[] pageDataByteData = PageData.ToByteArray();
            totalSize += pageDataByteData.Length;
            Head.PageDataSize = pageDataByteData.Length;
            // 资源表
            byte[] resDictByteData = ResList.ToByteArray();
            totalSize += resDictByteData.Length;
            Head.ResDictSize = resDictByteData.Length;
            // 资源数据
            totalSize += ResData.Length;
            Head.ResDataSize = ResData.Length;
            // 文件头
            byte[] headByteData = Head.ToByteArray();
            totalSize += headByteData.Length;

            byte[] archiveByteData = new byte[totalSize];
            int offset = 0;
            // 版本号
            Array.Copy(versionByteData, 0, archiveByteData, offset, versionByteData.Length);
            offset += versionByteData.Length;
            // 文件头
            Array.Copy(headByteData, 0, archiveByteData, offset, headByteData.Length);
            offset += headByteData.Length;
            // 元数据
            Array.Copy(metaDataByteData, 0, archiveByteData, offset, metaDataByteData.Length);
            offset += metaDataByteData.Length;
            // 块数据
            Array.Copy(blockDataByteData, 0, archiveByteData, offset, blockDataByteData.Length);
            offset += blockDataByteData.Length;
            // 页面数据
            Array.Copy(pageDataByteData, 0, archiveByteData, offset, pageDataByteData.Length);
            offset += pageDataByteData.Length;
            // 资源表
            Array.Copy(resDictByteData, 0, archiveByteData, offset, resDictByteData.Length);
            offset += resDictByteData.Length;
            // 资源数据
            Array.Copy(ResData, 0, archiveByteData, offset, ResData.Length);

            return archiveByteData;
        }
    }
}