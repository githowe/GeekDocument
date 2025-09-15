namespace GeekDocument.SubSystem.LayoutEngine;

public enum 元素类型
{
    Unknown,

    节,
    段落,
    字,
    图片,
    列表,
    表格,
    分割线,
    代码,
    公式
}

public enum 字类型
{
    Chinese,
    English,
    Space,
}

public enum 水平对齐方式
{
    Left,
    Center,
    Right,
    Justify,
}

public enum 垂直对齐方式
{
    Top,
    Center,
    Bottom,
}