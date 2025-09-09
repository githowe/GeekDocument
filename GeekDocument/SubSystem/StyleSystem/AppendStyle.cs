namespace GeekDocument.SubSystem.StyleSystem
{
    public enum AppendStyleType
    {
        None,

        /// <summary>字体</summary>
        Font,
        /// <summary>字号</summary>
        Size,
        /// <summary>加粗</summary>
        Bold,
        /// <summary>斜体</summary>
        Italic,
        /// <summary>颜色</summary>
        Color,
        /// <summary>链接</summary>
        Link,
    }

    /// <summary>
    /// 附加样式。例如字体、字号、加粗、斜体、颜色、链接
    /// </summary>
    public abstract class AppendStyle
    {
        public AppendStyleType Type { get; set; } = AppendStyleType.None;
    }

    public class AppendFont : AppendStyle
    {
        public AppendFont() => Type = AppendStyleType.Font;

        public string FontFamily { get; set; } = "";
    }

    public class AppendSize : AppendStyle
    {
        public AppendSize() => Type = AppendStyleType.Size;

        public int FontSize { get; set; } = 16;
    }

    public class AppendBold : AppendStyle
    {
        public AppendBold() => Type = AppendStyleType.Bold;

        public bool Enable { get; set; } = false;
    }

    public class AppendItalic : AppendStyle
    {
        public AppendItalic() => Type = AppendStyleType.Italic;

        public bool Enable { get; set; } = false;
    }

    public class AppendColor : AppendStyle
    {
        public AppendColor() => Type = AppendStyleType.Color;

        public byte R { get; set; } = 0;

        public byte G { get; set; } = 0;

        public byte B { get; set; } = 0;
    }

    public class AppendLink : AppendStyle
    {
        public AppendLink() => Type = AppendStyleType.Link;

        public string Url { get; set; } = "";
    }
}