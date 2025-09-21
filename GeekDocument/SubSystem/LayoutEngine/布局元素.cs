using GeekDocument.SubSystem.LayoutEngine.Element;
using GeekDocument.SubSystem.LayoutEngine.Ex;
using GeekDocument.SubSystem.LayoutEngine.Tool;
using System.Windows;
using System.Windows.Media;

namespace GeekDocument.SubSystem.LayoutEngine;

public abstract class 布局元素 : IDocumentElement
{
    #region IDocumentElement 成员

    public IDocumentElement? ParentElement { get; set; } = null;

    public virtual string Icon { get; set; } = "Element";

    public virtual string Name { get; set; } = "未命名布局元素";

    public virtual bool CanInput => false;

    public virtual List<IDocumentElement> GetSubElementList() => new List<IDocumentElement>();

    public virtual Rect GetViewRect()
    {
        段落 root = this.GetRootParagraph();
        double top = root.段落偏移 + Top;
        return new Rect(Left, top, ActualWidth, ActualHeight);
    }

    public virtual Rect GetHitTestRect() => GetViewRect();

    public virtual IDocumentElement? GetHitedElement(Point point)
    {
        // 获取子元素列表
        List<IDocumentElement> subList = GetSubElementList();
        // 反向遍历子元素
        for (int index = subList.Count - 1; index >= 0; index--)
        {
            IDocumentElement item = subList[index];
            IDocumentElement? hited = item.GetHitedElement(point);
            if (hited != null) return hited;
            if (item.GetHitTestRect().Contains(point)) return item;
        }
        // 没有任何子元素命中时，检测自己是否命中
        if (GetHitTestRect().Contains(point)) return this;
        return null;
    }

    public virtual IDocumentElement GetNearestElement(Point point)
    {
        return this;
    }

    public virtual void HandleMouseDown(Point point) { }

    public virtual CaretInfo MoveCaret(Point point)
    {
        return new CaretInfo();
    }

    public virtual 元素行 GetHitedLine(Point point)
    {
        throw new Exception("布局元素不支持获取命中行");
    }

    public virtual void MoveLeftCaret() { }

    public virtual void MoveInCaretToEnd() { }

    public virtual void MoveOutCaretFromHead(IDocumentElement sender) { }

    #endregion

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
    /// 获取元素图层列表
    /// </summary>
    public virtual List<ElementLayer> GetLayerList() { return new List<ElementLayer>(); }

    /// <summary>
    /// 计算元素大小
    /// </summary>
    public virtual void Measure() { }

    /// <summary>
    /// 排列元素
    /// </summary>
    public virtual void Arrange() { }

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