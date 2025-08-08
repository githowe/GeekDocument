using GeekDocument.SubSystem.LayoutSystem;
using GeekDocument.SubSystem.OptionSystem;
using Newtonsoft.Json;

namespace GeekDocument.SubSystem.EditerSystem.Define
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

        /// <summary>首行缩进</summary>
        public int FirstLineIndent { get; set; } = 0;
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

        /// <summary>首行缩进</summary>
        public int FirstLineIndent { get; set; } = 0;

        public List<TextLine> ViewData => _lineList;

        #endregion

        public override void UpdateViewData()
        {
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
            TextWrapTool.Instance.WrapText(wordList, _lineList, Options.Instance.Page.PageWidth, Align);
            // 更新视图高度
            if (_lineList.Count == 0) _viewHeight = FontSize;
            else _viewHeight = _lineList.Count * FontSize + LineSpace * (_lineList.Count - 1);
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
            FirstLineIndent = blockData.FirstLineIndent;
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
                FirstLineIndent = FirstLineIndent,
            };
            return JsonConvert.SerializeObject(blockData);
        }

        public override int GetViewHeight() => _viewHeight;

        protected List<TextLine> _lineList = new List<TextLine>();
        protected int _viewHeight = 0;
    }
}