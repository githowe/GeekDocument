using System.Windows;
using System.Windows.Media;

namespace GeekDocument.SubSystem.LayoutEngine.Element
{
    public class 图片 : 布局元素
    {
        public 图片() => 类型 = 元素类型.图片;

        #region 属性

        public string SourcePath { get; set; } = "";

        public int SourceWidth { get; set; } = 1280;

        public int SourceHeight { get; set; } = 720;

        #endregion

        #region 布局元素方法

        public override void Init()
        {
            if (SourcePath != "") LoadImage();
        }

        public override void UpdateLayout()
        {
            // 无效值
            if (MaxWidth < 0 || MaxHeight < 0)
            {
                ActualWidth = 0;
                ActualHeight = 0;
                return;
            }
            // 无限制
            if (double.IsNaN(MaxWidth) && double.IsNaN(MaxHeight))
            {
                ActualWidth = SourceWidth;
                ActualHeight = SourceHeight;
            }
            // 限制了宽度
            else if (MaxWidth > 0 && double.IsNaN(MaxHeight))
            {
                适配宽度();
            }
            // 限制了高度
            else if (double.IsNaN(MaxWidth) && MaxHeight > 0)
            {
                适配高度();
            }
            // 限制了宽度和高度
            else if (MaxWidth > 0 && MaxHeight > 0)
            {
                if (SourceWidth / (double)SourceHeight > MaxWidth / (double)MaxHeight) 适配宽度();
                else 适配高度();
            }
        }

        public override double 压缩左边距()
        {
            if (LeftMargin > 0) return ActualWidth + LeftMargin / 2;
            return ActualWidth;
        }

        public override double 压缩右边距()
        {
            if (RightMargin > 0) return ActualWidth + RightMargin / 2;
            return ActualWidth;
        }

        public override double 压缩元素()
        {
            double left = 0;
            if (LeftMargin > 0) left = LeftMargin / 2;
            double right = 0;
            if (RightMargin > 0) right = RightMargin / 2;
            return left + ActualWidth + right;
        }

        public override void 压缩至(double 比例)
        {
            double leftMax = LeftMargin / 2;
            LeftMargin -= leftMax * 比例;
            double rightMax = RightMargin / 2;
            RightMargin -= rightMax * 比例;
        }

        public override void 绘图(DrawingContext dc)
        {
            double x = Math.Round(Left);
            double y = Math.Round(Top);
            dc.DrawRectangle(Brushes.DarkMagenta, null, new Rect(x, y, ActualWidth, ActualHeight));
        }

        #endregion

        #region 私有方法

        private void LoadImage()
        {

        }

        private void 适配宽度()
        {
            if (SourceWidth <= MaxWidth)
            {
                ActualWidth = SourceWidth;
                ActualHeight = SourceHeight;
            }
            else
            {
                ActualWidth = MaxWidth;
                ActualHeight = Math.Round(SourceHeight * (MaxWidth / SourceWidth));
            }
        }

        private void 适配高度()
        {
            if (SourceHeight <= MaxHeight)
            {
                ActualWidth = SourceWidth;
                ActualHeight = SourceHeight;
            }
            else
            {
                ActualHeight = MaxHeight;
                ActualWidth = Math.Round(SourceWidth * (MaxHeight / SourceHeight));
            }
        }

        #endregion
    }
}