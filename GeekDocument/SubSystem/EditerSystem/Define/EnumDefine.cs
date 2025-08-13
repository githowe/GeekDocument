namespace GeekDocument.SubSystem.EditerSystem.Define
{
    /// <summary>
    /// 块类型
    /// </summary>
    public enum BlockType
    {
        /// <summary>文本</summary>
        Text,
        /// <summary>分割线</summary>
        SplitLine,
        /// <summary>代码</summary>
        Code,
        /// <summary>列表</summary>
        List,
        /// <summary>图片</summary>
        Image,
        /// <summary>表格</summary>
        Table,
        /// <summary>公式</summary>
        Formula,
        /// <summary>图表</summary>
        Chart,
        /// <summary>三维模型</summary>
        Model,
        /// <summary>音频</summary>
        Audio,
    }

    /// <summary>
    /// 文本类型
    /// </summary>
    public enum TextType
    {
        /// <summary>正文</summary>
        Text,
        /// <summary>标头</summary>
        Header_04,
        Header_03,
        Header_02,
        Header_01,
        /// <summary>标题</summary>
        Title
    }

    /// <summary>
    /// 文本样式
    /// </summary>
    public enum TextStyle
    {
        Normal = 0,
        Bold = 1,
        Italic = 2,
        BoldItalic = 3,
    }

    /// <summary>
    /// 行对齐方式
    /// </summary>
    public enum LineAlignType
    {
        /// <summary>左对齐</summary>
        Left,
        /// <summary>居中对齐</summary>
        Center,
        /// <summary>右对齐</summary>
        Right,
        /// <summary>两端对齐</summary>
        Justify
    }

    /// <summary>
    /// 编辑键
    /// </summary>
    public enum EditKey
    {
        Up,
        Down,
        Left,
        Right,

        Home,
        End,

        Backspace,
        Delete,
        Enter,
    }
}