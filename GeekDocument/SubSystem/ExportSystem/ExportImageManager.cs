namespace GeekDocument.SubSystem.ExportSystem
{
    /// <summary>
    /// 导出图片管理器。收集导出时所需的图片资源
    /// </summary>
    public class ExportImageManager
    {
        #region 单例

        private ExportImageManager() { }
        public static ExportImageManager Instance { get; } = new ExportImageManager();

        #endregion

        public string ImageUrl { get; set; } = "";

        public List<string> ImageHashList { get; set; } = new List<string>();

        public void Clear()
        {
            ImageHashList.Clear();
        }
    }
}