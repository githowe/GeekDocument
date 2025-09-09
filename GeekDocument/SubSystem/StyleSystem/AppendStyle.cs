using Newtonsoft.Json;

namespace GeekDocument.SubSystem.StyleSystem;

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

    public abstract bool SameAs(AppendStyle other);

    public abstract string ToJson();
}

public class AppendFont : AppendStyle
{
    public AppendFont() => Type = AppendStyleType.Font;

    public string FontFamily { get; set; } = "";

    public override bool SameAs(AppendStyle other)
    {
        if (other.Type != AppendStyleType.Font) return false;
        AppendFont otherStyle = (AppendFont)other;
        return FontFamily == otherStyle.FontFamily;
    }

    public override string ToJson()
    {
        List<string> data = new List<string>
        {
            Type.ToString(),
            FontFamily
        };
        return JsonConvert.SerializeObject(data);
    }
}

public class AppendSize : AppendStyle
{
    public AppendSize() => Type = AppendStyleType.Size;

    public int FontSize { get; set; } = 16;

    public override bool SameAs(AppendStyle other)
    {
        if (other.Type != AppendStyleType.Size) return false;
        AppendSize otherStyle = (AppendSize)other;
        return FontSize == otherStyle.FontSize;
    }

    public override string ToJson()
    {
        List<string> data = new List<string>
        {
            Type.ToString(),
            FontSize.ToString()
        };
        return JsonConvert.SerializeObject(data);
    }
}

public class AppendBold : AppendStyle
{
    public AppendBold() => Type = AppendStyleType.Bold;

    public bool Enable { get; set; } = false;

    public override bool SameAs(AppendStyle other)
    {
        if (other.Type != AppendStyleType.Bold) return false;
        AppendBold otherStyle = (AppendBold)other;
        return Enable == otherStyle.Enable;
    }

    public override string ToJson()
    {
        List<string> data = new List<string>
        {
            Type.ToString(),
            Enable.ToString()
        };
        return JsonConvert.SerializeObject(data);
    }
}

public class AppendItalic : AppendStyle
{
    public AppendItalic() => Type = AppendStyleType.Italic;

    public bool Enable { get; set; } = false;

    public override bool SameAs(AppendStyle other)
    {
        if (other.Type != AppendStyleType.Italic) return false;
        AppendItalic otherStyle = (AppendItalic)other;
        return Enable == otherStyle.Enable;
    }

    public override string ToJson()
    {
        List<string> data = new List<string>
        {
            Type.ToString(),
            Enable.ToString()
        };
        return JsonConvert.SerializeObject(data);
    }
}

public class AppendColor : AppendStyle
{
    public AppendColor() => Type = AppendStyleType.Color;

    public byte R { get; set; } = 0;

    public byte G { get; set; } = 0;

    public byte B { get; set; } = 0;

    public override bool SameAs(AppendStyle other)
    {
        if (other.Type != AppendStyleType.Color) return false;
        AppendColor otherStyle = (AppendColor)other;
        return R == otherStyle.R && G == otherStyle.G && B == otherStyle.B;
    }

    public override string ToJson()
    {
        List<string> data = new List<string>
        {
            Type.ToString(),
            $"{R:X2}{G:X2}{B:X2}"
        };
        return JsonConvert.SerializeObject(data);
    }
}

public class AppendLink : AppendStyle
{
    public AppendLink() => Type = AppendStyleType.Link;

    public string Url { get; set; } = "";

    public override bool SameAs(AppendStyle other)
    {
        if (other.Type != AppendStyleType.Link) return false;
        AppendLink otherStyle = (AppendLink)other;
        return Url == otherStyle.Url;
    }

    public override string ToJson()
    {
        List<string> data = new List<string>
        {
            Type.ToString(),
            Url
        };
        return JsonConvert.SerializeObject(data);
    }
}