namespace GeekDocument.SubSystem.ExportSystem
{
    /// <summary>
    /// 导出格式
    /// </summary>
    public enum ExportFormat
    {
        PDF,
        Word,
        Markdown,
        Html,
        Text,
        Image,
        /// <summary>博客文档：简化的html，需要在博客环境中才能正确渲染</summary>
        Bdoc,
    }
}