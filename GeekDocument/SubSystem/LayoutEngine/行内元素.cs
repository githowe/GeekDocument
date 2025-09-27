using System.Windows;

namespace GeekDocument.SubSystem.LayoutEngine
{
    public abstract class 行内元素 : 布局元素
    {
        public double Left { get; set; } = double.NaN;

        public double Top { get; set; } = double.NaN;

        public double MaxWidth { get; set; } = double.NaN;

        public double ActualWidth { get; set; } = double.NaN;

        public double ActualHeight { get; set; } = double.NaN;

        public bool IsSpace { get; set; } = false;

        public bool CanBreak { get; set; } = false;

        public bool CanInput { get; set; } = false;

        /// <summary>左边距</summary>
        public double LeftMargin { get; set; } = 0;

        /// <summary>右边距</summary>
        public double RightMargin { get; set; } = 0;

        /// <summary>右扩展边距。用于两端对齐时的字间距</summary>
        public double RightExtend { get; set; } = 0;

        public override Rect GetViewRect()
        {
            return new Rect(Left, Top, ActualWidth, ActualHeight);
        }

        /// <summary>
        /// 压缩左边距，然后返回与实际高度之和。用于添加至元素行时判断能否容纳
        /// 一般添加了左边距的元素需要重写此方法，例如插入至文本块的图片元素
        /// </summary>
        public virtual double 压缩左边距() => ActualWidth;

        /// <summary>
        /// 压缩实际宽度并返回。该方法中不需要考虑左边距和右边距
        /// 实际宽度不允许压缩的不需要重写（例如图片），如果是中文标点这种存在可压缩空间的，根据排版规则决定是否重写
        /// </summary>
        public virtual double 压缩实际宽度() => ActualWidth;

        /// <summary>
        /// 压缩右边距，行中没有元素时，添加元素不需要考虑左边距，此时只需压缩右边距即可
        /// </summary>
        public virtual double 压缩右边距() => ActualWidth;

        /// <summary>
        /// 返回压缩整个元素之后的宽度。在添加至元素行后调用，以最大化可用空间
        /// </summary>
        public virtual double 压缩元素() => ActualWidth;

        /// <summary>
        /// 压缩至指定比例，派生类需要在内部调整压缩后的布局
        /// </summary>
        public virtual void 压缩至(double 比例) { }

        public virtual 行内元素 断开(double 最大宽度) { return this; }

        public virtual void 移入光标至开头() { }

        public virtual void 移入光标至末尾() { }
    }
}