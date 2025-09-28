using GeekDocument.AppTool.Ex;

namespace GeekDocument.SubSystem.ArchiveSystem2;

public class 边线
{
    public 边线() { }

    public 边线(string data)
    {
        string[] array = data.Split(',');
        Left = double.Parse(array[0]);
        Top = double.Parse(array[1]);
        Right = double.Parse(array[2]);
        Bottom = double.Parse(array[3]);
    }

    public double Left { get; set; } = 0;

    public double Top { get; set; } = 0;

    public double Right { get; set; } = 0;

    public double Bottom { get; set; } = 0;

    public override string ToString() => $"{Left},{Top},{Right},{Bottom}";
}

public class 文档数据
{
    public string 作者 { get; set; } = "";

    public string 简介 { get; set; } = "";

    public DateTime 创建日期 { get; set; } = DateTime.Now;

    public string 备注 { get; set; } = "";

    public List<string> 标签列表 { get; set; } = new List<string>();

    public 页面数据 页面 { get; set; } = new 页面数据();
}

public class 页面数据
{
    public double 页面宽度 { get; set; } = 0;

    public 边线 内边距 { get; set; } = new 边线();

    public double 首行缩进 { get; set; } = 0;

    public double 段落间距 { get; set; } = 0;

    public List<元素信息> 元素列表 { get; set; } = new List<元素信息>();
}

public class 元素信息
{
    public string Type { get; set; } = "";

    public string Version { get; set; } = "1.0";

    public string Data { get; set; } = "";
}

public class 文本样式
{
    public string 名称 { get; set; } = "";

    public string 值 { get; set; } = "";

    public int 起始索引 { get; set; } = 0;

    public int 结束索引 { get; set; } = 0;
}

public class 段落元素属性
{
    public string 文本 { get; set; } = "";

    public int 水平对齐方式 { get; set; } = 0;

    public int 垂直对齐方式 { get; set; } = 0;

    public double 段前距 { get; set; } = 0;

    public double 段后距 { get; set; } = 0;

    public double 左缩进 { get; set; } = 0;

    public double 右缩进 { get; set; } = 0;

    public double 首行缩进 { get; set; } = 0;

    public double 行间距 { get; set; } = 0;

    public override string ToString()
    {
        List<string> list = new List<string>
        {
            文本,
            水平对齐方式.ToString(),
            垂直对齐方式.ToString(),
            段前距.ToString(),
            段后距.ToString(),
            左缩进.ToString(),
            右缩进.ToString(),
            首行缩进.ToString(),
            行间距.ToString(),
        };
        return list.序列化并压缩();
    }
}

public class 段落元素
{
    public string 属性 { get; set; } = "";

    public List<元素信息> 内嵌元素列表 { get; set; } = new List<元素信息>();

    public List<文本样式> 样式列表 { get; set; } = new List<文本样式>();
}

public class 图片元素属性
{
    public string 图片源 { get; set; } = "";

    public int 宽度 { get; set; } = -1;

    public int 高度 { get; set; } = -1;

    public bool 像素画 { get; set; } = false;

    public int 图注宽度模式 { get; set; } = 1;

    public double 图注最大宽度 { get; set; } = double.NaN;

    public double 图注固定宽度 { get; set; } = double.NaN;

    public double 图注顶边距 { get; set; } = 4;

    public override string ToString()
    {
        List<string> list = new List<string>
        {
            图片源,
            宽度.ToString(),
            高度.ToString(),
            像素画.ToString(),
            图注宽度模式.ToString(),
            图注最大宽度.ToString(),
            图注固定宽度.ToString(),
            图注顶边距.ToString(),
        };
        return list.序列化并压缩();
    }
}

public class 图片元素
{
    public string 属性 { get; set; } = "";

    public 元素信息? 图注信息 { get; set; } = null;
}

public class 表格元素属性
{
    public int 行数 { get; set; } = 0;

    public int 列数 { get; set; } = 0;

    public double 边框粗细 { get; set; } = 1;

    public override string ToString()
    {
        List<string> list = new List<string>
        {
            行数.ToString(),
            列数.ToString(),
            边框粗细.ToString(),
        };
        return list.序列化并压缩();
    }
}

public class 表格元素
{
    public string 属性 { get; set; } = "";

    public List<元素信息> 单元格列表 { get; set; } = new List<元素信息>();
}

public class 单元格元素属性
{
    public int 行号 { get; set; } = 0;

    public int 列号 { get; set; } = 0;

    public double 宽度 { get; set; } = double.NaN;

    public double 最小高度 { get; set; } = 0;

    public 边线 内边距 { get; set; } = new 边线();

    public int 水平对齐方式 { get; set; } = 0;

    public int 垂直对齐方式 { get; set; } = 0;

    public double 段间距 { get; set; } = 0;

    public override string ToString()
    {
        List<string> list = new List<string>
        {
            行号.ToString(),
            列号.ToString(),
            宽度.ToString(),
            最小高度.ToString(),
            内边距.ToString(),
            水平对齐方式.ToString(),
            垂直对齐方式.ToString(),
            段间距.ToString(),
        };
        return list.序列化并压缩();
    }
}

public class 单元格元素
{
    public string 属性 { get; set; } = "";

    public List<元素信息> 段落列表 { get; set; } = new List<元素信息>();
}