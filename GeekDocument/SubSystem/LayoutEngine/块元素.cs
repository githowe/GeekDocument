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
        // 最大宽度 = 宽度 - 左边距 - 右边距
        double maxWidth = BlockWidth - BlockMargin.Left - BlockMargin.Right;
        // 更新根元素坐标，这个坐标是相对于块的
        根元素.Left = 根元素.LeftMargin;
        根元素.Top = 0;
        // 设置最大宽度并更新布局
        根元素.MaxWidth = maxWidth;
        根元素.UpdateLayout();
        // 高度 = 根元素高度 + 上边距 + 下边距
        BlockHeight = 根元素.ActualHeight + BlockMargin.Top + BlockMargin.Bottom;
    }

    protected override void OnUpdate() => 根元素?.绘图(_dc!);
}