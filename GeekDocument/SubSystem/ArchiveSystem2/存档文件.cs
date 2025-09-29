namespace GeekDocument.SubSystem.ArchiveSystem2
{
    public class 资源文件
    {
        public string 哈希值 { get; set; } = "";

        public string 类型 { get; set; } = "";

        public byte[] 数据 { get; set; } = Array.Empty<byte>();
    }

    public class 存档文件
    {
        public string 版本 { get; set; } = "1.0";

        public 文档数据 文档数据 { get; set; } = new 文档数据();

        public List<资源文件> 资源列表 { get; set; } = new List<资源文件>();
    }
}