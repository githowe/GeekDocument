using System.Windows;

namespace GeekDocument.SubSystem.LayoutEngine;

public class 浮动元素
{
    public double Left { get; set; } = 0;

    public double Top { get; set; } = 0;

    public double Width { get; set; } = 0;

    public double Height { get; private set; } = 0;

    public Thickness Margin { get; set; } = new Thickness(0);

    public 布局元素? 根元素 { get; set; } = null;

    /// <summary>
    /// 更新布局
    /// </summary>
    public void UpdateLayout()
    {
        if (根元素 == null) throw new Exception("更新布局失败，根元素为空");
        // 最大宽度 = 宽度 - 左边距 - 右边距
        double maxWidth = Width - Margin.Left - Margin.Right;
        // 更新根元素布局
        根元素.MaxWidth = maxWidth;
        根元素.UpdateLayout();
        // 高度 = 根元素高度 + 上边距 + 下边距
        Height = 根元素.ActualHeight + Margin.Top + Margin.Bottom;
    }
}