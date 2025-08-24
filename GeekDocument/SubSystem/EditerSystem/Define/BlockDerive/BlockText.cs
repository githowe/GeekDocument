using GeekDocument.SubSystem.LayoutSystem;
using GeekDocument.SubSystem.StyleSystem;
using Newtonsoft.Json;

namespace GeekDocument.SubSystem.EditerSystem.Define.BlockDerive
{
    public class TextBlockData
    {
        /// <summary>内容</summary>
        public string Content { get; set; } = "";

        /// <summary>字体</summary>
        public string FontFamily { get; set; } = "仿宋";

        /// <summary>字号</summary>
        public int FontSize { get; set; } = 16;

        /// <summary>颜色</summary>
        public string Color { get; set; } = "FFFFFF";

        /// <summary>文本样式</summary>
        public int TextStyle { get; set; } = 0;

        /// <summary>对齐方式</summary>
        public int Align { get; set; } = 3;

        /// <summary>行间距</summary>
        public int LineSpace { get; set; } = 4;

        public int CustomFirstLineIndent { get; set; } = 0;

        public bool UseCustomFirstLineIndent { get; set; } = false;

        public int LeftIndent { get; set; } = 0;

        public int RightIndent { get; set; } = 0;
    }

    /// <summary>
    /// 文本块
    /// </summary>
    public class BlockText : Block
    {
        #region 属性

        /// <summary>内容</summary>
        public string Content { get; set; } = "";

        /// <summary>字体</summary>
        public string FontFamily { get; set; } = "仿宋";

        /// <summary>字号</summary>
        public int FontSize { get; set; } = 16;

        /// <summary>颜色</summary>
        public string Color { get; set; } = "FFFFFF";

        /// <summary>文本样式：普通、加粗、倾斜</summary>
        public TextStyle TStyle { get; set; } = TextStyle.Normal;

        /// <summary>对齐方式</summary>
        public LineAlignType Align { get; set; } = LineAlignType.Justify;

        /// <summary>行间距</summary>
        public int LineSpace { get; set; } = 4;

        /// <summary>首行缩进（从文档继承，无需写入存档）</summary>
        public int FirstLineIndent { get; set; } = 0;

        /// <summary>
        /// 实际首行缩进值
        /// </summary>
        public int RealFirstLineIndent => UseCustomFirstLineIndent ? CustomFirstLineIndent : FirstLineIndent;

        /// <summary>自定义首行缩进</summary>
        public int CustomFirstLineIndent { get; set; } = 0;

        /// <summary>使用自定义首行缩进</summary>
        public bool UseCustomFirstLineIndent { get; set; } = false;

        /// <summary>左缩进</summary>
        public int LeftIndent { get; set; } = 0;

        /// <summary>右缩进</summary>
        public int RightIndent { get; set; } = 0;

        public List<TextLine> ViewData => _lineList;

        #endregion

        public override void UpdateViewData(int blockWidth)
        {
            _lineList.Clear();
            // 生成字列表
            List<Word> wordList = WordSpliter.Instance.ToWordList(Content);
            // 更新字的字形图片
            bool bold = TStyle == TextStyle.Bold || TStyle == TextStyle.BoldItalic;
            bool italic = TStyle == TextStyle.Italic || TStyle == TextStyle.BoldItalic;
            foreach (var word in wordList) word.UpdateGlyphImage(FontFamily, FontSize, bold, italic);
            // 更新字号
            foreach (var word in wordList) word.Size = FontSize;
            // 更新字间距
            for (int index = 0; index < wordList.Count - 1; index++)
            {
                Word first = wordList[index];
                Word second = wordList[index + 1];
                // 如果是中文和英文之间，设置间距
                if (first.WordType == WordType.Chinese && second.WordType == WordType.English
                    || first.WordType == WordType.English && second.WordType == WordType.Chinese)
                {
                    first.Interval = first.Size * 0.25;
                }
            }
            // 生成文本行
            TextWrapTool.Instance.FirstLineIndent = RealFirstLineIndent;
            TextWrapTool.Instance.WrapText(wordList, _lineList, blockWidth, Align);
            // 更新视图高度
            if (_lineList.Count == 0) _viewHeight = FontSize;
            else _viewHeight = _lineList.Count * FontSize + LineSpace * (_lineList.Count - 1);
        }

        public override void ApplyStyle(StyleSheet? style)
        {
            ResetStyle();

            if (style == null) return;
            foreach (var item in style.ItemList)
            {
                bool bold = false;
                bool italic = false;

                switch (item.ID)
                {
                    case StyleID.FontFamily:
                        FontFamily = item.Value;
                        break;
                    case StyleID.FontSize:
                        FontSize = int.Parse(item.Value);
                        break;
                    case StyleID.Color:
                        Color = item.Value;
                        break;
                    case StyleID.Bold:
                        bold = item.Value == "true";
                        break;
                    case StyleID.Italic:
                        italic = item.Value == "true";
                        break;
                    case StyleID.Align:
                        Align = (LineAlignType)int.Parse(item.Value);
                        break;
                    case StyleID.LineSpace:
                        LineSpace = int.Parse(item.Value);
                        break;
                    case StyleID.FirstLineIndent:
                        CustomFirstLineIndent = int.Parse(item.Value);
                        UseCustomFirstLineIndent = true;
                        break;
                    case StyleID.IndentLeft:
                        LeftIndent = int.Parse(item.Value);
                        break;
                    case StyleID.IndentRight:
                        RightIndent = int.Parse(item.Value);
                        break;
                }

                if (bold && italic) TStyle = TextStyle.BoldItalic;
                else if (bold) TStyle = TextStyle.Bold;
                else if (italic) TStyle = TextStyle.Italic;
                else TStyle = TextStyle.Normal;
            }
        }

        public override void LoadJson(string json)
        {
            TextBlockData? blockData = JsonConvert.DeserializeObject<TextBlockData>(json);
            if (blockData == null) return;

            Content = blockData.Content;
            FontFamily = blockData.FontFamily;
            FontSize = blockData.FontSize;
            Color = blockData.Color;
            TStyle = (TextStyle)blockData.TextStyle;
            Align = (LineAlignType)blockData.Align;
            LineSpace = blockData.LineSpace;
            CustomFirstLineIndent = blockData.CustomFirstLineIndent;
            UseCustomFirstLineIndent = blockData.UseCustomFirstLineIndent;
            LeftIndent = blockData.LeftIndent;
            RightIndent = blockData.RightIndent;
        }

        public override string ToJson()
        {
            TextBlockData blockData = new TextBlockData
            {
                Content = Content,
                FontFamily = FontFamily,
                FontSize = FontSize,
                Color = Color,
                TextStyle = (int)TStyle,
                Align = (int)Align,
                LineSpace = LineSpace,
                CustomFirstLineIndent = CustomFirstLineIndent,
                UseCustomFirstLineIndent = UseCustomFirstLineIndent,
                LeftIndent = LeftIndent,
                RightIndent = RightIndent,
            };
            return JsonConvert.SerializeObject(blockData);
        }

        public override int GetViewHeight() => _viewHeight;

        /// <summary>
        /// 克隆文本块，但不包含内容
        /// </summary>
        public BlockText CloneWithoutContent()
        {
            BlockText blockText = new BlockText
            {
                FontFamily = FontFamily,
                FontSize = FontSize,
                Color = Color,
                TStyle = TStyle,
                Align = Align,
                LineSpace = LineSpace,
                FirstLineIndent = FirstLineIndent,
                CustomFirstLineIndent = CustomFirstLineIndent,
                UseCustomFirstLineIndent = UseCustomFirstLineIndent,
                LeftIndent = LeftIndent,
                RightIndent = RightIndent
            };
            return blockText;
        }

        /// <summary>
        /// 重置样式
        /// </summary>
        private void ResetStyle()
        {
            FontFamily = "仿宋";
            FontSize = 16;
            Color = "FFFFFF";
            TStyle = TextStyle.Normal;
            Align = LineAlignType.Justify;
            LineSpace = 4;
            CustomFirstLineIndent = 0;
            UseCustomFirstLineIndent = false;
            LeftIndent = 0;
            RightIndent = 0;
        }

        protected List<TextLine> _lineList = new List<TextLine>();
        protected int _viewHeight = 0;
    }
}