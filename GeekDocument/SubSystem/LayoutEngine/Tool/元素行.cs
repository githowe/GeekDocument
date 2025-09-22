using GeekDocument.SubSystem.EditerSystem.Define;
using GeekDocument.SubSystem.EditerSystemNew.Control;
using GeekDocument.SubSystem.EditerSystemNew.Define;
using GeekDocument.SubSystem.LayoutEngine.Element;
using GeekDocument.SubSystem.LayoutEngine.Ex;
using GeekDocument.SubSystem.OptionSystem;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GeekDocument.SubSystem.LayoutEngine.Tool;

public class 元素行 : IDocumentElement
{
    #region 构造方法

    public 元素行()
    {
        _stateTree = new STElementLine(this);
        _stateTree.Init();
        _rowLinePen.Freeze();
    }

    #endregion

    #region IDocumentElement 成员

    public IDocumentElement? ParentElement { get; set; } = null;

    public string Icon { get; set; } = "Line";

    public string Name { get; set; } = "元素行";

    public bool CanInput => true;

    public List<IDocumentElement> GetSubElementList()
    {
        return 元素列表.Cast<IDocumentElement>().ToList();
    }

    public Rect GetViewRect()
    {
        if (Owner == null) throw new Exception("当前元素行没有所属段落");
        double top = Owner.GetRootParagraph().段落偏移 + Top;
        return new Rect(Left, top, 行宽, 行高);
    }

    public Rect GetHitTestRect() => GetViewRect();

    public IDocumentElement? GetHitedElement(Point point)
    {
        for (int index = 元素列表.Count - 1; index >= 0; index--)
        {
            布局元素? item = 元素列表[index];
            IDocumentElement? hited = item.GetHitedElement(point);
            if (hited != null) return hited;
            if (item.GetViewRect().Contains(point)) return item;
        }
        if (GetHitTestRect().Contains(point)) return this;
        return null;
    }

    public IDocumentElement GetNearestElement(Point point)
    {
        // 元素行可能没有元素，此时直接返回自己
        if (元素列表.Count == 0) return this;

        IDocumentElement? 间接命中 = null;
        // 先通过横坐标获取命中元素
        for (int index = 元素列表.Count - 1; index >= 0; index--)
        {
            布局元素 元素 = 元素列表[index];
            Rect viewRect = 元素.GetViewRect();
            double x = point.X;
            if (viewRect.Left <= x && x < viewRect.Right)
            {
                间接命中 = 元素;
                break;
            }
        }
        // 无命中，可能点到了元素间的空白区域
        if (间接命中 == null)
        {
            间接命中 = 元素列表[0];
            double 最小距离 = double.MaxValue;
            foreach (var item in 元素列表)
            {
                Rect viewRect = item.GetViewRect();
                double distance = Math.Min(Math.Abs(point.X - viewRect.Left), Math.Abs(point.X - viewRect.Right));
                if (distance < 最小距离)
                {
                    最小距离 = distance;
                    间接命中 = item;
                }
            }
        }
        return 间接命中.GetNearestElement(point);
    }

    public CaretInfo MoveCaret(Point point)
    {
        if (元素列表.Count == 0)
        {
            _elementIndex = 0;
            return 获取空行光标信息();
        }

        // 获取第一个元素与最后一个元素
        布局元素 first = 元素列表[0];
        布局元素 last = 元素列表.Last();
        // 横坐标位于第一个元素左侧
        if (point.X < first.Left)
        {
            _elementIndex = 0;
            return 移动光标至元素左侧(first);
        }
        // 横坐标位于最后一个元素右侧
        if (point.X >= last.Left + last.ActualWidth)
        {
            _elementIndex = 元素列表.Count;
            return 移动光标至元素右侧(last);
        }

        // 获取命中元素
        布局元素 命中元素 = 获取命中元素(point);
        // 命中元素可以输入，则进一步获取元素内部的光标位置
        if (命中元素.CanInput) return 命中元素.MoveCaret(point);

        int elementIndex = 元素列表.IndexOf(命中元素);
        Rect elementRect = 命中元素.GetViewRect();
        // 命中元素左半部分
        if (point.X < elementRect.Left + elementRect.Width / 2)
        {
            _elementIndex = elementIndex;
            return 移动光标至元素左侧(命中元素);
        }
        // 命中元素右半部分
        else
        {
            _elementIndex = elementIndex + 1;
            return 移动光标至元素右侧(命中元素);
        }
    }

    public void HandleMouseDown(Point point)
    {

    }

    public 元素行 GetHitedLine(Point point)
    {
        // 没有元素时，直接返回自己
        if (元素列表.Count == 0) return this;

        // 获取第一个元素与最后一个元素
        布局元素 first = 元素列表[0];
        布局元素 last = 元素列表.Last();
        // 横坐标位于第一个元素左侧或最后一个元素右侧，返回自己
        if (point.X < first.Left || point.X >= last.Left + last.ActualWidth) return this;

        // 获取命中元素
        布局元素 命中元素 = 获取命中元素(point);
        // 命中元素可以输入，返回命中元素内的元素行
        if (命中元素.CanInput) return 命中元素.GetHitedLine(point);

        return this;
    }

    public void MoveInCaretToStart() => MoveCaretToStart(ElementSide.Left);

    public void MoveInCaretToEnd() => MoveCaretToEnd(ElementSide.Right);

    public void MoveOutCaretFromStart(IDocumentElement sender)
    {
        if (sender is 布局元素 element)
        {
            GetOwnerPage().UpdateCurrentLine(this);
            _elementIndex = 元素列表.IndexOf(element);
            CaretInfo info = 移动光标至元素左侧(element);
            GetOwnerPage().MoveCaret(info.X, info.Y, info.Height);
        }
    }

    public void MoveOutCaretFromEnd(IDocumentElement sender)
    {
        if (sender is 布局元素 element)
        {
            GetOwnerPage().UpdateCurrentLine(this);
            _elementIndex = 元素列表.IndexOf(element) + 1;
            CaretInfo info = 移动光标至元素右侧(element);
            GetOwnerPage().MoveCaret(info.X, info.Y, info.Height);
        }
    }

    #endregion

    #region 属性

    public 段落 Owner { get; set; } = null!;

    public double Left { get; set; } = double.NaN;

    public double Top { get; set; } = double.NaN;

    public double 行宽 { get; set; } = 0;

    public double 行高 { get; private set; } = 0;

    public bool 首行 { get; set; } = false;

    public List<布局元素> 元素列表 { get; set; } = new List<布局元素>();

    #endregion

    #region object 方法

    public override string ToString()
    {
        if (元素列表.Count == 0) return "空";
        return "非空行";
    }

    #endregion

    #region 公开方法

    public void 更新行高(double minHeight)
    {
        _fontSize = minHeight;
        行高 = minHeight;
        foreach (var item in 元素列表)
            if (item.ActualHeight > 行高) 行高 = item.ActualHeight;
    }

    public void 更新元素坐标(水平对齐方式 水平 = 水平对齐方式.Justify, 垂直对齐方式 垂直 = 垂直对齐方式.Bottom)
    {
        _horizontal = 水平;
        _vertical = 垂直;
        更新元素横坐标(水平);
        更新元素纵坐标(垂直);
        foreach (var item in 元素列表)
            if (item.类型 != 元素类型.字) item.Arrange();
    }

    public double 获取实际宽度()
    {
        double width = 0;
        foreach (var item in 元素列表)
            width += item.LeftMargin + item.ActualWidth + item.RightMargin;
        if (元素列表.Count > 1)
        {
            width -= 元素列表[0].LeftMargin;
            width -= 元素列表.Last().RightMargin;
        }
        return width;
    }

    public void HandleEditKey(EditKey key) => _stateTree.HandleEditKey(key);

    public void HandleCtrlEditKey(Key key)
    {

    }

    public void MoveCaretToStart(ElementSide side)
    {
        // 当前行没有元素
        if (元素列表.Count == 0)
        {
            EmptyLineMoveCaret();
            return;
        }
        // 获取第一个元素
        布局元素 first = 元素列表[0];
        // 移动至第一个元素左侧
        if (side == ElementSide.Left)
        {
            _elementIndex = 0;
            GetOwnerPage().UpdateCurrentLine(this);
            CaretInfo info = 移动光标至元素左侧(first);
            GetOwnerPage().MoveCaret(info.X, info.Y, info.Height);
            return;
        }
        // 第一个元素不支持输入
        if (!first.CanInput)
        {
            _elementIndex = 1;
            GetOwnerPage().UpdateCurrentLine(this);
            CaretInfo info = 移动光标至元素右侧(first);
            GetOwnerPage().MoveCaret(info.X, info.Y, info.Height);
            return;
        }
        // 支持输入，移入光标至元素开头
        first.MoveInCaretToStart();
    }

    /// <summary>
    /// 移动光标至最后一个元素
    /// </summary>
    public void MoveCaretToEnd(ElementSide side)
    {
        // 当前行没有元素
        if (元素列表.Count == 0)
        {
            EmptyLineMoveCaret();
            return;
        }
        // 获取最后一个元素
        布局元素 last = 元素列表.Last();
        // 移动至最后一个元素右侧
        if (side == ElementSide.Right)
        {
            _elementIndex = 元素列表.Count;
            // 更新页面的当前元素行
            GetOwnerPage().UpdateCurrentLine(this);
            CaretInfo info = 移动光标至元素右侧(last);
            GetOwnerPage().MoveCaret(info.X, info.Y, info.Height);
            return;
        }
        // 最后一个元素不支持输入
        if (!last.CanInput)
        {
            _elementIndex = 元素列表.Count - 1;
            // 移动至最后一个元素左侧
            GetOwnerPage().UpdateCurrentLine(this);
            CaretInfo info = 移动光标至元素左侧(last);
            GetOwnerPage().MoveCaret(info.X, info.Y, info.Height);
            return;
        }
        // 支持输入，移入光标至元素末尾
        last.MoveInCaretToEnd();
    }

    public void 绘图(DrawingContext dc)
    {
        // 绘制行线
        if (Options.Instance.View.ShowRowLine)
        {
            dc.DrawLine(_rowLinePen, new Point(Left, Top + 0.5), new Point(Left + 行宽, Top + 0.5));
            dc.DrawLine(_rowLinePen, new Point(Left, Top + 行高 - 0.5), new Point(Left + 行宽, Top + 行高 - 0.5));
        }
        // 绘制元素
        foreach (var item in 元素列表) item.绘图(dc);
    }

    #endregion

    #region 状态树接口

    public bool 光标前有元素() => _elementIndex > 0;

    public bool 前元素支持输入() => 元素列表[_elementIndex - 1].CanInput;

    public void 移入光标至前元素末尾()
    {
        元素列表[_elementIndex - 1].MoveInCaretToEnd();
    }

    public void 前移光标()
    {
        _elementIndex--;
        CaretInfo info = 移动光标至元素左侧(元素列表[_elementIndex]);
        GetOwnerPage().MoveCaret(info.X, info.Y, info.Height);
    }

    public void 调用所属段落的左移光标()
    {
        Owner.MoveLeftCaret(this);
    }

    public bool 光标后有元素() => _elementIndex < 元素列表.Count;

    public bool 当前元素支持输入() => 元素列表[_elementIndex].CanInput;

    public void 移入光标至当前元素开头()
    {
        元素列表[_elementIndex].MoveInCaretToStart();
    }

    public void 后移光标()
    {
        CaretInfo info = 移动光标至元素右侧(元素列表[_elementIndex]);
        GetOwnerPage().MoveCaret(info.X, info.Y, info.Height);
        _elementIndex++;
    }

    public void 调用所属段落的右移光标()
    {
        Owner.MoveRightCaret(this);
    }

    #endregion

    #region 私有方法

    private bool 全是空白()
    {
        foreach (var item in 元素列表)
            if (!item.IsSpace) return false;
        return true;
    }

    private double 获取头部空白宽度()
    {
        // 头部空白宽度 = 头部连续空格元素宽度

        double 结果 = 0;
        // 正向遍历元素列表
        for (int index = 0; index < 元素列表.Count; index++)
        {
            // 累加空白元素宽度
            布局元素 元素 = 元素列表[index];
            if (元素.IsSpace) 结果 += 元素.ActualWidth;
            // 遇到非空白元素时，退出循环
            else break;
        }
        return 结果;
    }

    private void 更新元素横坐标(水平对齐方式 对齐)
    {
        switch (对齐)
        {
            case 水平对齐方式.Left:
            case 水平对齐方式.Center:
            case 水平对齐方式.Right:
                横向排列元素();
                处理对齐(对齐);
                break;
            case 水平对齐方式.Justify:
                // 全是空白，按左对齐处理
                if (全是空白()) 横向排列元素();
                else 处理两端对齐();
                break;
        }
    }

    /// <summary>
    /// 将元素从左至右排列
    /// </summary>
    private void 横向排列元素()
    {
        行状态 状态 = 行状态.空;
        double 横坐标 = Left;
        int index = 0;
        while (index < 元素列表.Count)
        {
            布局元素 元素 = 元素列表[index];
            switch (状态)
            {
                case 行状态.空:
                    元素.Left = 横坐标;
                    if (元素.IsSpace)
                    {
                        横坐标 += 元素.ActualWidth;
                        状态 = 行状态.填充空格;
                    }
                    else
                    {
                        横坐标 += 元素.ActualWidth + 元素.RightMargin;
                        状态 = 行状态.填充元素;
                    }
                    break;
                case 行状态.填充空格:
                    元素.Left = 横坐标;
                    if (元素.IsSpace) 横坐标 += 元素.ActualWidth;
                    else
                    {
                        横坐标 += 元素.ActualWidth + 元素.RightMargin;
                        状态 = 行状态.填充元素;
                    }
                    break;
                case 行状态.填充元素:
                    横坐标 += 元素.LeftMargin;
                    元素.Left = 横坐标;
                    横坐标 += 元素.ActualWidth + 元素.RightMargin;
                    break;
            }
            index++;
        }
    }

    private void 处理对齐(水平对齐方式 对齐)
    {
        // 左对齐无需更新横坐标
        if (对齐 == 水平对齐方式.Left) return;
        // 宽度无限时无需更新横坐标
        if (double.IsPositiveInfinity(行宽)) return;

        // 全部元素宽度 = 最后一个元素横坐标 + 最后一个元素实际宽度
        布局元素 最后一个元素 = 元素列表.Last();
        double 全部元素宽度 = 最后一个元素.Left + 最后一个元素.ActualWidth - Left;
        // 根据对齐方式，偏移元素横坐标
        double offset = 0;
        if (对齐 == 水平对齐方式.Center) offset = (行宽 - 全部元素宽度) / 2;
        else if (对齐 == 水平对齐方式.Right) offset = 行宽 - 全部元素宽度;
        if (offset != 0)
        {
            foreach (var item in 元素列表)
                item.Left += offset;
        }
    }

    private void 处理两端对齐()
    {
        // 计算容器宽度。容器宽度 = 行宽 - 头部空白宽度
        double 容器宽度 = 行宽 - 获取头部空白宽度();
        // 获取可伸缩部分元素
        List<布局元素> 可伸缩部分 = 获取可伸缩部分元素();
        // 计算未压缩宽度
        double 未压缩宽度 = 0;
        if (可伸缩部分.Count == 1)
        {
            未压缩宽度 = 可伸缩部分[0].ActualWidth;
        }
        else if (可伸缩部分.Count == 2)
        {
            未压缩宽度 += 可伸缩部分[0].ActualWidth + 可伸缩部分[0].RightMargin;
            未压缩宽度 += 可伸缩部分[1].LeftMargin + 可伸缩部分[1].ActualWidth;
        }
        else
        {
            未压缩宽度 = 可伸缩部分[0].ActualWidth + 可伸缩部分[0].RightMargin;
            for (int index = 1; index < 可伸缩部分.Count - 1; index++)
            {
                布局元素 元素 = 可伸缩部分[index];
                未压缩宽度 += 元素.LeftMargin + 元素.ActualWidth + 元素.RightMargin;
            }
            未压缩宽度 += 可伸缩部分.Last().LeftMargin + 可伸缩部分.Last().ActualWidth;
        }
        // 执行拉伸
        if (未压缩宽度 < 容器宽度 && 可伸缩部分.Count > 1)
        {
            double 总拉伸量 = 容器宽度 - 未压缩宽度;
            double 平均拉伸量 = 总拉伸量 / (可伸缩部分.Count - 1);
            for (int index = 0; index < 可伸缩部分.Count - 1; index++)
                可伸缩部分[index].RightMargin += 平均拉伸量;
        }
        // 执行压缩
        else if (未压缩宽度 > 容器宽度)
        {
            // 计算极限压缩宽度
            double 极限压缩宽度 = 0;
            if (可伸缩部分.Count == 1)
            {
                极限压缩宽度 = 可伸缩部分[0].压缩实际宽度();
            }
            else if (可伸缩部分.Count == 2)
            {
                极限压缩宽度 += 可伸缩部分[0].压缩右边距();
                极限压缩宽度 += 可伸缩部分[1].压缩左边距();
            }
            else
            {
                极限压缩宽度 += 可伸缩部分[0].压缩右边距();
                for (int index = 1; index < 可伸缩部分.Count - 1; index++)
                    极限压缩宽度 += 可伸缩部分[index].压缩元素();
                极限压缩宽度 += 可伸缩部分.Last().压缩左边距();
            }
            // 可压缩量 = 未压缩宽度 - 极限压缩宽度
            double 可压缩量 = 未压缩宽度 - 极限压缩宽度;
            if (可压缩量 > 0)
            {
                // 目标压缩量 = 未压缩宽度 - 容器宽度
                double 目标压缩量 = 未压缩宽度 - 容器宽度;
                // 压缩比 = 目标压缩量 / 可压缩量
                double 压缩比 = 目标压缩量 / 可压缩量;
                // 外部已经处理了放不下的情况，如果这里出错，直接抛异常
                if (压缩比 > 1) throw new Exception("压缩过量");
                // 压缩元素
                foreach (var item in 可伸缩部分) item.压缩至(压缩比);
            }
        }
        横向排列元素();
    }

    private void 更新元素纵坐标(垂直对齐方式 对齐)
    {
        switch (对齐)
        {
            case 垂直对齐方式.Top:
                foreach (var item in 元素列表) item.Top = Top;
                break;
            case 垂直对齐方式.Center:
                foreach (var item in 元素列表) item.Top = Top + (行高 - item.ActualHeight) / 2;
                break;
            case 垂直对齐方式.Bottom:
                foreach (var item in 元素列表) item.Top = Top + 行高 - item.ActualHeight;
                break;
        }
    }

    private List<布局元素> 获取可伸缩部分元素()
    {
        // 可伸缩部分元素 = 全部元素 - 头部空白元素 - 尾部空白元素

        List<布局元素> 结果 = new List<布局元素>(元素列表);
        // 移除头部空白元素
        while (结果.Count > 0)
        {
            if (结果[0].IsSpace) 结果.RemoveAt(0);
            else break;
        }
        // 移除尾部空白元素
        while (结果.Count > 0)
        {
            if (结果.Last().IsSpace) 结果.RemoveAt(结果.Count - 1);
            else break;
        }
        return 结果;
    }

    /// <summary>
    /// 空行移动光标：直接移动至行首
    /// </summary>
    private void EmptyLineMoveCaret()
    {
        // 更新页面的当前元素行
        GetOwnerPage().UpdateCurrentLine(this);
        _elementIndex = 0;
        // 移动光标
        CaretInfo info = 获取空行光标信息();
        GetOwnerPage().MoveCaret(info.X, info.Y, info.Height);
    }

    private CaretInfo 获取空行光标信息()
    {
        CaretInfo result = new CaretInfo();
        Rect lineRect = GetViewRect();

        // 横坐标根据水平对齐计算
        switch (_horizontal)
        {
            case 水平对齐方式.Left:
                result.X = lineRect.Left;
                break;
            case 水平对齐方式.Center:
                result.X = lineRect.Left + 行宽 / 2;
                break;
            case 水平对齐方式.Right:
                result.X = lineRect.Right;
                break;
            case 水平对齐方式.Justify:
                result.X = lineRect.Left;
                break;
        }
        // 纵坐标取顶部
        result.Y = lineRect.Top;
        // 高度取字号
        result.Height = _fontSize;

        return result;
    }

    private CaretInfo 移动光标至元素左侧(布局元素 元素)
    {
        CaretInfo result = new CaretInfo();

        // 获取当前元素索引
        int elementIndex = 元素列表.IndexOf(元素);

        // 无前一个元素
        if (elementIndex == 0)
        {
            // 横坐标取当前元素左
            result.X = 元素.GetViewRect().Left;
            // 纵坐标根据垂直对齐与字号计算
            result.Y = 计算光标纵坐标();
            // 高度取字号
            result.Height = _fontSize;
            // 返回结果
            return result;
        }

        // 获取前一个元素以及前一个元素区域
        布局元素 前一个元素 = 元素列表[elementIndex - 1];
        Rect prevRect = 前一个元素.GetViewRect();
        // 前一个元素为字元素
        if (前一个元素.类型 == 元素类型.字)
        {
            // 坐标取前一个元素右上角
            result.X = prevRect.Right;
            result.Y = prevRect.Top;
            // 高度取前一个元素高度
            result.Height = 前一个元素.ActualHeight;
        }
        // 其他元素
        else
        {
            // 横坐标取前一个元素右 + 右间距
            result.X = prevRect.Right + 前一个元素.RightMargin;
            // 纵坐标根据垂直对齐与字号计算
            result.Y = 计算光标纵坐标();
            // 高度取字号
            result.Height = _fontSize;
        }

        return result;
    }

    private CaretInfo 移动光标至元素右侧(布局元素 元素)
    {
        CaretInfo result = new CaretInfo();
        Rect elementRect = 元素.GetViewRect();

        // 当前元素为字元素
        if (元素.类型 == 元素类型.字)
        {
            // 坐标取当前字右上角
            result.X = elementRect.Right;
            result.Y = elementRect.Top;
            // 高度取当前字高度
            result.Height = 元素.ActualHeight;
        }
        // 其他元素
        {
            // 横坐标取当前元素右
            result.X = elementRect.Right + 元素.RightMargin;
            // 纵坐标根据垂直对齐与字号计算
            result.Y = 计算光标纵坐标();
            // 高度取字号
            result.Height = _fontSize;
        }

        return result;
    }

    private double 计算光标纵坐标()
    {
        Rect lineRect = GetViewRect();
        return _vertical switch
        {
            垂直对齐方式.Top => lineRect.Top,
            垂直对齐方式.Center => lineRect.Top + (行高 - _fontSize) / 2,
            垂直对齐方式.Bottom => lineRect.Bottom - _fontSize,
            _ => 0,
        };
    }

    private 布局元素 获取命中元素(Point point)
    {
        布局元素? 命中元素 = null;

        // 先通过横坐标获取命中元素
        for (int index = 元素列表.Count - 1; index >= 0; index--)
        {
            布局元素 元素 = 元素列表[index];
            Rect viewRect = 元素.GetViewRect();
            double x = point.X;
            if (viewRect.Left <= x && x < viewRect.Right)
            {
                命中元素 = 元素;
                break;
            }
        }
        // 无命中，可能点到了元素间的空白区域
        if (命中元素 == null)
        {
            命中元素 = 元素列表[0];
            double 最小距离 = double.MaxValue;
            foreach (var item in 元素列表)
            {
                Rect viewRect = item.GetViewRect();
                double distance = Math.Min(Math.Abs(point.X - viewRect.Left), Math.Abs(point.X - viewRect.Right));
                if (distance < 最小距离)
                {
                    最小距离 = distance;
                    命中元素 = item;
                }
            }
        }

        return 命中元素;
    }

    private Page GetOwnerPage()
    {
        if (_ownerPage == null) _ownerPage = Owner.GetPage();
        return _ownerPage;
    }

    #endregion

    #region 字段

    private double _fontSize = 0;
    private 水平对齐方式 _horizontal = 水平对齐方式.Left;
    private 垂直对齐方式 _vertical = 垂直对齐方式.Bottom;

    private int _elementIndex = 0;

    private readonly STElementLine _stateTree;
    private Page? _ownerPage = null;

    private readonly Pen _rowLinePen = new Pen(new SolidColorBrush(Color.FromArgb(32, 255, 255, 255)), 1);

    #endregion
}