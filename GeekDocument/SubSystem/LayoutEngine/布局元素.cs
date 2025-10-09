using GeekDocument.SubSystem.LayoutEngine.Element;
using System.Windows;
using System.Windows.Media;

namespace GeekDocument.SubSystem.LayoutEngine;

public class MouseMoveArgs
{
    public Point Point { get; set; }

    public bool Handled { get; set; } = false;
}

public abstract class 布局元素 : IDocElement
{
    public virtual string Name { get; set; } = "未命名布局元素";

    public virtual string Icon { get; set; } = "Element";

    public 布局元素? Parent { get; set; } = null;

    public List<布局元素> Children { get; set; } = new List<布局元素>();

    public virtual List<IDocElement> ChildrenElement => Children.Cast<IDocElement>().ToList();

    public Action<IDocElement>? ChildrenChanged { get; set; } = null;

    public Action<IDocElement>? Removed { get; set; } = null;

    public bool MouseHover
    {
        get => _mouseHover;
        set
        {
            if (_mouseHover == value) return;
            _mouseHover = value;
            if (_mouseHover) MouseEnter();
            else MouseLeave();
        }
    }

    private bool _mouseHover = false;

    public void AddChild(布局元素 child)
    {
        child.Parent = this;
        Children.Add(child);
        更新绘图对象("添加子元素");
        ChildrenChanged?.Invoke(this);
    }

    public void AddChildList(List<布局元素> childList)
    {
        foreach (var child in childList) child.Parent = this;
        Children.AddRange(childList);
        更新绘图对象("添加子元素列表");
        ChildrenChanged?.Invoke(this);
    }

    public void RemoveChild(布局元素 child)
    {
        child.Parent = null;
        Children.Remove(child);
        更新绘图对象("移除子元素");
        ChildrenChanged?.Invoke(this);
        child.Removed?.Invoke(child);
    }

    public void ClearChildren()
    {
        foreach (var child in Children)
        {
            child.Parent = null;
            child.Removed?.Invoke(child);
        }
        Children.Clear();
        更新绘图对象("清空子元素");
        ChildrenChanged?.Invoke(this);
    }

    public string GetPath()
    {
        if (Parent == null) return Name;
        return Parent.GetPath() + " > " + Name;
    }

    public List<布局元素> GetAllParent()
    {
        List<布局元素> parentList = new List<布局元素>();
        if (Parent == null) return parentList;
        parentList.AddRange(Parent.GetAllParent());
        parentList.Add(Parent);
        return parentList;
    }

    public virtual Rect GetViewRect() => Rect.Empty;

    public virtual Rect GetHitTestRect() => GetViewRect();

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

    public virtual void MouseEnter()
    {
        // Console.WriteLine($"鼠标进入: {GetPath()}");
    }

    public virtual void MouseMove(MouseMoveArgs args) { }

    public virtual void MouseLeave()
    {
        // Console.WriteLine($"鼠标离开: {GetPath()}");
    }

    public virtual void 命中测试(Point point)
    {
        // 先检测自己是否命中
        Rect rect = GetHitTestRect();
        if (rect.Contains(point))
        {
            // 获取直接命中元素
            布局元素? 直接命中 = HitManager.Instance.直接命中元素;
            // 进入这个分支，说明命中了自己或子元素，所以直接命中元素不应为空
            if (直接命中 == null) throw new Exception("命中测试时，直接命中元素不应为空");
            // 直接命中自己，则设置为悬停状态
            if (直接命中 == this) MouseHover = true;
            else
            {
                // 获取命中元素的全部父元素
                List<布局元素> parentList = 直接命中.GetAllParent();
                // 如果命中元素为自己的子元素，则设置为悬停状态
                if (parentList.Contains(this)) MouseHover = true;
                else MouseHover = false;
            }
        }
        else MouseHover = false;
        // 再处理子元素
        for (int index = Children.Count - 1; index >= 0; index--)
        {
            布局元素? item = Children[index];
            item.命中测试(point);
        }
    }

    public virtual void 移动测试(Point point)
    {
        if (_mouseHover) MouseMove(new MouseMoveArgs() { Point = point });
        foreach (var item in Children) item.移动测试(point);
    }

    public virtual 命中信息? 获取命中信息(Point point) => null;

    public virtual 元素行 获取最近元素行(Point point) => throw new Exception("当前元素不支持获取最近元素行");

    public virtual void 从开头移出光标(布局元素 元素) { }

    public virtual void 从末尾移出光标(布局元素 元素) { }
}