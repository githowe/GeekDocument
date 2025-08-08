namespace GeekDocument.SubSystem.OptionSystem
{
    /// <summary>
    /// 页面选项
    /// </summary>
    public class PageOption
    {
        /// <summary>页面宽度</summary>
        public int PageWidth { get; set; } = 800;

        /// <summary>页边距</summary>
        public PageMargin PageMargin { get; set; } = new PageMargin(32);

        /// <summary>块间距</summary>
        public int BlockInterval { get; set; } = 16;
    }
}