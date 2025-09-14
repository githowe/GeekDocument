using System.Windows;
using System.Windows.Media;

namespace GeekDocument.SubSystem.LayoutEngine.Element
{
    /// <summary>
    /// 字。一个字包含单个或多个字符
    /// </summary>
    public class 字 : 布局元素
    {
        #region 属性

        public 字类型 类型 { get; set; } = 字类型.Chinese;

        public string 文本 { get; set; } = "";

        /// <summary>字号列表。数量须与文本长度一致</summary>
        public List<double> 字号列表 { get; set; } = new List<double>();

        #endregion

        #region object 方法

        public override string ToString()
        {
            string type = IsSpace ? "空白" : 文本;
            return $"{type}：{Left}";
        }

        #endregion

        #region 布局元素 方法

        public override void Init()
        {
            if (文本 == " ") 类型 = 字类型.Space;
            if (类型 == 字类型.Space) IsSpace = true;
            if (类型 == 字类型.English) CanBreak = true;

            _space.Freeze();
            _text.Freeze();
            _widthChanged.Freeze();
        }

        public override void UpdateLayout()
        {
            // 注意：字元素忽略宽高限制

            // Todo：字符宽度应该根据字体与字号计算，这里简化为字号即字符宽度

            // 遍历字符
            int index = 0;
            ActualWidth = 0;
            ActualHeight = 字号列表[0];
            foreach (var item in 文本)
            {
                // 累加宽度
                ActualWidth += 字号列表[index];
                // 取最大高度
                if (字号列表[index] > ActualHeight) ActualHeight = 字号列表[index];
                index++;
            }
            _initWidth = ActualWidth;
        }

        public override double 压缩元素()
        {
            // 空白元素，最大可压缩一半
            if (IsSpace) return ActualWidth / 2;
            // 非空白元素，且存在右边距，最大可压缩右边距的一半
            if (RightMargin > 0) return ActualWidth + RightMargin / 2;
            // 其他情况不压缩
            return ActualWidth;
        }

        public override void 压缩至(double 比例)
        {
            if (IsSpace)
            {
                double max = ActualWidth / 2;
                ActualWidth -= max * 比例;
            }
            else if (RightMargin > 0)
            {
                double max = RightMargin / 2;
                RightMargin -= max * 比例;
            }
        }

        public override 布局元素 断开(double 最大宽度)
        {
            // 如果断开的是代码行，断开之后需要压缩左边距再判断有没有超出最大宽度

            // 创建断开后的布局元素
            字 断开 = new 字();
            while (true)
            {
                // Todo：这里应该加字符宽度，先用字号代替字符宽度

                // 当前宽度加上第一个字符宽度没有超过最大宽度时，将第一个字符加入断开部分
                if (断开.ActualWidth + 字号列表[0] < 最大宽度)
                {
                    // 分离一个字符
                    断开.文本 += 文本[0];
                    文本 = 文本.Substring(1);
                    断开.字号列表.Add(字号列表[0]);
                    字号列表.RemoveAt(0);
                    // 更新布局
                    断开.UpdateLayout();
                }
                // 超过最大宽度时，断开完成
                else break;
            }
            // 断开元素后，自身布局需要重新计算
            UpdateLayout();
            // 返回断开部分
            return 断开;
        }

        public override void 绘图(DrawingContext dc, double left, double top)
        {
            // left = Math.Round(left);
            // top = Math.Round(top);
            Brush brush = IsSpace ? _space : _text;
            if (IsSpace && ActualWidth + RightMargin != _initWidth) brush = _widthChanged;
            dc.DrawRectangle(brush, null, new Rect(left, top, ActualWidth, ActualHeight));
        }

        #endregion

        #region 公开方法

        public double 最后一个字宽()
        {
            // 暂时使用字号代替字符宽度
            return 字号列表.Last();
        }

        #endregion

        /// <summary>初始宽度</summary>
        private double _initWidth = 0;
        private Brush _space = Brushes.BurlyWood;
        private Brush _text = Brushes.LightGray;
        private Brush _widthChanged = Brushes.OrangeRed;
    }
}