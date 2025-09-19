using GeekDocument.SubSystem.LayoutEngine.Element;
using GeekDocument.SubSystem.LayoutEngine.Ex;
using System.Windows;

namespace GeekDocument.SubSystem.LayoutEngine.Tool;

public class 元素行 : IDocumentElement
{
    #region IDocumentElement 属性

    public string Icon { get; set; } = "Line";

    public string Name { get; set; } = "元素行";

    #endregion

    #region 属性

    public 段落? Owner { get; set; } = null;

    public double Left { get; set; } = double.NaN;

    public double Top { get; set; } = double.NaN;

    public double 行宽 { get; set; } = 0;

    public double 行高 { get; private set; } = 0;

    public bool 首行 { get; set; } = false;

    public List<布局元素> 元素列表 { get; set; } = new List<布局元素>();

    public bool 无可见元素
    {
        get
        {
            if (元素列表.Count == 0) return true;
            return 全是空白();
        }
    }

    public double 剩余空间 => 行宽 - 当前行宽;

    public double 最后一个元素右边距 { get; private set; } = 0;

    #endregion

    public override string ToString()
    {
        if (元素列表.Count == 0) return "空";
        return "非空行";
    }

    public List<IDocumentElement> GetSubElementList()
    {
        return 元素列表.Cast<IDocumentElement>().ToList();
    }

    public Rect GetElementRect()
    {
        if (Owner == null) throw new Exception("当前元素行没有所属段落");
        double top = Owner.GetRootParagraph().段落偏移 + Top;
        return new Rect(Left, top, 行宽, 行高);
    }

    #region 公开方法

    public bool 尝试添加元素(布局元素 元素, bool 两端对齐)
    {
        if (能否添加元素(元素, 两端对齐, out bool 已压缩))
        {
            // 如果是通过压缩后才能添加元素，则比较压缩与拉伸的幅度，以决定是压缩还是拉伸，如果是拉伸，则不添加元素
            if (已压缩)
            {
                // 计算未压缩宽度：当前行宽 - 头部空白宽度 + 最后一个元素右边距+ 元素左边距 + 元素实际宽度
                double 未压缩宽度 = 当前行宽 - 获取头部空白宽度() + 最后一个元素右边距 + 元素.LeftMargin + 元素.ActualWidth;
                // 容器宽度 = 行宽 - 头部空白宽度
                double 容器宽度 = 行宽 - 获取头部空白宽度();
                // 计算压缩幅度与拉伸幅度
                double 压缩幅度 = 未压缩宽度 - 容器宽度;
                double 拉伸幅度 = 剩余空间;
                // 压缩幅度小，则添加元素
                if (压缩幅度 < 拉伸幅度)
                {
                    添加元素(元素);
                    return true;
                }
                // 否则，不添加元素
                else return false;
            }
            else
            {
                添加元素(元素);
                return true;
            }
        }
        return false;
    }

    public void 更新行高(double 最小行高)
    {
        行高 = 最小行高;
        foreach (var item in 元素列表)
            if (item.ActualHeight > 行高) 行高 = item.ActualHeight;
    }

    public void 更新元素坐标(水平对齐方式 水平 = 水平对齐方式.Justify, 垂直对齐方式 垂直 = 垂直对齐方式.Bottom)
    {
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

    #endregion

    #region 私有方法

    private bool 全是空白()
    {
        foreach (var item in 元素列表)
            if (!item.IsSpace) return false;
        return true;
    }

    private bool 能否添加元素(布局元素 元素, bool 两端对齐, out bool 已压缩)
    {
        // 如果是压缩后才能添加元素，设置此标记为真
        已压缩 = false;

        // 无限行宽，直接添加
        if (double.IsPositiveInfinity(行宽)) return true;
        // 空白元素可以无限添加，不管行有没有满
        if (元素.IsSpace) return true;
        // 没有元素，直接添加。可断开元素已在外部处理
        if (状态 == 行状态.空) return true;

        // 非空白元素，首先判断未压缩状态下能否添加，再判断极限压缩后能否添加

        // 当前行全是空白元素时，判断能否添加只考虑元素的实际宽度
        if (状态 == 行状态.填充空格)
        {
            // 空白元素已经满行时，无法添加非空白元素
            if (剩余空间 <= 0) return false;
            // 剩余空间>= 元素实际宽度，可以添加
            if (剩余空间 >= 元素.ActualWidth) return true;
            // 如果两端对齐
            if (两端对齐)
            {
                // 剩余空间 >= 极限压缩后的实际宽度，可以添加
                if (剩余空间 >= 元素.压缩实际宽度())
                {
                    已压缩 = true;
                    return true;
                }
            }
        }
        // 否则，判断元素的左边距加实际宽度能否添加
        else
        {
            // 剩余空间>= 最后一个元素右边距 + 元素左边距 + 实际宽度，可以添加
            if (剩余空间 >= 最后一个元素右边距 + 元素.LeftMargin + 元素.ActualWidth) return true;
            // 如果两端对齐
            if (两端对齐)
            {
                // 获取非头部元素列表并加上当前元素
                List<布局元素> 非头部元素列表 = 获取非头部元素列表();
                非头部元素列表.Add(元素);
                // 进行极限压缩。第一个元素忽略左边距，最后一个元素忽略右边距
                double 压缩后宽度 = 0;
                if (非头部元素列表.Count == 1)
                {
                    压缩后宽度 = 非头部元素列表[0].压缩实际宽度();
                }
                else if (非头部元素列表.Count == 2)
                {
                    压缩后宽度 += 非头部元素列表[0].压缩右边距();
                    压缩后宽度 += 非头部元素列表[1].压缩左边距();
                }
                else
                {
                    压缩后宽度 += 非头部元素列表[0].压缩右边距();
                    for (int index = 1; index < 非头部元素列表.Count - 1; index++)
                        压缩后宽度 += 非头部元素列表[index].压缩元素();
                    压缩后宽度 += 非头部元素列表.Last().压缩左边距();
                }
                // 容器宽度 = 行宽 - 头部空白宽度
                double 容器宽度 = 行宽 - 获取头部空白宽度();
                // 容器宽度 >= 极限压缩后的实际宽度，可以添加
                if (容器宽度 >= 压缩后宽度)
                {
                    已压缩 = true;
                    return true;
                }
            }
        }

        return false;
    }

    private void 添加元素(布局元素 元素)
    {
        // 添加元素主要就两步：添加元素、更新当前行宽

        switch (状态)
        {
            case 行状态.空:
                元素列表.Add(元素);
                当前行宽 += 元素.ActualWidth;
                if (元素.IsSpace) 状态 = 行状态.填充空格;
                else
                {
                    最后一个元素右边距 = 元素.RightMargin;
                    状态 = 行状态.填充元素;
                }
                break;
            case 行状态.填充空格:
                元素列表.Add(元素);
                当前行宽 += 元素.ActualWidth;
                if (!元素.IsSpace)
                {
                    最后一个元素右边距 = 元素.RightMargin;
                    状态 = 行状态.填充元素;
                }
                break;
            case 行状态.填充元素:
                元素列表.Add(元素);
                当前行宽 += 最后一个元素右边距;
                当前行宽 += 元素.LeftMargin + 元素.ActualWidth;
                最后一个元素右边距 = 元素.RightMargin;
                break;
        }
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

    private List<布局元素> 获取非头部元素列表()
    {
        List<布局元素> 结果 = new List<布局元素>(元素列表);
        // 移除头部空白元素
        while (结果.Count > 0)
        {
            if (结果[0].IsSpace) 结果.RemoveAt(0);
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

    #endregion

    #region 字段

    public enum 行状态
    {
        空,
        填充空格,
        填充元素,
    }

    private 行状态 状态 = 行状态.空;
    private double 当前行宽 = 0;

    #endregion
}