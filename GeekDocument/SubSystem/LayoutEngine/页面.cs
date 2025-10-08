using GeekDocument.SubSystem.LayoutEngine.Element;
using System.Windows;

namespace GeekDocument.SubSystem.LayoutEngine;

/// <summary>
/// 页面不包括内外边距，仅表示内容渲染区域
/// </summary>
public class 页面 : IDocElement
{
    #region 构造方法

    public 页面() { }

    #endregion

    #region 属性

    public double 页宽 { get; set; } = 800;

    public Thickness 内边距 { get; set; } = new Thickness(64);

    public double 页高 { get; private set; } = 0;

    public List<段落> 段落列表 { get; set; } = new List<段落>();

    public 绘图图层 Layer => _图层;

    public double 首行缩进 { get; set; } = 32;

    public double 段落间距 { get; set; } = 16;

    public 行内元素? 高亮元素 => _高亮元素;

    #endregion

    #region 事件

    public event Action<double>? 高度变化;

    public event Action<光标信息>? 光标移动;

    public event Action<段落>? 当前段落变化;

    public event Action<元素行>? 当前行变化;

    public event Action<行内元素?>? 高亮元素变化;

    #endregion

    #region IDocElement 接口

    public string Name => "页面";

    public string Icon => "Page";

    public List<IDocElement> ChildrenElement => 段落列表.Cast<IDocElement>().ToList();

    public Action<IDocElement>? ChildrenChanged { get; set; } = null;

    public Action<IDocElement>? Removed { get; set; } = null;

    public Rect GetViewRect() => new Rect(0, 0, 页宽, 页高);

    #endregion

    #region 公开方法

    public void 更新绘图对象(string reason)
    {
        List<绘图对象> 新列表 = new List<绘图对象>();
        foreach (var 段落 in 段落列表)
            新列表.AddRange(段落.获取绘图对象());
        _图层.更新绘图对象列表(新列表);

        Console.WriteLine($"因“{reason}”更新绘图对象。绘图对象数量：{新列表.Count}");
    }

    public void 测量()
    {
        // 测量所有段落
        foreach (var 段落 in 段落列表)
        {
            // 页面需要给段落设置固定宽度
            段落.Width = 页宽;
            段落.测量();
        }
        // 测量页面高度
        页高 = 测量页面高度();
    }

    public void 重新测量(段落 sender)
    {
        // 更新页面高度
        页高 = 测量页面高度();
        高度变化?.Invoke(页高);
        // 从发送段落开始重排
        重新排列(sender);
    }

    public void 排列()
    {
        double y = 0;
        foreach (var 段落 in 段落列表)
        {
            段落.Left = 0;
            y += 段落.段前距;
            段落.Top = y;
            段落.排列();
            y += 段落.段后距 + 段落.ActualHeight + 段落间距;
        }
    }

    public void 重新排列(段落 current)
    {
        // 获取当前段落索引
        int currentIndex = 段落列表.IndexOf(current);
        // 如果从第一个段落开始重排，则直接调用排列与渲染
        if (currentIndex == 0)
        {
            排列();
            渲染();
        }
        // 否则，从当前段落开始重排
        else
        {
            段落 前段落 = 段落列表[currentIndex - 1];
            double y = 前段落.Top + 前段落.ActualHeight + 前段落.段后距 + 段落间距;
            for (int index = currentIndex; index < 段落列表.Count; index++)
            {
                段落 段落 = 段落列表[index];
                y += 段落.段前距;
                段落.Top = y;
                段落.排列();
                段落.渲染(null);
                y += 段落.段后距 + 段落.ActualHeight + 段落间距;
            }
        }
    }

    public void 渲染()
    {
        foreach (var 段落 in 段落列表) 段落.渲染(null);
    }

    public 命中信息? 获取命中信息(Point point)
    {
        命中信息? info = null;
        for (int index = 段落列表.Count - 1; index >= 0; index--)
        {
            段落? 段落 = 段落列表[index];
            info = 段落.获取命中信息(point);
            if (info != null) break;
        }
        if (info != null)
        {
            // Console.WriteLine("命中坐标：" + info.坐标);
            // Console.WriteLine("命中区域：" + info.区域名称);
        }
        return info;
    }

    public 段落 获取最近段落(Point point)
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

    public void 左移光标(段落 段落)
    {
        int index = 段落列表.IndexOf(段落);
        if (index > 0) 段落列表[index - 1].移动光标至末尾();
    }

    public void 右移光标(段落 段落)
    {
        int index = 段落列表.IndexOf(段落);
        if (index < 段落列表.Count - 1) 段落列表[index + 1].移动光标至开头();
    }

    public void 移动光标(double x, double y, double height)
    {
        光标信息 info = new 光标信息
        {
            X = x,
            Y = y,
            Height = height
        };
        光标移动?.Invoke(info);
    }

    public void 更新当前段落(段落 currentParagraph)
    {
        if (_当前段落 != currentParagraph)
        {
            _当前段落 = currentParagraph;
            当前段落变化?.Invoke(currentParagraph);
        }
    }

    public void 更新当前元素行(元素行 currentLine)
    {
        更新当前段落((段落)currentLine.Parent);
        当前行变化?.Invoke(currentLine);
    }

    public void 更新高亮元素(行内元素? 元素)
    {
        _高亮元素 = 元素;
        高亮元素变化?.Invoke(元素);
    }

    public 行内元素? 获取高亮元素() => _高亮元素;

    public void 清除高亮元素()
    {
        _高亮元素 = null;
    }

    public void 合并段落(段落 sender)
    {
        int 段落索引 = 段落列表.IndexOf(sender);
        if (段落索引 == 0) return;

        段落 前段落 = 段落列表[段落索引 - 1];
        // 前段落为空，直接删除前段落
        if (前段落.全部行内元素.Count == 0)
        {
            段落列表.Remove(前段落);
            ChildrenChanged?.Invoke(this);
            // 更新绘图对象
            更新绘图对象("删除空段落");
            // 更新页面高度
            页高 = 测量页面高度();
            高度变化?.Invoke(页高);
            // 从发送段落开始重排
            重新排列(sender);
            // 移动光标段落开头
            sender.移动光标至开头();
        }
        // 与前段落合并
        else
        {
            int 光标索引 = 前段落.全部行内元素.Count;
            // 合并段落元素
            List<行内元素> 元素列表 = 前段落.全部行内元素;
            元素列表.AddRange(sender.全部行内元素);
            前段落.更新文本与内嵌元素(元素列表);
            // 重新初始化前段落
            前段落.Init();
            // 删除当前段落
            段落列表.Remove(sender);
            ChildrenChanged?.Invoke(this);
            // 测量前段落
            前段落.测量();
            // 更新绘图对象
            更新绘图对象("合并段落");
            // 更新页面高度
            页高 = 测量页面高度();
            高度变化?.Invoke(页高);
            // 从前段落开始重排
            重新排列(前段落);
            // 移动光标至前段落合并后的位置
            前段落.移动光标至(光标索引);
        }
    }

    public void 处理回车(段落 sender)
    {
        // 获取当前段落索引
        int 段落索引 = 段落列表.IndexOf(sender);
        // 克隆段落
        段落 新段落 = new 段落
        {
            OwnerPage = this,
            Left = 0,
            Width = 页宽,
            水平对齐 = sender.水平对齐,
            垂直对齐 = sender.垂直对齐,
            左缩进 = sender.左缩进,
            右缩进 = sender.右缩进,
            字体 = sender.字体,
            字号 = sender.字号,
            首行缩进 = sender.首行缩进,
            自定义首行缩进 = sender.自定义首行缩进,
            使用自定义首行缩进 = sender.使用自定义首行缩进,
            行间距 = sender.行间距
        };

        // 光标处于行首时，新建一个段落，然后将当前段落的全部元素移动至新段落
        if (sender.光标索引 == 0)
        {
            // 移动元素
            新段落.文本 = sender.获取文本();
            新段落.内嵌元素列表 = sender.获取内嵌元素();
            sender.文本 = "";
            sender.内嵌元素列表.Clear();
            // 初始化段落，旧段落因为更新了文本与内嵌元素，需要重新初始化
            sender.Init();
            新段落.Init();
            // 插入新段落
            段落列表.Insert(段落索引 + 1, 新段落);
            ChildrenChanged?.Invoke(this);
            // 测量旧段落与新段落
            sender.测量();
            新段落.测量();
            // 更新绘图对象
            更新绘图对象("创建新段落");
            // 更新页面高度
            页高 = 测量页面高度();
            高度变化?.Invoke(页高);
            // 从发送段落开始重排
            重新排列(sender);
        }
        // 光标在行中，拆分当前段落为两个段落
        else if (sender.光标索引 < sender.全部行内元素.Count)
        {
            // 从当前段落分割元素列表
            List<行内元素> 元素列表 = sender.分割元素(sender.光标索引);
            // 加载元素列表至新段落
            新段落.更新文本与内嵌元素(元素列表);
            // 初始化段落，旧段落因为更新了文本与内嵌元素，需要重新初始化
            sender.Init();
            新段落.Init();
            // 插入新段落
            段落列表.Insert(段落索引 + 1, 新段落);
            ChildrenChanged?.Invoke(this);
            // 测量旧段落与新段落
            sender.测量();
            新段落.测量();
            // 更新绘图对象
            更新绘图对象("创建新段落");
            // 更新页面高度
            页高 = 测量页面高度();
            高度变化?.Invoke(页高);
            // 从发送段落开始重排
            重新排列(sender);
        }
        // 光标在行尾，在当前段落后创建一个空段落
        else
        {
            新段落.Init();
            // 插入新段落
            段落列表.Insert(段落索引 + 1, 新段落);
            ChildrenChanged?.Invoke(this);
            新段落.测量();
            // 更新绘图对象
            更新绘图对象("创建新段落");
            // 更新页面高度
            页高 = 测量页面高度();
            高度变化?.Invoke(页高);
            // 从新段落开始重排
            重新排列(新段落);
        }

        // 移动光标至新段落开头
        新段落.移动光标至开头();
    }

    #endregion

    #region 私有方法

    private double 测量页面高度()
    {
        double height = 0;
        foreach (var item in 段落列表)
        {
            height += item.段前距;
            height += item.ActualHeight;
            height += item.段后距;
        }
        height += 段落间距 * (段落列表.Count - 1);
        return height;
    }

    #endregion

    #region 字段

    private readonly 绘图图层 _图层 = new 绘图图层();

    private 段落? _当前段落 = null;
    private 行内元素? _高亮元素 = null;

    #endregion
}