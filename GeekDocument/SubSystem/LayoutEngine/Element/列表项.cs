using System.Windows;
using System.Windows.Media;

namespace GeekDocument.SubSystem.LayoutEngine.Element
{
    public class 列表项 : 布局元素
    {
        #region 构造方法

        public 列表项()
        {
            Name = "列表项";
            Icon = "ListItem";
        }

        #endregion

        #region 属性

        public int Deep { get; set; } = 0;

        public double Width { get; set; } = double.NaN;

        public double ActualWidth { get; set; } = double.NaN;

        public double ActualHeight { get; set; } = double.NaN;

        public double Left { get; set; } = double.NaN;

        public double Top { get; set; } = double.NaN;

        public 段落 段落 { get; set; } = new 段落();

        public double 行间距 { get; set; } = 8;

        public double 缩进 { get; set; } = 32;

        public double MarkSize { get; set; } = 4;

        public bool IsEmpty => 段落.全部行内元素.Count == 0;

        #endregion

        #region 布局元素方法

        public override void Init()
        {
            段落.更新文本与内嵌元素(段落.全部行内元素);
            段落.禁用缩进 = true;
            段落.Init();
            AddChild(段落);
            _markBack.Freeze();
        }

        public override void 测量()
        {
            if (double.IsNaN(Width)) throw new Exception("列表项未设置宽度");

            段落.Width = Width - 缩进 * Deep;
            段落.测量();

            ActualWidth = Width;
            ActualHeight = 0;
            ActualHeight += 段落.ActualHeight;
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
            段落.Left = Left + 缩进 * Deep;
            段落.Top = Top;
            段落.排列();
        }

        public override void 渲染(DrawingContext? dc)
        {
            // 绘制标记
            DrawingContext self_dc = _绘图对象.RenderOpen();
            double mark_x = 段落.Left - 缩进 + (缩进 - MarkSize) / 2;
            double mark_y = Top + (段落.字号 - MarkSize) / 2;
            self_dc.DrawRectangle(_markBack, null, new Rect(mark_x, mark_y, MarkSize, MarkSize));
            self_dc.Close();
            // 渲染段落
            段落.渲染(dc);
        }

        public override List<绘图对象> 获取绘图对象()
        {
            List<绘图对象> result = new List<绘图对象>();
            result.Add(_绘图对象);
            result.AddRange(段落.获取绘图对象());
            return result;
        }

        public override Rect GetViewRect() => new Rect(Left, Top, ActualWidth, ActualHeight);

        public override void 从开头移出光标(布局元素 元素) => Parent?.从开头移出光标(this);

        public override void 从末尾移出光标(布局元素 元素) => Parent?.从末尾移出光标(this);

        #endregion

        #region object 方法

        public override string ToString()
        {
            int length = Math.Min(段落.文本.Length, 20);
            return 段落.文本.Substring(0, length);
        }

        #endregion

        #region 公开方法

        public void 移入光标至开头() => 段落.移动光标至开头();

        public void 移入光标至末尾() => 段落.移动光标至末尾();

        public void 移动光标至开头() => 段落.移动光标至开头();

        public void 移动光标至末尾() => 段落.移动光标至末尾();

        public void 处理退格() => ((列表)Parent).处理退格(this);

        public void 处理回车() => ((列表)Parent).处理回车(this);

        #endregion

        #region 私有方法


        #endregion

        #region 字段

        private readonly 绘图对象 _绘图对象 = new 绘图对象();
        private readonly Brush _markBack = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

        #endregion
    }
}