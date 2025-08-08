namespace GeekDocument.SubSystem.OptionSystem
{
    /// <summary>
    /// 文档库
    /// </summary>
    public class DocumentLib
    {
        public string Name { get; set; } = "";

        public string Path { get; set; } = "";
    }

    /// <summary>
    /// 系统选项
    /// </summary>
    public class SystemOption
    {
        /// <summary>库列表</summary>
        public List<DocumentLib> LibList { get; set; } = new List<DocumentLib>();

        /// <summary>默认路径索引</summary>
        public int DefaultPathIndex { get; set; } = 0;

        public string DefaultPath => LibList[DefaultPathIndex].Path;
    }
}