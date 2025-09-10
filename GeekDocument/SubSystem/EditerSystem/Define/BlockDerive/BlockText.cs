using GeekDocument.SubSystem.LayoutSystem;
using GeekDocument.SubSystem.StyleSystem;
using Newtonsoft.Json;
using System.Windows.Media;
using XLogic.Base.Ex;

namespace GeekDocument.SubSystem.EditerSystem.Define.BlockDerive
{
    public class SubStyleData
    {
        public int StartIndex { get; set; } = -1;

        public int EndIndex { get; set; } = -1;

        public string StyleJson { get; set; } = "";

        public override string ToString() => $"{StartIndex}-{EndIndex}: {StyleJson}";
    }

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

        public int MarginTop { get; set; } = 0;

        public int MarginBottom { get; set; } = 0;

        public int CustomFirstLineIndent { get; set; } = 0;

        public bool UseCustomFirstLineIndent { get; set; } = false;

        public int LeftIndent { get; set; } = 0;

        public int RightIndent { get; set; } = 0;

        public List<SubStyleData> SubStyleList { get; set; } = new List<SubStyleData>();
    }

    public class LinkInfo
    {
        public int StartIndex { get; set; } = -1;

        public int EndIndex { get; set; } = -1;

        public string Url { get; set; } = "";
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

        public List<LinkInfo> LinkInfoList => _linkInfoList;

        #endregion

        public override void UpdateViewData(int blockWidth)
        {
            _lineList.Clear();
            // 生成字列表
            List<Word> wordList = WordSpliter.Instance.ToWordList(Content);
            // 更新字的字形图片
            bool bold = TStyle == TextStyle.Bold || TStyle == TextStyle.BoldItalic;
            bool italic = TStyle == TextStyle.Italic || TStyle == TextStyle.BoldItalic;
            foreach (var word in wordList)
            {
                List<string> fontList = new List<string>();
                List<bool?> boldList = new List<bool?>();
                List<bool?> italicList = new List<bool?>();
                // 遍历字的字符索引
                int index = 0;
                foreach (var charIndex in word.CharIndexList)
                {
                    fontList.Add("");
                    boldList.Add(null);
                    italicList.Add(null);
                    // 查找子样式
                    if (_styleDict.TryGetValue(charIndex, out SubStyle? subStyle))
                    {
                        // 遍历样式
                        foreach (var style in subStyle.StyleList)
                        {
                            if (style.Type == AppendStyleType.Font)
                            {
                                AppendFont fontStyle = (AppendFont)style;
                                fontList[index] = fontStyle.FontFamily;
                            }
                            else if (style.Type == AppendStyleType.Bold)
                            {
                                AppendBold boldStyle = (AppendBold)style;
                                boldList[index] = boldStyle.Enable;
                            }
                            else if (style.Type == AppendStyleType.Italic)
                            {
                                AppendItalic italicStyle = (AppendItalic)style;
                                italicList[index] = italicStyle.Enable;
                            }
                        }
                    }
                    index++;
                }
                // 没有子样式，则使用块样式
                for (index = 0; index < word.CharIndexList.Count; index++)
                {
                    if (fontList[index] == "") fontList[index] = FontFamily;
                    if (boldList[index] == null) boldList[index] = bold;
                    if (italicList[index] == null) italicList[index] = italic;
                }

                word.UpdateGlyphImage(fontList, FontSize, boldList.Cast<bool>().ToList(), italicList.Cast<bool>().ToList());
            }

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

        public override void SetSubStyle(AppendStyle style, int startIndex, int endIndex)
        {
            for (int index = startIndex; index < endIndex; index++)
            {
                if (!_styleDict.ContainsKey(index)) _styleDict[index] = new SubStyle();
                _styleDict[index].AddStyle(style);
            }
            UpdateLinkInfo();
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
            MarginTop = blockData.MarginTop;
            MarginBottom = blockData.MarginBottom;
            CustomFirstLineIndent = blockData.CustomFirstLineIndent;
            UseCustomFirstLineIndent = blockData.UseCustomFirstLineIndent;
            LeftIndent = blockData.LeftIndent;
            RightIndent = blockData.RightIndent;
            LoadSubStyleData(blockData.SubStyleList);
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
                MarginTop = MarginTop,
                MarginBottom = MarginBottom,
                CustomFirstLineIndent = CustomFirstLineIndent,
                UseCustomFirstLineIndent = UseCustomFirstLineIndent,
                LeftIndent = LeftIndent,
                RightIndent = RightIndent,
                SubStyleList = GetSubStyleData()
            };
            return JsonConvert.SerializeObject(blockData);
        }

        public override int GetViewHeight() => _viewHeight;

        public void RemoveSubStyle(AppendStyleType type, int startIndex, int endIndex)
        {
            for (int index = startIndex; index < endIndex; index++)
            {
                if (_styleDict.TryGetValue(index, out SubStyle? subStyle))
                {
                    subStyle.RemoveStyle(type);
                    if (subStyle.StyleList.Count == 0) _styleDict.Remove(index);
                }
            }
            UpdateLinkInfo();
        }

        public void ClearSubStyle(AppendStyleType type)
        {
            foreach (var subStyle in _styleDict.Values) subStyle.RemoveStyle(type);
        }

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
                MarginTop = MarginTop,
                MarginBottom = MarginBottom,
                FirstLineIndent = FirstLineIndent,
                CustomFirstLineIndent = CustomFirstLineIndent,
                UseCustomFirstLineIndent = UseCustomFirstLineIndent,
                LeftIndent = LeftIndent,
                RightIndent = RightIndent
            };
            return blockText;
        }

        /// <summary>
        /// 获取指定索引处字符的字体
        /// </summary>
        public string GetFont(int charIndex)
        {
            // 获取子样式
            if (_styleDict.TryGetValue(charIndex, out SubStyle? subStyle))
            {
                // 遍历样式
                foreach (var style in subStyle.StyleList)
                {
                    // 找到了字体样式则返回
                    if (style.Type == AppendStyleType.Font)
                    {
                        AppendFont fontStyle = (AppendFont)style;
                        return fontStyle.FontFamily;
                    }
                }
            }
            // 返回块样式
            return FontFamily;
        }

        /// <summary>
        /// 获取指定索引处字符的颜色
        /// </summary>
        public Color? GetColor(int charIndex)
        {
            // 获取子样式
            if (_styleDict.TryGetValue(charIndex, out SubStyle? subStyle))
            {
                // 遍历样式
                foreach (var style in subStyle.StyleList)
                {
                    // 找到了颜色样式则返回
                    if (style.Type == AppendStyleType.Color)
                    {
                        AppendColor colorStyle = (AppendColor)style;
                        return new Color { R = colorStyle.R, G = colorStyle.G, B = colorStyle.B };
                    }
                }
            }
            // 返回空
            return null;
        }

        public bool GetBold(int charIndex)
        {
            // 获取子样式
            if (_styleDict.TryGetValue(charIndex, out SubStyle? subStyle))
            {
                // 遍历样式
                foreach (var style in subStyle.StyleList)
                {
                    // 找到了加粗样式则返回
                    if (style.Type == AppendStyleType.Bold)
                    {
                        AppendBold boldStyle = (AppendBold)style;
                        return boldStyle.Enable;
                    }
                }
            }
            // 返回块样式
            return TStyle == TextStyle.Bold || TStyle == TextStyle.BoldItalic;
        }

        public bool GetItalic(int charIndex)
        {
            // 获取子样式
            if (_styleDict.TryGetValue(charIndex, out SubStyle? subStyle))
            {
                // 遍历样式
                foreach (var style in subStyle.StyleList)
                {
                    // 找到了斜体样式则返回
                    if (style.Type == AppendStyleType.Italic)
                    {
                        AppendItalic italicStyle = (AppendItalic)style;
                        return italicStyle.Enable;
                    }
                }
            }
            // 返回块样式
            return TStyle == TextStyle.Italic || TStyle == TextStyle.BoldItalic;
        }

        public string GetLink(int charIndex)
        {
            if (_styleDict.TryGetValue(charIndex, out SubStyle? subStyle))
            {
                // 遍历样式
                foreach (var style in subStyle.StyleList)
                {
                    // 找到了链接样式则返回
                    if (style.Type == AppendStyleType.Link)
                    {
                        AppendLink linkStyle = (AppendLink)style;
                        return linkStyle.Url;
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// 更新链接信息
        /// </summary>
        public void UpdateLinkInfo()
        {
            if (HasLink()) _linkInfoList = GetLinkInfo();
            else _linkInfoList.Clear();
        }

        /// <summary>
        /// 有链接
        /// </summary>
        private bool HasLink()
        {
            foreach (var pair in _styleDict)
                foreach (var item in pair.Value.StyleList)
                    if (item.Type == AppendStyleType.Link) return true;
            return false;
        }

        /// <summary>
        /// 获取链接信息
        /// </summary>
        private List<LinkInfo> GetLinkInfo()
        {
            List<LinkInfo> result = new List<LinkInfo>();
            List<SubStyleData> styleData = GetSubStyleData();
            foreach (var style in styleData)
            {
                List<string>? data = JsonConvert.DeserializeObject<List<string>>(style.StyleJson);
                if (data == null || data.Count == 0) continue;
                if (data[0] == "Link")
                {
                    result.Add(new LinkInfo
                    {
                        StartIndex = style.StartIndex,
                        EndIndex = style.EndIndex,
                        Url = data[1]
                    });
                }
            }
            return result;
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
            MarginTop = 0;
            MarginBottom = 0;
            CustomFirstLineIndent = 0;
            UseCustomFirstLineIndent = false;
            LeftIndent = 0;
            RightIndent = 0;
        }

        private List<SubStyleData> GetSubStyleData()
        {
            List<SubStyleData> result = new List<SubStyleData>();

            Dictionary<AppendStyleType, AppendStyle?> _currentStyleDict = new Dictionary<AppendStyleType, AppendStyle?>
            {
                { AppendStyleType.Font, null },
                { AppendStyleType.Bold, null },
                { AppendStyleType.Italic, null },
                { AppendStyleType.Color, null },
                { AppendStyleType.Link, null },
            };
            Dictionary<AppendStyleType, SubStyleData?> styleDataDict = new Dictionary<AppendStyleType, SubStyleData?>
            {
                { AppendStyleType.Font, null },
                { AppendStyleType.Bold, null },
                { AppendStyleType.Italic, null },
                { AppendStyleType.Color, null },
                { AppendStyleType.Link, null },
            };

            // 获取排序后的索引列表
            List<int> indexList = new List<int>(_styleDict.Keys);
            indexList.Sort();
            // 遍历索引列表
            int prevIndex = -1;
            foreach (var index in indexList)
            {
                // 索引不连续，结束所有样式
                if (prevIndex != -1 && index != prevIndex + 1)
                {
                    foreach (var type in _currentStyleDict.Keys.ToList())
                    {
                        if (_currentStyleDict[type] != null)
                        {
                            result.Add(styleDataDict[type]!);
                            _currentStyleDict[type] = null;
                            styleDataDict[type] = null;
                        }
                    }
                }
                prevIndex = index;
                // 获取子样式
                SubStyle subStyle = _styleDict[index];
                foreach (var item in subStyle.StyleList)
                {
                    // 当前样式为空，则添加当前样式并创建样式数据
                    if (_currentStyleDict[item.Type] == null)
                    {
                        _currentStyleDict[item.Type] = item;
                        styleDataDict[item.Type] = new SubStyleData
                        {
                            StartIndex = index,
                            EndIndex = index + 1,
                            StyleJson = item.ToJson()
                        };
                    }
                    else
                    {
                        // 样式相同，则更新结束索引
                        if (_currentStyleDict[item.Type]!.SameAs(item)) styleDataDict[item.Type]!.EndIndex++;
                        // 样式不同，则添加当前样式并置空当前样式
                        else
                        {
                            result.Add(styleDataDict[item.Type]!);
                            _currentStyleDict[item.Type] = null;
                            styleDataDict[item.Type] = null;
                        }
                    }
                }
            }
            // 添加剩余样式
            foreach (var item in styleDataDict)
            {
                if (item.Value != null) result.Add(item.Value);
            }

            return result;
        }

        private void LoadSubStyleData(List<SubStyleData> subStyleList)
        {
            foreach (var item in subStyleList)
            {
                List<string>? data = JsonConvert.DeserializeObject<List<string>>(item.StyleJson);
                if (data == null || data.Count == 0) continue;

                AppendStyle? appendStyle = null;
                switch (data[0])
                {
                    case "Font":
                        appendStyle = new AppendFont { FontFamily = data[1] };
                        break;
                    case "Bold":
                        appendStyle = new AppendBold { Enable = bool.Parse(data[1]) };
                        break;
                    case "Italic":
                        appendStyle = new AppendItalic { Enable = bool.Parse(data[1]) };
                        break;
                    case "Color":
                        var (r, g, b) = data[1].ParseColorCode();
                        appendStyle = new AppendColor { R = r, G = g, B = b };
                        break;
                    case "Link":
                        appendStyle = new AppendLink { Url = data[1] };
                        break;
                }
                if (appendStyle == null) continue;
                SetSubStyle(appendStyle, item.StartIndex, item.EndIndex);
            }
            UpdateLinkInfo();
        }

        private readonly List<TextLine> _lineList = new List<TextLine>();
        private int _viewHeight = 0;

        private readonly Dictionary<int, SubStyle> _styleDict = new Dictionary<int, SubStyle>();

        private List<LinkInfo> _linkInfoList = new List<LinkInfo>();
    }
}