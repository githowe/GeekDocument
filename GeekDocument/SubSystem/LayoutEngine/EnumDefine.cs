namespace GeekDocument.SubSystem.LayoutEngine;

public enum 行状态
{
    空,
    填充空格,
    填充元素,
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

public enum ElementSide
{
    Left,
    Right,
}