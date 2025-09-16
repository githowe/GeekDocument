using System.Windows;
using XLogic.Wpf.Drawing;

namespace GeekDocument.SubSystem.LayoutEngine;

public class 块元素 : SingleBoard
{
    #region 属性

    public Thickness BlockMargin { get; set; } = new Thickness(0);

    public 布局元素? 根元素 { get; set; } = null;

    #endregion

    #region 动态属性

    /// <summary>块横坐标。由页面控件设置</summary>
    public double BlockLeft { get; set; } = 0;

    /// <summary>块纵坐标。由页面控件设置</summary>
    public double BlockTop { get; set; } = 0;

    /// <summary>块宽度。由页面控件设置</summary>
    public double BlockWidth { get; set; } = 0;

    /// <summary>块高度。根据内部元素布局设置</summary>
    public double BlockHeight { get; private set; } = 0;

    /// <summary>空白区域列表。用于处理文字环绕</summary>
    public List<Rect> SpaceRectList { get; set; } = new List<Rect>();

    #endregion

    /// <summary>
    /// 更新元素布局
    /// </summary>
    public void UpdateElementLayout()
    {
        if (根元素 == null) throw new Exception("更新布局失败，根元素为空");
        // 元素最大宽度 = 块宽度 - 块左边距 - 块右边距
        double maxWidth = BlockWidth - BlockMargin.Left - BlockMargin.Right;
        // 设置元素最大宽度并计算元素大小
        if (double.IsNaN(根元素.MaxWidth)) 根元素.MaxWidth = maxWidth;
        if (根元素.MaxWidth > maxWidth) 根元素.MaxWidth = maxWidth;
        根元素.Measure();
        // 根据元素实际大小与对齐方式设置元素坐标
        根元素.Left = BlockMargin.Left;
        if (根元素.ActualWidth < maxWidth)
        {
            switch (根元素.水平对齐)
            {
                case 水平对齐方式.Left:
                    根元素.Left = BlockMargin.Left;
                    break;
                case 水平对齐方式.Center:
                    根元素.Left = BlockMargin.Left + (maxWidth - 根元素.ActualWidth) / 2;
                    break;
                case 水平对齐方式.Right:
                    根元素.Left = BlockMargin.Left + (maxWidth - 根元素.ActualWidth);
                    break;
                case 水平对齐方式.Justify:
                    根元素.Left = BlockMargin.Left;
                    break;
            }
        }
        根元素.Top = 0;
        // 排列元素
        根元素.Arrange();
        // 高度 = 根元素高度 + 上边距 + 下边距
        BlockHeight = 根元素.ActualHeight + BlockMargin.Top + BlockMargin.Bottom;
    }

    protected override void OnUpdate() => 根元素?.绘图(_dc!);
}