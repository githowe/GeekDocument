using GeekDocument.SubSystem.GlyphSystem;

namespace GeekDocument.SubSystem.LayoutSystem;

/// <summary>
/// 字。表示单字、单词或空格
/// </summary>
public class Word
{
    #region 属性

    public string Text { get; set; } = "";

    public WordType WordType { get; set; } = WordType.Chinese;

    /// <summary>
    /// 字符索引列表
    ///     字符索引指的是相对整个字符串中的索引，之所以是列表，是因为单个字可能是一个英文单词
    /// </summary>
    public List<int> CharIndexList { get; set; } = new List<int>();

    /// <summary>字形图片列表</summary>
    public List<GlyphImage> GlyphImageList { get; set; } = new List<GlyphImage>();

    /// <summary>多个字符</summary>
    public bool MultiChar => GlyphImageList.Count > 1;

    /// <summary>字号</summary>
    public double Size { get; set; } = 0;

    /// <summary>
    /// 字宽
    ///     中文是字的宽度。英文是单词的宽度
    /// </summary>
    public double Width { get; set; } = 0;

    /// <summary>
    /// 间距
    ///     分割中英文的间距
    /// </summary>
    public double Interval { get; set; } = 0;

    #endregion

    public override string ToString() => Text;

    /// <summary>
    /// 更新字形图片
    /// </summary>
    public void UpdateGlyphImage(List<string> fontList, int fontSize, List<bool> boldList, List<bool> italicList, bool oneStyle = true)
    {
        List<GlyphImage> imageList = new List<GlyphImage>();
        double width = 0;

        int index = 0;
        // 遍历字符
        foreach (var c in Text)
        {
            string font = fontList[index];
            bool bold = boldList[index];
            bool italic = italicList[index];
            // 获取字形图片
            GlyphImage? glyphImage = GlyphCache.Instance.GetGlyphImage(c, font, fontSize, bold, italic);
            if (glyphImage == null) glyphImage = GlyphCache.Instance.GetGlyphImage('□', font, fontSize, bold, italic);
            if (glyphImage == null) glyphImage = GlyphCache.Instance.GetGlyphImage(c, "新宋体", fontSize, bold, italic);
            if (glyphImage == null) glyphImage = GlyphCache.Instance.GetGlyphImage('□', "新宋体", fontSize, bold, italic);
            // 添加字形图片并累加宽度
            imageList.Add(glyphImage);
            width += glyphImage.GlyphWidth;
            if (!oneStyle) index++;
        }
        // 更新字形图片列表和宽度
        GlyphImageList = imageList;
        Width = width;
    }

    public List<double> GetXList(double start_x)
    {
        List<double> result = new List<double>();
        foreach (var item in GlyphImageList)
        {
            result.Add(start_x);
            start_x += item.GlyphWidth;
        }
        return result;
    }
}