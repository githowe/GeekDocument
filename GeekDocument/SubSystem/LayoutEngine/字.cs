using GeekDocument.SubSystem.GlyphSystem;
using System.Windows;
using System.Windows.Media;

namespace GeekDocument.SubSystem.LayoutEngine
{
    public class 字 : 行内元素
    {
        public 字()
        {
            Icon = "Text";
        }

        public override string Name => 字符.ToString();

        public char 字符 { get; set; } = '\0';

        public string 字体 { get; set; } = "霞鹜文楷";

        public double 字号 { get; set; } = 16;

        public GlyphImage? GlyphImage { get; set; } = null;

        #region 行内元素方法

        public override double 压缩元素()
        {
            // 空白元素，最大可压缩一半
            if (IsSpace) return ActualWidth / 2;
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
        }

        #endregion

        #region 布局元素核心方法

        public override void Init()
        {
            if (字符 == ' ') IsSpace = true;
        }

        public override void 测量()
        {
            // 注意：字元素忽略宽高限制

            ActualWidth = 0;
            ActualHeight = 字号;
            GlyphImage? glyphImage = GlyphCache.Instance.GetGlyphImage(字符, 字体, 字号, false, false);
            if (glyphImage == null)
                throw new Exception("生成字形图片失败");
            GlyphImage = glyphImage;
            ActualWidth = glyphImage.GlyphWidth;
        }

        public override void 渲染(DrawingContext? dc)
        {
            if (IsSpace) return;

            // 字元素使用元素行的绘图上下文
            if (dc == null) return;
            // dc.DrawRectangle(Brushes.LightSteelBlue, null, new Rect(Left, Top, ActualWidth, ActualHeight));

            if (GlyphImage == null) return;
            Point leftTop = new Point(Math.Round(Left) + GlyphImage.Origin.X, Math.Round(Top) + GlyphImage.Origin.Y);
            dc.DrawImage(GlyphImage.GetBitmap(), new Rect(leftTop, new Size(GlyphImage.RenderWidth, GlyphImage.RenderHeight)));
        }

        #endregion

        public override string ToString() => IsSpace ? "空格" : 字符.ToString();
    }
}