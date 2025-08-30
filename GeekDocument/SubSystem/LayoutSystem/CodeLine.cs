using GeekDocument.SubSystem.GlyphSystem;

namespace GeekDocument.SubSystem.LayoutSystem
{
    public class CodeLine
    {
        #region 属性

        public string Text { get; set; } = "";

        /// <summary>字形图片列表</summary>
        public List<GlyphImage> GlyphImageList { get; set; } = new List<GlyphImage>();

        /// <summary>字体</summary>
        public string FontFamily { get; set; } = "新宋体";

        /// <summary>字号</summary>
        public int Size { get; set; } = 0;

        public int Length => Text.Length;

        #endregion

        /// <summary>
        /// 更新字形图片
        /// </summary>
        public void UpdateGlyphImage()
        {
            List<GlyphImage> imageList = new List<GlyphImage>();
            foreach (var c in Text)
            {
                // 获取字形图片
                GlyphImage? glyphImage = GlyphCache.Instance.GetGlyphImage(c, FontFamily, Size);
                // 获取失败时，使用默认字体再获取一次
                if (glyphImage == null)
                {
                    glyphImage = GlyphCache.Instance.GetGlyphImage(c, "新宋体", Size);
                    if (glyphImage == null) throw new Exception($"获取字符 '{c}' 的字形图片失败");
                }
                imageList.Add(glyphImage);
            }
            GlyphImageList = imageList;
        }

        public List<double> GetXList(double start_x)
        {
            List<double> result = new List<double>();
            foreach (var item in GlyphImageList)
            {
                result.Add(start_x);
                start_x += item.GlyphWidth;
            }
            result.Add(start_x);
            return result;
        }

        public double GetWidth()
        {
            double width = 0;
            foreach (var item in GlyphImageList) width += item.GlyphWidth;
            return width;
        }
    }
}