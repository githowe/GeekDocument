using System.Windows;
using System.Windows.Media;

namespace GeekDocument.SubSystem.LayoutEngine;

public abstract class 布局元素
{
    public virtual string Name { get; set; } = "未命名布局元素";

    public virtual string Icon { get; set; } = "Element";

    public 布局元素? Parent { get; set; } = null;

    public List<布局元素> Children { get; set; } = new List<布局元素>();

    public virtual void AddChild(布局元素 child)
    {
        child.Parent = this;
        Children.Add(child);
        更新绘图对象("添加子元素");
    }

    public virtual void AddChildList(List<布局元素> childList)
    {
        foreach (var child in childList) child.Parent = this;
        Children.AddRange(childList);
        更新绘图对象("添加子元素列表");
    }

    public virtual void RemoveChild(布局元素 child)
    {
        child.Parent = null;
        Children.Remove(child);
        更新绘图对象("移除子元素");
    }

    public virtual void ClearChildren()
    {
        foreach (var child in Children) child.Parent = null;
        Children.Clear();
        更新绘图对象("清空子元素");
    }

    public string GetPath()
    {
        if (Parent == null) return Name;
        return Parent.GetPath() + " > " + Name;
    }

    public virtual Rect GetViewRect() => Rect.Empty;

    public virtual void 更新绘图对象(string reason) => Parent?.更新绘图对象(reason);

    public virtual List<绘图对象> 获取绘图对象() => new List<绘图对象>();

    public virtual void Init() { }

    public virtual void 测量() { }

    /// <summary>
    /// 重新测量
    ///     子元素尺寸变更时通知父元素重新测量，父元素直接根据子元素当前尺寸计算父元素尺寸
    /// </summary>
    public virtual void 重新测量() { }

    public virtual void 排列() { }

    public virtual void 渲染(DrawingContext? dc)
    {
        foreach (var item in Children) item.渲染(dc);
    }

    public virtual 命中信息? 获取命中信息(Point point) => null;

    public virtual 元素行 获取最近元素行(Point point) => throw new Exception("当前元素不支持获取最近元素行");

    public virtual void 从开头移出光标(布局元素 元素) { }

    public virtual void 从末尾移出光标(布局元素 元素) { }
}