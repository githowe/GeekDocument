using GeekDocument.SubSystem.EditerSystem.Define;
using GeekDocument.SubSystem.LayoutEngine.Tool;
using GeekDocument.SubSystem.OptionSystem;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GeekDocument.SubSystem.LayoutEngine
{
    public class 元素行 : 布局元素
    {
        #region 构造方法

        public 元素行()
        {
            Name = "行";
            Icon = "Line";
            _keyHandler = new STElementLine(this);
            _keyHandler.Init();
        }

        #endregion

        #region 属性

        public double Left { get; set; } = double.NaN;

        public double Top { get; set; } = double.NaN;

        public double Width { get; set; } = double.NaN;

        public double MaxWidth { get; set; } = double.NaN;

        public double ActualWidth { get; set; } = double.NaN;

        public double ActualHeight { get; set; } = double.NaN;

        public double 字号 { get; set; } = 16;

        public 水平对齐方式 水平对齐 { get; set; } = 水平对齐方式.Justify;

        public 垂直对齐方式 垂直对齐 { get; set; } = 垂直对齐方式.Bottom;

        public bool 首行 { get; set; } = false;

        public double 首行缩进 { get; set; } = 0;

        #endregion

        #region 运行时属性

        public List<行内元素> 元素列表 { get; set; } = new List<行内元素>();

        #endregion

        #region 布局元素方法

        public override List<绘图对象> 获取绘图对象()
        {
            List<绘图对象> result = new List<绘图对象>();
            _绘图对象.Name = GetPath();
            result.Add(_绘图对象);
            foreach (var item in 元素列表)
                result.AddRange(item.获取绘图对象());
            return result;
        }

        public override 命中信息? 获取命中信息(Point point)
        {
            命中信息? result = null;
            // 先获取行内元素的命中信息
            for (int index = 元素列表.Count - 1; index >= 0; index--)
            {
                行内元素? item = 元素列表[index];
                result = item.获取命中信息(point);
                if (result != null) return result;
            }
            // 获取自身的可命中区域
            Rect rect = new Rect(Left, Top, ActualWidth, ActualHeight);
            if (rect.Contains(point))
            {
                result = new 命中信息()
                {
                    坐标 = point,
                    命中元素 = this,
                    命中区域 = rect,
                    区域名称 = "元素行"
                };
            }
            // 返回命中信息
            return result;
        }

        public override 元素行 获取最近元素行(Point point)
        {
            if (元素列表.Count == 0) return this;

            // 获取第一个元素与最后一个元素
            行内元素 first = 元素列表[0];
            行内元素 last = 元素列表.Last();
            // 横坐标位于第一个元素左侧或最后一个元素右侧，返回自己
            if (point.X < first.Left || point.X >= last.Left + last.ActualWidth) return this;

            // 获取命中元素
            行内元素 元素 = 获取最近行内元素(point);
            // 命中元素可以输入，返回命中元素内的元素行
            if (元素.CanInput) return 元素.获取最近元素行(point);

            return this;
        }

        public override Rect GetViewRect() => new Rect(Left, Top, ActualWidth, ActualHeight);

        #endregion

        #region 布局元素核心方法

        public override void Init()
        {
            _rowLinePen.Freeze();
        }

        public override void 测量()
        {
            // 测量内容宽度
            double 内容宽度 = 0;
            foreach (var item in 元素列表)
                内容宽度 += item.LeftMargin + item.ActualWidth + item.RightMargin;
            if (元素列表.Count > 0)
            {
                内容宽度 -= 元素列表[0].LeftMargin;
                内容宽度 -= 元素列表.Last().RightMargin;
            }
            // 测量宽度
            if (!double.IsNaN(Width)) ActualWidth = Width;
            else
            {
                ActualWidth = 内容宽度;
                if (首行) ActualWidth += 首行缩进;
            }
            // 测量高度
            ActualHeight = 字号;
            foreach (var item in 元素列表)
                if (item.ActualHeight > ActualHeight) ActualHeight = item.ActualHeight;
        }

        public override void 重新测量() => Parent?.重新测量();

        public override void 排列()
        {
            if (double.IsNaN(Left) || double.IsNaN(Top)) throw new Exception("未设置元素行坐标");
            更新元素横坐标();
            更新元素纵坐标();
            foreach (var item in 元素列表) item.排列();
        }

        public override void 渲染(DrawingContext? dc)
        {
            // 元素行使用自己的绘图上下文
            DrawingContext self_dc = _绘图对象.RenderOpen();
            // 绘制行线
            if (Options.Instance.View.ShowRowLine)
            {
                self_dc.DrawLine(_rowLinePen, new Point(Left, Top + 0.5), new Point(Left + ActualWidth, Top + 0.5));
                self_dc.DrawLine(_rowLinePen, new Point(Left, Top + ActualHeight - 0.5), new Point(Left + ActualWidth, Top + ActualHeight - 0.5));
            }
            // 绘制元素
            foreach (var item in 元素列表) item.渲染(self_dc);
            self_dc.Close();
        }

        #endregion

        #region 光标处理

        public int 光标索引 { get; set; } = 0;

        public 光标信息 移动光标(Point point)
        {
            if (元素列表.Count == 0)
            {
                光标索引 = 0;
                return 获取空行光标信息();
            }

            // 获取第一个元素与最后一个元素
            行内元素 first = 元素列表[0];
            行内元素 last = 元素列表.Last();
            // 横坐标位于第一个元素左侧
            if (point.X < first.Left)
            {
                光标索引 = 0;
                return 移动光标至元素左侧(first);
            }
            // 横坐标位于最后一个元素右侧
            if (point.X >= last.Left + last.ActualWidth)
            {
                光标索引 = 元素列表.Count;
                return 移动光标至元素右侧(last);
            }

            // 获取命中元素
            行内元素 命中元素 = 获取最近行内元素(point);
            // 命中元素可以输入，则进一步获取元素内部的光标位置
            if (命中元素.CanInput)
            {
                元素行 元素行 = 命中元素.获取最近元素行(point);
                return 元素行.移动光标(point);
            }

            int elementIndex = 元素列表.IndexOf(命中元素);
            Rect elementRect = new Rect(命中元素.Left, 命中元素.Top, 命中元素.ActualWidth, 命中元素.ActualHeight);
            // 命中元素左半部分
            if (point.X < elementRect.Left + elementRect.Width / 2)
            {
                光标索引 = elementIndex;
                return 移动光标至元素左侧(命中元素);
            }
            // 命中元素右半部分
            else
            {
                光标索引 = elementIndex + 1;
                return 移动光标至元素右侧(命中元素);
            }
        }

        public void MoveInCaretToStart() => MoveCaretToStart(ElementSide.Left);

        public void MoveInCaretToEnd() => MoveCaretToEnd(ElementSide.Right);

        public override void 从开头移出光标(布局元素 元素)
        {
            行内元素? 行内元素 = 元素 as 行内元素;
            if (行内元素 == null) throw new Exception("没有从行内元素移出光标");

            页面? page = this.获取根段落().OwnerPage;
            page?.更新当前元素行(this);
            光标索引 = 元素列表.IndexOf(行内元素);
            光标信息 info = 移动光标至元素左侧(行内元素);
            page?.移动光标(info.X, info.Y, info.Height);
        }

        public override void 从末尾移出光标(布局元素 元素)
        {
            行内元素? 行内元素 = 元素 as 行内元素;
            if (行内元素 == null) throw new Exception("没有从行内元素移出光标");

            页面? page = this.获取根段落().OwnerPage;
            page?.更新当前元素行(this);
            光标索引 = 元素列表.IndexOf(行内元素) + 1;
            光标信息 info = 移动光标至元素右侧(行内元素);
            page?.移动光标(info.X, info.Y, info.Height);
        }

        /// <summary>
        /// 移动光标至第一个元素
        /// </summary>
        public void MoveCaretToStart(ElementSide side)
        {
            // 当前行没有元素
            if (元素列表.Count == 0)
            {
                EmptyLineMoveCaret();
                return;
            }

            页面? page = this.获取根段落().OwnerPage;
            // 获取第一个元素
            行内元素 first = 元素列表[0];
            // 移动至第一个元素左侧
            if (side == ElementSide.Left)
            {
                光标索引 = 0;
                page?.更新当前元素行(this);
                光标信息 info = 移动光标至元素左侧(first);
                page?.移动光标(info.X, info.Y, info.Height);
                return;
            }
            // 第一个元素不支持输入
            if (!first.CanInput)
            {
                光标索引 = 1;
                page?.更新当前元素行(this);
                光标信息 info = 移动光标至元素右侧(first);
                page?.移动光标(info.X, info.Y, info.Height);
                return;
            }
            // 支持输入，移入光标至元素开头
            first.移入光标至开头();
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

            页面? page = this.获取根段落().OwnerPage;
            // 获取最后一个元素
            行内元素 last = 元素列表.Last();
            // 移动至最后一个元素右侧
            if (side == ElementSide.Right)
            {
                光标索引 = 元素列表.Count;
                // 更新页面的当前元素行
                page?.更新当前元素行(this);
                光标信息 info = 移动光标至元素右侧(last);
                page?.移动光标(info.X, info.Y, info.Height);
                return;
            }
            // 最后一个元素不支持输入
            if (!last.CanInput)
            {
                光标索引 = 元素列表.Count - 1;
                // 移动至最后一个元素左侧
                page?.更新当前元素行(this);
                光标信息 info = 移动光标至元素左侧(last);
                page?.移动光标(info.X, info.Y, info.Height);
                return;
            }
            // 支持输入，移入光标至元素末尾
            last.移入光标至末尾();
        }

        /// <summary>
        /// 移动光标至指定索引
        /// </summary>
        public void MoveCaretTo(int index)
        {
            // 没有元素
            if (元素列表.Count == 0)
            {
                EmptyLineMoveCaret();
                return;
            }

            光标索引 = index;
            页面? page = this.获取根段落().OwnerPage;
            page?.更新当前元素行(this);
            // 光标在最后一个元素前
            if (光标索引 < 元素列表.Count)
            {
                行内元素 target = 元素列表[光标索引];
                光标信息 info = 移动光标至元素左侧(target);
                page?.移动光标(info.X, info.Y, info.Height);
            }
            // 光标在行尾
            else if (光标索引 == 元素列表.Count)
            {
                行内元素 target = 元素列表[光标索引 - 1];
                光标信息 info = 移动光标至元素右侧(target);
                page?.移动光标(info.X, info.Y, info.Height);
            }
        }

        /// <summary>
        /// 空行移动光标：直接移动至行首
        /// </summary>
        private void EmptyLineMoveCaret()
        {
            // 更新页面的当前元素行
            页面? page = this.获取根段落().OwnerPage;
            page?.更新当前元素行(this);
            光标索引 = 0;
            // 移动光标
            光标信息 info = 获取空行光标信息();
            page?.移动光标(info.X, info.Y, info.Height);
        }

        private 光标信息 获取空行光标信息()
        {
            光标信息 result = new 光标信息();
            Rect lineRect = new Rect(Left, Top, ActualWidth, ActualHeight);

            // 横坐标根据水平对齐计算
            switch (水平对齐)
            {
                case 水平对齐方式.Left:
                    result.X = lineRect.Left;
                    if (首行) result.X += 首行缩进;
                    break;
                case 水平对齐方式.Center:
                    result.X = lineRect.Left + lineRect.Width / 2;
                    break;
                case 水平对齐方式.Right:
                    result.X = lineRect.Right;
                    break;
                case 水平对齐方式.Justify:
                    result.X = lineRect.Left;
                    if (首行) result.X += 首行缩进;
                    break;
            }
            // 纵坐标取顶部
            result.Y = lineRect.Top;
            // 高度取字号
            result.Height = 字号;

            return result;
        }

        private 光标信息 移动光标至元素左侧(行内元素 元素)
        {
            光标信息 result = new 光标信息();

            // 获取当前元素索引
            int elementIndex = 元素列表.IndexOf(元素);

            // 无前一个元素
            if (elementIndex == 0)
            {
                // 横坐标取当前元素左
                result.X = 元素.Left;
                // 纵坐标根据垂直对齐与字号计算
                result.Y = 计算光标纵坐标();
                // 高度取字号
                result.Height = 字号;
                // 返回结果
                return result;
            }

            // 获取前一个元素以及前一个元素区域
            行内元素 前一个元素 = 元素列表[elementIndex - 1];
            Rect prevRect = new Rect(前一个元素.Left, 前一个元素.Top, 前一个元素.ActualWidth, 前一个元素.ActualHeight);
            // 前一个元素为字元素
            if (前一个元素 is 字)
            {
                // 坐标取前一个元素右上角 + 右间距
                result.X = prevRect.Right + 前一个元素.RightMargin + 前一个元素.RightExtend;
                result.Y = prevRect.Top;
                // 高度取前一个元素高度
                result.Height = 前一个元素.ActualHeight;
            }
            // 其他元素
            else
            {
                // 横坐标取前一个元素右 + 右间距
                result.X = prevRect.Right + 前一个元素.RightMargin + 前一个元素.RightExtend;
                // 纵坐标根据垂直对齐与字号计算
                result.Y = 计算光标纵坐标();
                // 高度取字号
                result.Height = 字号;
            }

            return result;
        }

        private 光标信息 移动光标至元素右侧(行内元素 元素)
        {
            光标信息 result = new 光标信息();
            Rect elementRect = new Rect(元素.Left, 元素.Top, 元素.ActualWidth, 元素.ActualHeight);

            // 当前元素为字元素
            if (元素 is 字)
            {
                // 坐标取当前字右上角
                result.X = elementRect.Right + 元素.RightMargin + 元素.RightExtend;
                result.Y = elementRect.Top;
                // 高度取当前字高度
                result.Height = 元素.ActualHeight;
            }
            // 其他元素
            {
                // 横坐标取当前元素右
                result.X = elementRect.Right + 元素.RightMargin + 元素.RightExtend;
                // 纵坐标根据垂直对齐与字号计算
                result.Y = 计算光标纵坐标();
                // 高度取字号
                result.Height = 字号;
            }

            return result;
        }

        private double 计算光标纵坐标()
        {
            Rect lineRect = new Rect(Left, Top, ActualWidth, ActualHeight);
            return 垂直对齐 switch
            {
                垂直对齐方式.Top => lineRect.Top,
                垂直对齐方式.Center => lineRect.Top + (ActualHeight - 字号) / 2,
                垂直对齐方式.Bottom => lineRect.Bottom - 字号,
                _ => 0,
            };
        }

        #endregion

        #region 按键处理器接口

        public bool 光标前有元素() => 光标索引 > 0;

        public bool 前元素支持输入() => 元素列表[光标索引 - 1].CanInput;

        public void 移入光标至前元素末尾()
        {
            元素列表[光标索引 - 1].移入光标至末尾();
        }

        public void 前移光标()
        {
            光标索引--;
            光标信息 info = 移动光标至元素左侧(元素列表[光标索引]);
            页面? page = this.获取根段落().OwnerPage;
            page?.移动光标(info.X, info.Y, info.Height);
        }

        public void 调用所属段落的左移光标()
        {
            if (Parent is 段落 段落) 段落.左移光标(this);
        }

        public bool 光标后有元素() => 光标索引 < 元素列表.Count;

        public bool 当前元素支持输入() => 元素列表[光标索引].CanInput;

        public void 移入光标至当前元素开头()
        {
            元素列表[光标索引].移入光标至开头();
        }

        public void 后移光标()
        {
            光标信息 info = 移动光标至元素右侧(元素列表[光标索引]);
            页面? page = this.获取根段落().OwnerPage;
            page?.移动光标(info.X, info.Y, info.Height);
            光标索引++;
        }

        public void 调用所属段落的右移光标()
        {
            if (Parent is 段落 段落) 段落.右移光标(this);
        }

        public bool 光标前为字元素() => 元素列表[光标索引 - 1] is 字;

        public void 删除前字符()
        {
            ((段落)Parent).删除前字符(this);
        }

        public bool 前元素未高亮()
        {
            return false;
        }

        public void 高亮前元素()
        {

        }

        public void 删除前元素()
        {

        }

        public void 调用所属段落的退格()
        {
            ((段落)Parent).处理退格(this);
        }

        public void 处理回车()
        {
            ((段落)Parent).处理回车(this);
        }

        #endregion

        public void HandleEditKey(EditKey key) => _keyHandler.HandleEditKey(key);

        public void HandleCtrlEditKey(Key key)
        {
            switch (key)
            {
                // 全选
                case Key.A:
                    break;
                // 剪切
                case Key.X:
                    break;
                // 复制
                case Key.C:
                    break;
                // 粘贴
                case Key.V:
                    string text = Clipboard.GetText();
                    if (string.IsNullOrEmpty(text)) return;
                    HandleTextInput(text);
                    break;
                // 撤销
                case Key.Z:
                    break;
                // 重做
                case Key.Y:
                    break;
                // 保存
                case Key.S:
                    break;
                // 回车
                case Key.Enter:
                    HandleEditKey(EditKey.Enter);
                    break;
            }
        }

        public void HandleTextInput(string text)
        {
            // 忽略空字符、退格、回车、Esc
            if (text is "" or "\b" or "\r" or "\u001b") return;
            // 将制表符转换为空格
            text = text.Replace("\t", "    ");
            // 统一换行符
            text = text.Replace("\r\n", "\n");

            ((段落)Parent).输入文本(text, this);
        }

        private void 更新元素横坐标()
        {
            foreach (var item in 元素列表) item.RightExtend = 0;
            switch (水平对齐)
            {
                case 水平对齐方式.Left:
                    左对齐元素();
                    break;
                case 水平对齐方式.Center:
                    居中对齐元素();
                    break;
                case 水平对齐方式.Right:
                    右对齐元素();
                    break;
                case 水平对齐方式.Justify:
                    // 全是空白，按左对齐处理
                    if (全是空白()) 左对齐元素();
                    else 两端对齐元素();
                    break;
            }
        }

        private void 更新元素纵坐标()
        {
            switch (垂直对齐)
            {
                case 垂直对齐方式.Top:
                    foreach (var item in 元素列表) item.Top = Top;
                    break;
                case 垂直对齐方式.Center:
                    foreach (var item in 元素列表) item.Top = Top + (ActualHeight - item.ActualHeight) / 2;
                    break;
                case 垂直对齐方式.Bottom:
                    foreach (var item in 元素列表) item.Top = Top + ActualHeight - item.ActualHeight;
                    break;
            }
        }

        private void 左对齐元素()
        {
            // 规则：
            //     有首行缩进时，从缩进位置开始排列
            //     忽略第一个非空白元素的左边距

            行状态 状态 = 行状态.空;
            double 横坐标 = Left;
            if (首行) 横坐标 += 首行缩进;
            foreach (var 元素 in 元素列表)
            {
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
            }
        }

        private void 居中对齐元素()
        {
            // 规则：
            //     忽略首行缩进
            //     忽略首尾的连续空白元素
            //     忽略第一个非空白元素的左边距
            //     忽略最后一个非空白元素的右边距

            // 获取去除首尾连续空白元素后的元素列表
            List<行内元素> 可视元素 = 获取可伸缩部分元素();
            // 计算可视元素宽度
            double 可视元素宽度 = 0;
            foreach (var 元素 in 可视元素)
                可视元素宽度 += 元素.LeftMargin + 元素.ActualWidth + 元素.RightMargin;
            if (可视元素.Count > 0)
            {
                可视元素宽度 -= 可视元素[0].LeftMargin;
                可视元素宽度 -= 可视元素.Last().RightMargin;
            }
            // 计算可视元素坐标
            List<double> 可视元素坐标 = new List<double>();
            double 横坐标 = Left + (ActualWidth - 可视元素宽度) / 2;
            foreach (var 元素 in 可视元素)
            {
                if (可视元素坐标.Count == 0)
                {
                    可视元素坐标.Add(横坐标);
                    横坐标 += 元素.ActualWidth + 元素.RightMargin;
                }
                else
                {
                    横坐标 += 元素.LeftMargin;
                    可视元素坐标.Add(横坐标);
                    横坐标 += 元素.ActualWidth + 元素.RightMargin;
                }
            }
            // 计算可视元素的坐标范围
            double left = Left + ActualWidth / 2;
            double right = Left + ActualWidth / 2;
            if (可视元素坐标.Count > 0)
            {
                left = 可视元素坐标[0];
                right = 可视元素坐标.Last() + 可视元素.Last().ActualWidth;
            }
            // 添加头部空白元素
            List<行内元素> 头部空白元素 = 获取头部空白();
            for (int index = 头部空白元素.Count - 1; index >= 0; index--)
            {
                left -= 头部空白元素[index].ActualWidth;
                可视元素坐标.Insert(0, left);
            }
            // 添加尾部空白元素
            List<行内元素> 尾部空白元素 = 获取尾部空白();
            foreach (var item in 尾部空白元素)
            {
                可视元素坐标.Add(right);
                right += item.ActualWidth;
            }
            // 设置所有元素坐标
            for (int index = 0; index < 元素列表.Count; index++)
                元素列表[index].Left = 可视元素坐标[index];
        }

        private void 右对齐元素()
        {
            // 规则：忽略最后一个非空白元素的右边距

            // 获取去除首尾连续空白元素后的元素列表
            List<行内元素> 可视元素 = 获取可伸缩部分元素();
            // 计算可视元素宽度
            double 可视元素宽度 = 0;
            foreach (var 元素 in 可视元素)
                可视元素宽度 += 元素.LeftMargin + 元素.ActualWidth + 元素.RightMargin;
            if (可视元素.Count > 0)
                可视元素宽度 -= 可视元素.Last().RightMargin;
            // 计算可视元素坐标
            List<double> 可视元素坐标 = new List<double>();
            double 横坐标 = Left + ActualWidth - 可视元素宽度;
            foreach (var 元素 in 可视元素)
            {
                横坐标 += 元素.LeftMargin;
                可视元素坐标.Add(横坐标);
                横坐标 += 元素.ActualWidth + 元素.RightMargin;
            }
            // 计算可视元素的坐标范围
            double left = Left + ActualWidth;
            double right = Left + ActualWidth;
            if (可视元素坐标.Count > 0) left = 可视元素坐标[0];
            // 添加头部空白元素
            List<行内元素> 头部空白元素 = 获取头部空白();
            for (int index = 头部空白元素.Count - 1; index >= 0; index--)
            {
                left -= 头部空白元素[index].ActualWidth;
                可视元素坐标.Insert(0, left);
            }
            // 添加尾部空白元素
            List<行内元素> 尾部空白元素 = 获取尾部空白();
            foreach (var item in 尾部空白元素)
            {
                可视元素坐标.Add(right);
                right += item.ActualWidth;
            }
            // 设置所有元素坐标
            for (int index = 0; index < 元素列表.Count; index++)
                元素列表[index].Left = 可视元素坐标[index];
        }

        private bool 全是空白()
        {
            foreach (var item in 元素列表)
                if (!item.IsSpace) return false;
            return true;
        }

        private void 两端对齐元素()
        {
            // 计算容器宽度。容器宽度 = 行宽 - 头部空白宽度
            double 容器宽度 = ActualWidth - 获取头部空白宽度();
            if (首行) 容器宽度 -= 首行缩进;
            // 获取可伸缩部分元素
            List<行内元素> 可伸缩部分 = 获取可伸缩部分元素();
            // 计算未压缩宽度
            double 未压缩宽度 = 0;
            foreach (var 元素 in 可伸缩部分)
                未压缩宽度 += 元素.LeftMargin + 元素.ActualWidth + 元素.RightMargin;
            if (可伸缩部分.Count > 0)
            {
                未压缩宽度 -= 可伸缩部分[0].LeftMargin;
                未压缩宽度 -= 可伸缩部分.Last().RightMargin;
            }
            // 执行拉伸
            if (未压缩宽度 < 容器宽度 && 可伸缩部分.Count > 1)
            {
                double 总拉伸量 = 容器宽度 - 未压缩宽度;
                double 平均拉伸量 = 总拉伸量 / (可伸缩部分.Count - 1);
                for (int index = 0; index < 可伸缩部分.Count - 1; index++)
                    可伸缩部分[index].RightExtend = 平均拉伸量;
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
            // 横向排列元素
            行状态 状态 = 行状态.空;
            double 横坐标 = Left;
            if (首行) 横坐标 += 首行缩进;
            foreach (var 元素 in 元素列表)
            {
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
                            横坐标 += 元素.ActualWidth + 元素.RightMargin + 元素.RightExtend;
                            状态 = 行状态.填充元素;
                        }
                        break;
                    case 行状态.填充空格:
                        元素.Left = 横坐标;
                        if (元素.IsSpace) 横坐标 += 元素.ActualWidth;
                        else
                        {
                            横坐标 += 元素.ActualWidth + 元素.RightMargin + 元素.RightExtend;
                            状态 = 行状态.填充元素;
                        }
                        break;
                    case 行状态.填充元素:
                        横坐标 += 元素.LeftMargin;
                        元素.Left = 横坐标;
                        横坐标 += 元素.ActualWidth + 元素.RightMargin + 元素.RightExtend;
                        break;
                }
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
                行内元素 元素 = 元素列表[index];
                if (元素.IsSpace) 结果 += 元素.ActualWidth;
                // 遇到非空白元素时，退出循环
                else break;
            }
            return 结果;
        }

        public List<行内元素> 获取头部空白()
        {
            List<行内元素> result = new List<行内元素>();
            foreach (var item in 元素列表)
            {
                if (item.IsSpace) result.Add(item);
                else break;
            }
            return result;
        }

        public List<行内元素> 获取尾部空白()
        {
            List<行内元素> result = new List<行内元素>();
            for (int index = 元素列表.Count - 1; index >= 0; index--)
            {
                行内元素 item = 元素列表[index];
                if (item.IsSpace) result.Add(item);
                else break;
            }
            result.Reverse();
            return result;
        }

        private List<行内元素> 获取可伸缩部分元素()
        {
            // 可伸缩部分元素 = 全部元素 - 头部空白元素 - 尾部空白元素

            List<行内元素> 结果 = new List<行内元素>(元素列表);
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

        private 行内元素 获取最近行内元素(Point point)
        {
            行内元素? 命中元素 = null;

            // 先通过横坐标获取命中元素
            for (int index = 元素列表.Count - 1; index >= 0; index--)
            {
                行内元素 元素 = 元素列表[index];
                Rect rect = new Rect(元素.Left, 元素.Top, 元素.ActualWidth, 元素.ActualHeight);
                if (rect.Left <= point.X && point.X < rect.Right)
                {
                    命中元素 = 元素;
                    break;
                }
            }
            // 无命中，可能点到了元素间的左右间距
            if (命中元素 == null)
            {
                命中元素 = 元素列表[0];
                double 最小距离 = double.MaxValue;
                foreach (var 元素 in 元素列表)
                {
                    Rect viewRect = new Rect(元素.Left, 元素.Top, 元素.ActualWidth, 元素.ActualHeight);
                    double distance = Math.Min(Math.Abs(point.X - viewRect.Left), Math.Abs(point.X - viewRect.Right));
                    if (distance < 最小距离)
                    {
                        最小距离 = distance;
                        命中元素 = 元素;
                    }
                }
            }

            return 命中元素;
        }

        private 绘图对象 _绘图对象 = new 绘图对象();
        private readonly Pen _rowLinePen = new Pen(new SolidColorBrush(Color.FromArgb(32, 255, 255, 255)), 1);

        private readonly STElementLine _keyHandler;
    }
}