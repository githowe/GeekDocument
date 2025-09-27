namespace GeekDocument.SubSystem.LayoutEngine.Tool;

public class 元素行生成器
{
    public List<行内元素> 源元素列表 { get; set; } = new List<行内元素>();

    public 元素行? 生成元素行(double 行宽, bool 两端对齐)
    {
        // 重置生成器状态
        Reset();

        // 已取完
        if (_currentIndex >= 源元素列表.Count) return null;

        // 创建行
        _行宽 = 行宽;
        // 循环添加元素至行
        while (_currentIndex < 源元素列表.Count)
        {
            // 取出元素
            行内元素 元素 = 源元素列表[_currentIndex];
            // 空白元素直接添加，必然添加成功
            if (元素.IsSpace)
            {
                _currentIndex++;
                尝试添加元素(元素, 两端对齐);
                continue;
            }
            // 处理可断开元素
            if (元素.CanBreak && 无可见元素 && 剩余空间 > 0 && 元素.ActualWidth > 剩余空间)
            {
                // 非两端对齐，
                if (!两端对齐 || 元素.压缩实际宽度() > 剩余空间)
                {
                    行内元素 断开部分 = 元素.断开(剩余空间);
                    源元素列表.Insert(_currentIndex, 断开部分);
                    continue;
                }
            }
            // 添加至行，添加失败表示行已满
            bool added = 尝试添加元素(元素, 两端对齐);
            if (added) _currentIndex++;
            else break;
        }
        // 返回行
        return 生成元素行();
    }

    private void Reset()
    {
        _行宽 = 0;
        _当前元素列表.Clear();
        _最后一个元素右边距 = 0;
        状态 = 行状态.空;
        当前行宽 = 0;
    }

    private bool 尝试添加元素(行内元素 元素, bool 两端对齐)
    {
        if (能否添加元素(元素, 两端对齐, out bool 已压缩))
        {
            // 如果是通过压缩后才能添加元素，则比较压缩与拉伸的幅度，以决定是压缩还是拉伸，如果是拉伸，则不添加元素
            if (已压缩)
            {
                // 计算未压缩宽度：当前行宽 - 头部空白宽度 + 最后一个元素右边距+ 元素左边距 + 元素实际宽度
                double 未压缩宽度 = 当前行宽 - 获取头部空白宽度() + _最后一个元素右边距 + 元素.LeftMargin + 元素.ActualWidth;
                // 容器宽度 = 行宽 - 头部空白宽度
                double 容器宽度 = _行宽 - 获取头部空白宽度();
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

    private bool 全是空白()
    {
        foreach (var item in _当前元素列表)
            if (!item.IsSpace) return false;
        return true;
    }

    private bool 能否添加元素(行内元素 元素, bool 两端对齐, out bool 已压缩)
    {
        // 如果是压缩后才能添加元素，设置此标记为真
        已压缩 = false;

        // 无限行宽，直接添加
        if (double.IsPositiveInfinity(_行宽)) return true;
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
            if (剩余空间 >= _最后一个元素右边距 + 元素.LeftMargin + 元素.ActualWidth) return true;
            // 如果两端对齐
            if (两端对齐)
            {
                // 获取非头部元素列表并加上当前元素
                List<行内元素> 非头部元素列表 = 获取非头部元素列表();
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
                double 容器宽度 = _行宽 - 获取头部空白宽度();
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

    private void 添加元素(行内元素 元素)
    {
        // 添加元素主要就两步：添加元素、更新当前行宽

        switch (状态)
        {
            case 行状态.空:
                _当前元素列表.Add(元素);
                当前行宽 += 元素.ActualWidth;
                if (元素.IsSpace) 状态 = 行状态.填充空格;
                else
                {
                    _最后一个元素右边距 = 元素.RightMargin;
                    状态 = 行状态.填充元素;
                }
                break;
            case 行状态.填充空格:
                _当前元素列表.Add(元素);
                当前行宽 += 元素.ActualWidth;
                if (!元素.IsSpace)
                {
                    _最后一个元素右边距 = 元素.RightMargin;
                    状态 = 行状态.填充元素;
                }
                break;
            case 行状态.填充元素:
                _当前元素列表.Add(元素);
                当前行宽 += _最后一个元素右边距;
                当前行宽 += 元素.LeftMargin + 元素.ActualWidth;
                _最后一个元素右边距 = 元素.RightMargin;
                break;
        }
    }

    private double 获取头部空白宽度()
    {
        // 头部空白宽度 = 头部连续空格元素宽度

        double 结果 = 0;
        // 正向遍历元素列表
        for (int index = 0; index < _当前元素列表.Count; index++)
        {
            // 累加空白元素宽度
            行内元素 元素 = _当前元素列表[index];
            if (元素.IsSpace) 结果 += 元素.ActualWidth;
            // 遇到非空白元素时，退出循环
            else break;
        }
        return 结果;
    }

    private List<行内元素> 获取非头部元素列表()
    {
        List<行内元素> 结果 = new List<行内元素>(_当前元素列表);
        // 移除头部空白元素
        while (结果.Count > 0)
        {
            if (结果[0].IsSpace) 结果.RemoveAt(0);
            else break;
        }
        return 结果;
    }

    private 元素行 生成元素行()
    {
        元素行 result = new 元素行();
        result.Init();
        result.元素列表.AddRange(_当前元素列表);
        result.AddChildList(_当前元素列表.Cast<布局元素>().ToList());
        return result;
    }

    private bool 无可见元素
    {
        get
        {
            if (_当前元素列表.Count == 0) return true;
            return 全是空白();
        }
    }

    private double 剩余空间 => _行宽 - 当前行宽;

    private 行状态 状态 = 行状态.空;
    private double 当前行宽 = 0;
    private int _currentIndex = 0;
    private double _行宽;
    private List<行内元素> _当前元素列表 = new List<行内元素>();
    private double _最后一个元素右边距 = 0;
}