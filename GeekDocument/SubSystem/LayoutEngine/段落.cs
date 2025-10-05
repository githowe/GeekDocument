using GeekDocument.SubSystem.LayoutEngine.Tool;
using System.Windows;
using System.Windows.Shapes;

namespace GeekDocument.SubSystem.LayoutEngine;

public class 段落 : 布局元素
{
    #region 构造方法

    public 段落()
    {
        Name = "段落";
        Icon = "Paragraph";
    }

    #endregion

    #region 属性

    public string 文本 { get; set; } = "";

    public List<行内元素> 内嵌元素列表 { get; set; } = new List<行内元素>();

    public string 字体 { get; set; } = "霞鹜文楷";

    public int 字号 { get; set; } = 16;

    public 水平对齐方式 水平对齐 { get; set; } = 水平对齐方式.Justify;

    public 垂直对齐方式 垂直对齐 { get; set; } = 垂直对齐方式.Bottom;

    public double 首行缩进 { get; set; } = 32;

    public double 自定义首行缩进 { get; set; } = 0;

    public bool 使用自定义首行缩进 { get; set; } = false;

    public double 左缩进 { get; set; } = 0;

    public double 右缩进 { get; set; } = 0;

    public double 自定义段间距 { get; set; } = 4;

    public bool 使用自定义段间距 { get; set; } = false;

    public double 行间距 { get; set; } = 4;

    public double 段前距 { get; set; } = 0;

    public double 段后距 { get; set; } = 0;

    #endregion

    #region 运行时属性

    public 页面? OwnerPage { get; set; } = null;

    public double Left { get; set; } = double.NaN;

    public double Top { get; set; } = double.NaN;

    public double Width { get; set; } = double.NaN;

    public double MaxWidth { get; set; } = double.NaN;

    public double ActualWidth { get; set; } = double.NaN;

    public double ActualHeight { get; set; } = double.NaN;

    public bool 纯文本模式 { get; set; } = false;

    public List<行内元素集> 元素集列表 { get; set; } = new List<行内元素集>();

    public List<元素行> 元素行列表 { get; set; } = new List<元素行>();

    public int 光标索引 => _光标索引;

    public List<行内元素> 全部行内元素 => _全部行内元素;

    #endregion

    #region 布局元素方法

    public override void 更新绘图对象(string reason)
    {
        if (OwnerPage != null)
        {
            OwnerPage.更新绘图对象(reason);
            return;
        }
        Parent?.更新绘图对象(reason);
    }

    public override List<绘图对象> 获取绘图对象()
    {
        List<绘图对象> result = new List<绘图对象>();
        foreach (var item in 元素行列表)
            result.AddRange(item.获取绘图对象());
        return result;
    }

    public override 命中信息? 获取命中信息(Point point)
    {
        命中信息? result = null;
        // 先获取元素行的命中信息
        for (int index = 元素行列表.Count - 1; index >= 0; index--)
        {
            元素行? item = 元素行列表[index];
            result = item.获取命中信息(point);
            if (result != null) return result;
        }
        // 获取自身的可命中区域
        Rect rect = new Rect(Left, Top, ActualWidth, ActualHeight);
        if (rect.Contains(point))
        {
            result = new 命中信息
            {
                坐标 = point,
                命中元素 = this,
                命中区域 = rect,
                区域名称 = "段落"
            };
        }
        // 返回命中信息
        return result;
    }

    public override 元素行 获取最近元素行(Point point)
    {
        元素行? 命中行 = null;
        // 先通过纵坐标获取命中行
        for (int index = 元素行列表.Count - 1; index >= 0; index--)
        {
            元素行 元素行 = 元素行列表[index];
            if (元素行.Top <= point.Y && point.Y < 元素行.Top + 元素行.ActualHeight)
            {
                命中行 = 元素行;
                break;
            }
        }
        // 无命中，说明行之间有间隔，此时通过距离找到最近行
        if (命中行 == null)
        {
            命中行 = 元素行列表[0];
            double 最小距离 = double.MaxValue;
            foreach (var 元素行 in 元素行列表)
            {
                double distance = Math.Min(Math.Abs(point.Y - 元素行.Top), Math.Abs(point.Y - (元素行.Top + 元素行.ActualHeight)));
                if (distance < 最小距离)
                {
                    最小距离 = distance;
                    命中行 = 元素行;
                }
            }
        }
        return 命中行.获取最近元素行(point);
    }

    public override Rect GetViewRect() => new Rect(Left, Top, ActualWidth, ActualHeight);

    #endregion

    #region 布局元素核心方法

    public override void Init()
    {
        元素集列表.Clear();
        // 将文本分割成多个元素集
        foreach (var part in 文本.Split(_占位标记))
        {
            行内元素集 元素集 = new 行内元素集 { Text = part };
            生成字元素(元素集);
            元素集列表.Add(元素集);
        }
        // 只有一个元素集时，表示没有内嵌元素，直接获取全部行内元素返回
        if (元素集列表.Count == 1)
        {
            _全部行内元素 = 获取全部行内元素();
            return;
        }
        // 插入内嵌元素
        List<行内元素集> 新列表 = new List<行内元素集>();
        // 每两个字元素集之间添加一个内嵌元素集
        for (int index = 0; index < 元素集列表.Count - 1; index++)
        {
            新列表.Add(元素集列表[index]);
            行内元素集 单元素集 = new 行内元素集 { InnerElement = true };
            单元素集.行内元素列表.Add(获取一个内嵌元素());
            新列表.Add(单元素集);
        }
        // 添加最后一个元素集
        新列表.Add(元素集列表.Last());
        // 替换元素集列表
        元素集列表 = 新列表;
        _全部行内元素 = 获取全部行内元素();
    }

    public override void 测量()
    {
        约束();

        // 先测量全部行内元素
        foreach (var item in _全部行内元素) item.测量();
        // 再根据段落宽度与对齐方式，将全部行内元素分割成元素行列表
        元素行列表.Clear();
        ClearChildren();
        // 创建生成器
        元素行生成器 生成器 = new 元素行生成器 { 源元素列表 = _全部行内元素 };
        // 循环生成元素行
        while (true)
        {
            double 行宽 = 获取填充元素宽度();
            元素行? 元素行 = 生成器.生成元素行(行宽, 水平对齐 == 水平对齐方式.Justify);
            if (元素行 != null)
            {
                元素行.首行 = 元素行列表.Count == 0;
                元素行.首行缩进 = 获取首行缩进();
                元素行.字号 = 字号;
                元素行列表.Add(元素行);
                AddChild(元素行);
            }
            else break;
        }
        // 没有生成任何元素行，则添加一个空元素行
        if (元素行列表.Count == 0)
        {
            元素行 空行 = new 元素行
            {
                首行 = true,
                首行缩进 = 获取首行缩进(),
                字号 = 字号,
            };
            空行.Init();
            元素行列表.Add(空行);
            AddChild(空行);
        }
        // 段落设置了固定宽度，则将所有元素行设置为固定宽度
        if (!double.IsNaN(Width))
        {
            double lineFixedWidth = Width - 左缩进 - 右缩进;
            foreach (var item in 元素行列表)
            {
                item.Width = lineFixedWidth;
                item.测量();
            }
            // 设置段落实际宽度
            ActualWidth = Width;
        }
        // 否则，设置元素行的最大宽度
        else
        {
            double lineMaxWidth = MaxWidth - 左缩进 - 右缩进;
            foreach (var item in 元素行列表)
            {
                item.MaxWidth = lineMaxWidth;
                item.测量();
            }
            // 取元素行中最大的宽度作为段落宽度
            ActualWidth = 0;
            foreach (var item in 元素行列表)
                if (item.ActualWidth > ActualWidth) ActualWidth = item.ActualWidth;
            // 再将所有行设置为统一宽度
            foreach (var item in 元素行列表)
            {
                item.Width = ActualWidth;
                item.ActualWidth = ActualWidth;
            }
            // 设置段落实际宽度：因为获取最大宽度时已经减去缩进，所以这里要加上缩进
            ActualWidth += 左缩进 + 右缩进;
        }
        // 计算段落高度
        ActualHeight = 0;
        foreach (var item in 元素行列表)
            ActualHeight += item.ActualHeight;
        ActualHeight += 行间距 * (元素行列表.Count - 1);
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
        else
        {
            // 通知页面重新测量
            if (OwnerPage != null) OwnerPage.重新测量(this);
            // 通知父元素重新测量与排列
            else Parent?.重新测量();
        }
    }

    public override void 排列()
    {
        if (double.IsNaN(Left) || double.IsNaN(Top)) throw new Exception("未设置段落坐标");

        // 设置每一行的坐标与垂直对齐
        double y = Top;
        int index = 0;
        foreach (var 元素行 in 元素行列表)
        {
            元素行.Left = Left + 左缩进;
            元素行.Top = y;
            y += 元素行.ActualHeight + 行间距;
            元素行.垂直对齐 = 垂直对齐;
            index++;
        }
        // 单行，按左对齐排列行内元素
        if (元素行列表.Count == 1)
        {
            元素行 第一行 = 元素行列表[0];
            if (水平对齐 == 水平对齐方式.Justify) 第一行.水平对齐 = 水平对齐方式.Left;
            else 第一行.水平对齐 = 水平对齐;
            第一行.排列();
            return;
        }
        // 多行，先排列除最后一行
        for (index = 0; index < 元素行列表.Count - 1; index++)
        {
            元素行列表[index].水平对齐 = 水平对齐;
            元素行列表[index].排列();
        }
        // 再排列最后一行
        元素行 最后一行 = 元素行列表.Last();
        if (水平对齐 == 水平对齐方式.Justify) 最后一行.水平对齐 = 水平对齐方式.Left;
        else 最后一行.水平对齐 = 水平对齐;
        最后一行.排列();
    }

    #endregion

    #region 公开方法

    public string 获取文本()
    {
        string result = "";
        foreach (var 集 in 元素集列表)
        {
            if (集.InnerElement) result += _占位标记;
            else
            {
                string text = 集.Text.Replace("<ele>", "\\<ele>");
                result += text;
            }
        }
        return result;
    }

    public List<行内元素> 获取内嵌元素()
    {
        List<行内元素> result = new List<行内元素>();
        foreach (var 集 in 元素集列表)
        {
            if (!集.InnerElement) continue;
            result.AddRange(集.行内元素列表);
        }
        return result;
    }

    /// <summary>
    /// 从指定处分割元素，然后返回分割出来的元素列表
    /// </summary>
    public List<行内元素> 分割元素(int index)
    {
        List<行内元素> left = _全部行内元素.Take(index).ToList();
        List<行内元素> right = _全部行内元素.Skip(index).ToList();
        _全部行内元素 = left;
        // 更新文本与内嵌元素列表
        更新文本与内嵌元素(left);
        // 返回分割出来的元素列表
        return right;
    }

    public void 更新文本与内嵌元素(List<行内元素> newList)
    {
        文本 = "";
        内嵌元素列表.Clear();
        foreach (var item in newList)
        {
            if (item is 字 字) 文本 += 字.字符;
            else
            {
                // 先用零宽字符占位，避免和文本中的 <ele> 冲突
                文本 += '\u002b';
                内嵌元素列表.Add(item);
            }
        }
        // 将文本中的 <ele> 转义
        文本 = 文本.Replace(_占位标记, $"\\{_占位标记}");
        // 再将零宽字符替换为 <ele>
        文本 = 文本.Replace("\u002b", _占位标记);
    }

    public void 更新光标索引(元素行 line)
    {
        _光标索引 = 获取段落光标索引(line);
        // Console.WriteLine("光标索引：" + _光标索引);
    }

    public void 移动光标至开头()
    {
        元素行列表[0].MoveInCaretToStart();
    }

    public void 移动光标至末尾()
    {
        元素行列表.Last().MoveInCaretToEnd();
    }

    public void 左移光标(元素行 元素行)
    {
        int 行索引 = 元素行列表.IndexOf(元素行);
        // 有上一行
        if (行索引 > 0)
        {
            元素行 上一行 = 元素行列表[行索引 - 1];
            上一行.MoveCaretToEnd(ElementSide.Left);
        }
        // 无上一行
        else
        {
            // 如果没有父级，表示该段落处于页面中，调用页面的左移光标
            if (Parent == null) OwnerPage?.左移光标(this);
            // 否则，表示段落处于其他元素中，此时调用父级的移出光标
            else Parent.从开头移出光标(this);
        }
    }

    public void 右移光标(元素行 元素行)
    {
        int 行索引 = 元素行列表.IndexOf(元素行);
        // 有下一行
        if (行索引 < 元素行列表.Count - 1)
        {
            元素行 下一行 = 元素行列表[行索引 + 1];
            下一行.MoveCaretToStart(ElementSide.Right);
        }
        else
        {
            if (Parent == null) OwnerPage?.右移光标(this);
            else Parent.从末尾移出光标(this);
        }
    }

    public void 移动光标至(int index)
    {
        元素行? 目标元素行 = null;
        int charIndex = 0;
        int indexInLine = 0;
        foreach (var 元素行 in 元素行列表)
        {
            int startIndex = charIndex;
            int endIndex = charIndex + 元素行.元素列表.Count;
            if (startIndex <= index && index <= endIndex)
            {
                目标元素行 = 元素行;
                indexInLine = index - startIndex;
                // 光标在末尾，且有下一个元素行
                if (index == endIndex && 元素行 != 元素行列表.Last())
                {
                    int lineIndex = 元素行列表.IndexOf(元素行);
                    目标元素行 = 元素行列表[lineIndex + 1];
                    indexInLine = 0;
                }
                break;
            }
            charIndex += 元素行.元素列表.Count;
        }
        目标元素行?.MoveCaretTo(indexInLine);
    }

    public void 输入文本(string text, 元素行 sender)
    {
        // 计算相对于段落的光标索引
        int indexInParagraph = 获取段落光标索引(sender);
        // 找到当前索引所在的子元素集，并插入文本
        行内元素集? 集 = null;
        int index = 0;
        foreach (var 元素集 in 元素集列表)
        {
            // 跳过内嵌元素
            if (元素集.InnerElement)
            {
                index++;
                continue;
            }
            // 计算当前元素的索引范围
            int startIndex = index;
            int endIndex = startIndex + 元素集.Length;
            // 判断索引是否在此元素集内
            if (startIndex <= indexInParagraph && indexInParagraph <= endIndex)
            {
                集 = 元素集;
                集.Text = 集.Text.Insert(indexInParagraph - startIndex, text);
                break;
            }
            index += 元素集.Length;
        }
        // 重新生成元素集的字元素
        集.行内元素列表.Clear();
        生成字元素(集);
        // 处理元素更新
        处理元素更新();
        // 更新光标位置
        indexInParagraph += text.Length;
        移动光标至(indexInParagraph);
        // 更新文本
        更新文本与内嵌元素(_全部行内元素);
    }

    public void 删除前字符(元素行 sender)
    {
        _光标索引 = 获取段落光标索引(sender);
        // 找到当前索引所在的子元素集，并删除字符
        行内元素集? 集 = null;
        int index = 0;
        foreach (var 元素集 in 元素集列表)
        {
            // 跳过内嵌元素
            if (元素集.InnerElement)
            {
                index++;
                continue;
            }
            // 计算当前元素的索引范围
            int startIndex = index;
            int endIndex = startIndex + 元素集.Length;
            // 判断索引是否在此元素集内
            if (startIndex <= _光标索引 && _光标索引 <= endIndex)
            {
                集 = 元素集;
                集.Text = 集.Text.Remove(_光标索引 - startIndex - 1, 1);
                break;
            }
            index += 元素集.Length;
        }
        // 重新生成元素集的字元素
        集.行内元素列表.Clear();
        生成字元素(集);
        // 处理元素更新
        处理元素更新();
        // 更新光标位置
        _光标索引--;
        移动光标至(_光标索引);
    }

    public void 删除前元素(元素行 sender)
    {
        _光标索引 = 获取段落光标索引(sender);
        _全部行内元素.RemoveAt(_光标索引 - 1);
        更新文本与内嵌元素(_全部行内元素);
        Init();
        // 处理元素更新
        处理元素更新();
        // 更新光标位置
        _光标索引--;
        移动光标至(_光标索引);
    }

    public void 处理退格(元素行 sender)
    {
        _光标索引 = 获取段落光标索引(sender);
        if (_光标索引 == 0)
        {
            if (OwnerPage != null) OwnerPage.合并段落(this);
            else if (Parent is 单元格 单元格)
            {

            }
            return;
        }
        行内元素 前元素 = _全部行内元素[_光标索引 - 1];
        if (前元素 is 字) 删除前字符(sender);
        else
        {
            // 获取高亮元素
            行内元素? 高亮 = 获取高亮元素();
            if (高亮 == 前元素)
            {
                更新高亮元素(null);
                删除前元素(sender);
            }
            else
            {
                更新高亮元素(前元素);
                int lineIndex = 元素行列表.IndexOf(sender);
                元素行 上一行 = 元素行列表[lineIndex - 1];
                if (OwnerPage != null) OwnerPage.更新当前元素行(上一行);
                else this.获取根段落().OwnerPage.更新当前元素行(上一行);
                上一行.UpdateCaretIndex(前元素, ElementSide.Right);
            }
        }
    }

    public bool 前元素已高亮(行内元素 前元素)
    {
        行内元素? 高亮 = 获取高亮元素();
        if (高亮 == null) return false;
        return 高亮 == 前元素;
    }

    public void 更新高亮元素(行内元素? 元素)
    {
        if (OwnerPage != null) OwnerPage.更新高亮元素(元素);
        else this.获取根段落().OwnerPage.更新高亮元素(元素);
    }

    public void 处理回车(元素行 sender)
    {
        // 处理逻辑
        //     光标在段首：在当前段落后插入新段落，然后移动元素和光标至新段落
        //     光标在段中：从光标处分割段落，然后移动光标至分割出来的段落开头
        //     光标在段尾：在当前段落后插入空段落，并移动光标至空段落

        _光标索引 = 获取段落光标索引(sender);
        // 因为段落列表是在父级管理的，所以为方便操作，将回车处理移交给父级
        if (OwnerPage != null)
        {
            OwnerPage.处理回车(this);
        }
        else if (Parent is 单元格 单元格)
        {

        }
        else if (Parent is 图片 图片)
        {
            // 父级为图片，表示此段落是图注，而图注只允许单个段落
            return;
        }
    }

    public void 插入图片(List<图片> list, 元素行 sender)
    {
        // 获取光标索引，然后插入元素
        _光标索引 = 获取段落光标索引(sender);
        _全部行内元素.InsertRange(_光标索引, list);
        // 更新文本与内嵌元素列表
        更新文本与内嵌元素(_全部行内元素);
        // 重新初始化
        Init();
        // 处理元素更新
        处理元素更新();
        // 更新光标位置
        _光标索引 += list.Count;
        移动光标至(_光标索引);
    }

    public void 插入表格(表格 表格, 元素行 sender)
    {
        // 获取光标索引，然后插入元素
        _光标索引 = 获取段落光标索引(sender);
        _全部行内元素.Insert(_光标索引, 表格);
        // 更新文本与内嵌元素列表
        更新文本与内嵌元素(_全部行内元素);
        // 重新初始化
        Init();
        // 处理元素更新
        处理元素更新();
        // 更新光标位置
        _光标索引++;
        移动光标至(_光标索引);
    }

    public void 插入公式(公式 公式, 元素行 sender)
    {
        // 获取光标索引，然后插入元素
        _光标索引 = 获取段落光标索引(sender);
        _全部行内元素.Insert(_光标索引, 公式);
        // 更新文本与内嵌元素列表
        更新文本与内嵌元素(_全部行内元素);
        // 重新初始化
        Init();
        // 处理元素更新
        处理元素更新();
        // 更新光标位置
        _光标索引++;
        移动光标至(_光标索引);
    }

    #endregion

    #region 更新属性

    public void 更新字体(string font)
    {

    }

    public void 更新字号(int size)
    {
        字号 = size;
        更新文本与内嵌元素(_全部行内元素);
        Init();
        处理元素更新();
        移动光标至(_光标索引);
    }

    public void 更新水平对齐方式(水平对齐方式 水平对齐)
    {
        this.水平对齐 = 水平对齐;
        处理元素更新();
        移动光标至(_光标索引);
    }

    public void 更新垂直对齐方式(垂直对齐方式 垂直对齐)
    {
        this.垂直对齐 = 垂直对齐;
        处理元素更新();
        移动光标至(_光标索引);
    }

    public void 更新使用自定义首行缩进(bool use)
    {
        使用自定义首行缩进 = use;
        处理元素更新();
        移动光标至(_光标索引);
    }

    public void 更新自定义首行缩进(double indent)
    {
        自定义首行缩进 = indent;
        处理元素更新();
        移动光标至(_光标索引);
    }

    public void 更新左缩进(double indent)
    {
        左缩进 = indent;
        处理元素更新();
        移动光标至(_光标索引);
    }

    public void 更新右缩进(double indent)
    {
        右缩进 = indent;
        处理元素更新();
        移动光标至(_光标索引);
    }

    #endregion

    #region 私有方法

    private void 约束()
    {
        // 设置了固定宽度，给全部行内元素设置最大宽度
        if (!double.IsNaN(Width))
        {
            foreach (var 行内元素 in _全部行内元素)
                行内元素.MaxWidth = Width - 左缩进 - 右缩进;
            return;
        }
        // 设置了最大宽度，也给全部行内元素设置最大宽度
        if (!double.IsNaN(MaxWidth))
        {
            foreach (var 行内元素 in _全部行内元素)
                行内元素.MaxWidth = MaxWidth - 左缩进 - 右缩进;
            return;
        }
        throw new Exception("段落必须设置一个宽度");
    }

    private void 生成字元素(行内元素集 元素集)
    {
        // 生成元素集的字元素
        foreach (var c in 元素集.Text)
        {
            字 element = new 字
            {
                字符 = c,
                字体 = 字体,
                字号 = 字号
            };
            element.Init();
            元素集.行内元素列表.Add(element);
        }
        // 生成中英文间距
        元素集.生成中英文间距();
    }

    private 行内元素 获取一个内嵌元素()
    {
        if (内嵌元素列表.Count == 0) throw new Exception("没有可用的内嵌元素");
        行内元素 result = 内嵌元素列表[0];
        内嵌元素列表.RemoveAt(0);
        return result;
    }

    /// <summary>
    /// 获取填充元素宽度。填充宽度的意思是填充元素用到的宽度，不等于元素行的最终宽度
    /// </summary>
    private double 获取填充元素宽度()
    {
        double 宽度 = double.NaN;
        // 设置了固定宽度，则以固定宽度为准
        if (!double.IsNaN(Width)) 宽度 = Width;
        // 否则，以最大宽度为准
        else if (!double.IsNaN(MaxWidth)) 宽度 = MaxWidth;
        if (double.IsNaN(宽度)) throw new Exception("未设置段落宽度");
        // 减去左右缩进
        宽度 -= 左缩进 + 右缩进;
        // 如果是首行
        if (元素行列表.Count == 0) 宽度 -= 获取首行缩进();
        return 宽度;
    }

    private double 获取首行缩进()
    {
        // 返回自定义首行缩进
        if (使用自定义首行缩进) return 自定义首行缩进;
        // 返回页面首行缩进
        return 首行缩进;
    }

    private List<行内元素> 获取全部行内元素()
    {
        List<行内元素> result = new List<行内元素>();
        foreach (var 集 in 元素集列表)
            result.AddRange(集.行内元素列表);
        return result;
    }

    private int 获取段落光标索引(元素行 sender)
    {
        int result = 0;
        foreach (var 元素行 in 元素行列表)
        {
            if (元素行 == sender)
            {
                result += sender.光标索引;
                break;
            }
            result += 元素行.元素列表.Count;
        }
        return result;
    }

    private void 处理元素更新()
    {
        // 更新全部行内元素列表
        _全部行内元素 = 获取全部行内元素();
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
        else
        {
            // 通知页面重新测量
            if (OwnerPage != null) OwnerPage.重新测量(this);
            // 通知父元素重新测量与排列
            else Parent?.重新测量();
        }
    }

    private 行内元素? 获取高亮元素()
    {
        if (OwnerPage != null) return OwnerPage.获取高亮元素();
        return this.获取根段落().OwnerPage.获取高亮元素();
    }

    #endregion

    #region 字段

    private readonly string _占位标记 = "<ele>";

    private List<行内元素> _全部行内元素 = new List<行内元素>();

    private int _光标索引 = 0;

    #endregion
}