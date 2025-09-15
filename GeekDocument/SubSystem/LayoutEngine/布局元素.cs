using System.Windows.Media;

namespace GeekDocument.SubSystem.LayoutEngine;

public class 布局元素
{
    public 元素类型 类型 { get; set; } = 元素类型.Unknown;

    public 布局元素? Parent { get; set; } = null;

    public double Left { get; set; } = double.NaN;

    public double Top { get; set; } = double.NaN;

    public double MaxWidth { get; set; } = double.NaN;

    public double MaxHeight { get; set; } = double.NaN;

    /// <summary>实际宽度</summary>
    public double ActualWidth { get; protected set; } = double.NaN;

    /// <summary>实际高度</summary>
    public double ActualHeight { get; protected set; } = double.NaN;

    /// <summary>左边距</summary>
    public double LeftMargin { get; set; } = 0;

    /// <summary>右边距</summary>
    public double RightMargin { get; set; } = 0;

    public 水平对齐方式 水平对齐 { get; set; } = 水平对齐方式.Justify;

    public 垂直对齐方式 垂直对齐 { get; set; } = 垂直对齐方式.Bottom;

    /// <summary>空白元素</summary>
    public bool IsSpace { get; protected set; } = false;

    /// <summary>可断开</summary>
    public bool CanBreak { get; protected set; } = false;

    public virtual void Init() { }

    /// <summary>
    /// 更新布局。最大宽度为负数时表示不限制宽度
    /// </summary>
    public virtual void UpdateLayout() { }

    /// <summary>
    /// 压缩左边距，然后返回与实际高度之和。用于添加至元素行时判断能否容纳
    /// 一般添加了左边距的元素需要重写此方法，例如插入至文本块的图片元素
    /// </summary>
    public virtual double 压缩左边距() => ActualWidth;

    /// <summary>
    /// 压缩实际宽度并返回。该方法中不需要考虑左边距和右边距
    /// 实际宽度不允许压缩的不需要重写（例如图片），如果是中文标点这种存在可压缩空间的，根据排版规则决定是否重写
    /// </summary>
    public virtual double 压缩实际宽度() => ActualWidth;

    /// <summary>
    /// 压缩右边距，行中没有元素时，添加元素不需要考虑左边距，此时只需压缩右边距即可
    /// </summary>
    public virtual double 压缩右边距() => ActualWidth;

    /// <summary>
    /// 返回压缩整个元素之后的宽度。在添加至元素行后调用，以最大化可用空间
    /// </summary>
    public virtual double 压缩元素() => ActualWidth;

    /// <summary>
    /// 压缩至指定比例，派生类需要在内部调整压缩后的布局
    /// </summary>
    public virtual void 压缩至(double 比例) { }

    public virtual 布局元素 断开(double 最大宽度) { return this; }

    /// <summary>
    /// 绘图。注意，坐标为相对于绘图上下文的坐标，而元素的Left和Top为相对于父元素的坐标，所以该坐标由调用方计算出绝对坐标后传入
    /// </summary>
    public virtual void 绘图(DrawingContext dc) { }
}