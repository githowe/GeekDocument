using System.Windows;
using System.Windows.Media;

namespace GeekDocument.SubSystem.LayoutEngine.Element;

public class 单元格 : 布局元素, IComparable<单元格>
{
    public 单元格()
    {
        Name = "单元格";
        Icon = "Cell";
    }

    public int 行号 { get; set; } = 0;

    public int 列号 { get; set; } = 0;

    public double Left { get; set; } = double.NaN;

    public double Top { get; set; } = double.NaN;

    public double Width { get; set; } = double.NaN;

    public double MinHeight
    {
        get => _最小高度 + 垂直内边距;
        set => _最小高度 = value - 垂直内边距;
    }

    public double ActualWidth { get; set; } = double.NaN;

    public double ActualHeight
    {
        get
        {
            double 最小高度 = _最小高度 + Padding.Top + Padding.Bottom;
            double 内容高度 = _内容高度 + Padding.Top + Padding.Bottom;
            return Math.Max(Math.Max(最小高度, 内容高度), _外框高度);
        }
    }

    public Thickness Padding { get; set; } = new Thickness(4);

    public 水平对齐方式 水平对齐 { get; set; } = 水平对齐方式.Justify;

    public 垂直对齐方式 垂直对齐 { get; set; } = 垂直对齐方式.Bottom;

    public double 段间距 { get; set; } = 4;

    public double 水平内边距 => Padding.Left + Padding.Right;

    public double 垂直内边距 => Padding.Top + Padding.Bottom;

    public List<段落> 段落列表 { get; set; } = new List<段落>();

    public override Rect GetViewRect() => new Rect(Left, Top, ActualWidth, ActualHeight);

    public override void 测量()
    {
        // 设置段落最大宽度并测量
        foreach (var 段落 in 段落列表)
        {
            段落.MaxWidth = Width - 水平内边距;
            段落.测量();
        }
        // 设置实际宽度
        ActualWidth = Width;
        // 计算内容高度
        double 内容高度 = 0;
        foreach (var 段落 in 段落列表)
        {
            内容高度 += 段落.段前距;
            内容高度 += 段落.ActualHeight;
            内容高度 += 段落.段后距;
        }
        内容高度 += 段间距 * (段落列表.Count - 1);
        _内容高度 = 内容高度;
    }

    public override void 重新测量()
    {
        // 当前尺寸
        double oldWidth = ActualWidth;
        double oldHeight = ActualHeight;
        // 重新测量
        测量();
        // 尺寸没有变化，则只需重新排列自身并渲染即可
        if (ActualWidth == oldWidth && ActualHeight == oldHeight)
        {
            排列();
            渲染(null);
        }
        else Parent?.重新测量();
    }

    public override void 排列()
    {
        double x = Left + Padding.Left;
        double y = Top + Padding.Top;
        foreach (var item in 段落列表)
        {
            item.Left = x;
            y += item.段前距;
            item.Top = y;
            item.排列();
            y += item.段后距 + item.ActualHeight + 段间距;
        }
    }

    public override void 渲染(DrawingContext? dc)
    {
        foreach (var item in 段落列表) item.渲染(dc);
    }

    public override List<绘图对象> 获取绘图对象()
    {
        List<绘图对象> result = new List<绘图对象>();
        foreach (var item in 段落列表)
            result.AddRange(item.获取绘图对象());
        return result;
    }

    public override 元素行 获取最近元素行(Point point)
    {
        段落 段落 = 获取最近段落(point);
        return 段落.获取最近元素行(point);
    }

    public int CompareTo(单元格? other)
    {
        if (other == null) return 1;
        if (行号 != other.行号) return 行号.CompareTo(other.行号);
        return 列号.CompareTo(other.列号);
    }

    public void 添加段落(段落 段落)
    {
        段落列表.Add(段落);
        AddChild(段落);
    }

    public void 同步高度(double height)
    {
        if (ActualHeight < height) _外框高度 = height - 垂直内边距;
    }

    public void 移入光标至开头()
    {
        段落列表[0].移动光标至开头();
    }

    public void 移入光标至末尾()
    {
        段落列表.Last().移动光标至末尾();
    }

    public override void 从开头移出光标(布局元素 元素)
    {
        if (Parent is 表格 表格) 表格.移动光标至上一个单元格(this);
    }

    public override void 从末尾移出光标(布局元素 元素)
    {
        if (Parent is 表格 表格) 表格.移动光标至下一个单元格(this);
    }

    private 段落 获取最近段落(Point point)
    {
        段落? 命中段落 = null;
        // 首先通过纵坐标找到命中段落，当段落重叠时，优先命中最上层的段落
        for (int index = 段落列表.Count - 1; index >= 0; index--)
        {
            段落 段落 = 段落列表[index];
            Rect rect = new Rect(段落.Left, 段落.Top, 段落.ActualWidth, 段落.ActualHeight);
            if (rect.Top <= point.Y && point.Y <= rect.Bottom)
            {
                命中段落 = 段落;
                break;
            }
        }
        // 段落之间有间距时，会无法命中，此时通过最近距离找到命中段落
        if (命中段落 == null)
        {
            double 当前距离 = double.MaxValue;
            命中段落 = 段落列表[0];
            foreach (var 段落 in 段落列表)
            {
                Rect rect = new Rect(段落.Left, 段落.Top, 段落.ActualWidth, 段落.ActualHeight);
                double 距离 = Math.Min(Math.Abs(point.Y - rect.Top), Math.Abs(point.Y - rect.Bottom));
                if (距离 < 当前距离)
                {
                    当前距离 = 距离;
                    命中段落 = 段落;
                }
            }
        }
        return 命中段落;
    }

    private double _最小高度 = 0;
    private double _内容高度 = 0;
    private double _外框高度 = 0;
}