namespace GeekDocument.SubSystem.EditerSystem.Define
{
    /// <summary>
    /// 文档资源
    /// </summary>
    public class DocumentRes
    {
        public DocumentRes() { }

        /// <summary>哈希值</summary>
        public string Hash { get; set; } = "";

        /// <summary>类型</summary>
        public string ResType { get; set; } = "";

        /// <summary>源数据</summary>
        public byte[] SourceData { get; set; } = Array.Empty<byte>();
    }
}