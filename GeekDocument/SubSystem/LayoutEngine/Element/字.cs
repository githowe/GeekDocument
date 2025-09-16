using GeekDocument.SubSystem.GlyphSystem;
using System.Windows;
using System.Windows.Media;

namespace GeekDocument.SubSystem.LayoutEngine.Element
{
    /// <summary>
    /// 字。一个字包含单个或多个字符
    /// </summary>
    public class 字 : 布局元素
    {
        public 字() => 类型 = 元素类型.字;

        #region 属性

        public 字类型 字类型 { get; set; } = 字类型.Chinese;

        public string 文本 { get; set; } = "";

        #endregion

        #region 动态属性

        /// <summary>字形图片列表</summary>
        public List<GlyphImage> GlyphImageList { get; set; } = new List<GlyphImage>();

        public List<string> 字体列表 { get; set; } = new List<string> { "霞鹜文楷" };

        public List<double> 字号列表 { get; set; } = new List<double> { 16 };

        public List<double> 字宽列表 { get; set; } = new List<double>();

        #endregion

        #region object 方法

        public override string ToString()
        {
            string type = IsSpace ? "空白" : 文本;
            return $"{type}：{ActualWidth}";
        }

        #endregion

        #region 布局元素 方法

        public override void Init()
        {
            if (文本 == " ")
            {
                字类型 = 字类型.Space;
                IsSpace = true;
            }
            if (字类型 == 字类型.English) CanBreak = true;
        }

        public override void Measure()
        {
            // 注意：字元素忽略宽高限制

            int index = 0;
            ActualWidth = 0;
            ActualHeight = 字号列表[0];
            foreach (var item in 文本)
            {
                // 获取字形样式
                string font = 字体列表[0];
                double size = 字号列表[0];
                // 生成字形图片
                GlyphImage? glyphImage = GlyphCache.Instance.GetGlyphImage(item, font, size, false, false);
                if (glyphImage == null)
                    throw new Exception("生成字形图片失败");
                GlyphImageList.Add(glyphImage);
                字宽列表.Add(glyphImage.GlyphWidth);
                // 累加宽度
                ActualWidth += glyphImage.GlyphWidth;
                // 取最大高度
                if (字号列表[index] > ActualHeight) ActualHeight = 字号列表[index];
                index++;
            }
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
                    // 更新大小
                    断开.Measure();
                }
                // 超过最大宽度时，断开完成
                else break;
            }
            // 断开元素后，自身大小需要重新计算
            Measure();
            // 返回断开部分
            return 断开;
        }

        public override void 绘图(DrawingContext dc)
        {
            // 不绘制空格
            if (IsSpace) return;

            double word_x = Math.Round(Left);
            double word_y = Math.Round(Top);
            foreach (var image in GlyphImageList)
            {
                Point leftTop = new Point(word_x + image.Origin.X, word_y + image.Origin.Y);
                dc.DrawImage(image.GetBitmap(), new Rect(leftTop, new Size(image.RenderWidth, image.RenderHeight)));
            }
        }

        #endregion

        #region 公开方法

        public double 最后一个字宽() => 字宽列表.Last();

        #endregion
    }
}