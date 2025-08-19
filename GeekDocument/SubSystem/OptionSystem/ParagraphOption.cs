namespace GeekDocument.SubSystem.OptionSystem
{
    /// <summary>
    /// 段落选项
    /// </summary>
    public class ParagraphOption
    {
        /// <summary>首行缩进。单位：像素</summary>
        public int FirstLineIndent { get; set; } = 32;

        /// <summary>块间距</summary>
        public int BlockInterval { get; set; } = 16;
    }
}