using GeekDocument.AppTool;
using GeekDocument.SubSystem.EditerSystem.Define;
using GeekDocument.SubSystem.EditerSystem.Define.BlockDerive;
using GeekDocument.SubSystem.StyleSystem;
using GeekDocument.SubSystem.WindowSystem;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using XLogic.Base.Ex;

namespace GeekDocument.SubSystem.EditerSystem.Control.PropertyPanel;

public partial class TextPropertyPanel : PropertyPanel
{
    public TextPropertyPanel() => InitializeComponent();

    #region 属性

    public BlockText Block { get; set; }

    #endregion

    public override void Init()
    {
        if (SelectStartIndex >= 0 && SelectEndIndex >= 0)
            LoadSelectedProperty();
        else
            LoadBlockProperty();
    }

    /// <summary>
    /// 加载块属性
    /// </summary>
    private void LoadBlockProperty()
    {
        // 字体、字号、颜色
        Bar_Font.LoadProperty(FontManager.FontList, Block.FontFamily);
        Bar_Size.LoadProperty(Block.FontSize.ToString());
        var (r, g, b) = Block.Color.ParseColorCode();
        Bar_Color.LoadProperty(r, g, b);
        // 样式
        Bar_Style.LoadProperty();
        Bar_Style.AddCheckBox(GetIcon("Bold"), "Bold");
        Bar_Style.AddCheckBox(GetIcon("Italic"), "Italic");
        Bar_Style.SetChecked("Bold", Block.TStyle is TextStyle.Bold or TextStyle.BoldItalic);
        Bar_Style.SetChecked("Italic", Block.TStyle is TextStyle.Italic or TextStyle.BoldItalic);
        // 对齐方式
        Bar_Align.LoadProperty();
        Bar_Align.AddRadioButton(GetIcon("AlignLeft"), "Left");
        Bar_Align.AddRadioButton(GetIcon("AlignCenter"), "Center");
        Bar_Align.AddRadioButton(GetIcon("AlignRight"), "Right");
        Bar_Align.AddRadioButton(GetIcon("AlignJustify"), "Justify");
        Bar_Align.SetChecked(Block.Align.ToString());
        // 首行缩进、左右缩进
        Bar_CustonIndent.LoadProperty(Block.UseCustomFirstLineIndent);
        Bar_Indent.LoadProperty(Block.CustomFirstLineIndent.ToString());
        Bar_LeftIndent.LoadProperty(Block.LeftIndent.ToString());
        Bar_RightIndent.LoadProperty(Block.RightIndent.ToString());
        // 行间距、上边距、下边距
        Bar_LineSpace.LoadProperty(Block.LineSpace.ToString());
        Bar_MarginTop.LoadProperty(Block.MarginTop.ToString());
        Bar_MarginBottom.LoadProperty(Block.MarginBottom.ToString());

        // 监听字体、字号、颜色
        Bar_Font.SelectionChanged += Font_SelectionChanged;
        Bar_Size.TextChanged += Size_TextChanged;
        Bar_Color.ColorChanged += Color_ColorChanged;
        // 监听样式
        Bar_Style.BoxChecked += Style_BoxChecked;
        Bar_Style.BoxUnchecked += Bar_Style_BoxUnchecked;
        // 监听缩进
        Bar_CustonIndent.Opened += CustonIndent_Opened;
        Bar_CustonIndent.Closed += CustonIndent_Closed;
        Bar_Indent.TextChanged += Indent_TextChanged;
        // 监听行间距、上边距、下边距
        Bar_LineSpace.TextChanged += LineSpace_TextChanged;
        Bar_MarginTop.TextChanged += MarginTop_TextChanged;
        Bar_MarginBottom.TextChanged += MarginBottom_TextChanged;
    }

    /// <summary>
    /// 加载选中属性
    /// </summary>
    private void LoadSelectedProperty()
    {
        // 隐藏不相关属性
        Group_01.Visibility = Visibility.Collapsed;
        Group_02.Visibility = Visibility.Collapsed;
        // 显示选中相关属性
        Group_03.Visibility = Visibility.Visible;

        // 字体、颜色
        string font = Block.GetFont(SelectStartIndex);
        Color? color = Block.GetColor(SelectStartIndex);
        Bar_Font2.LoadProperty(FontManager.FontList, font);
        if (color != null)
            Bar_Color2.LoadProperty(color.Value.R, color.Value.G, color.Value.B);
        else
        {
            var (r, g, b) = Block.Color.ParseColorCode();
            Bar_Color2.LoadProperty(r, g, b);
        }
        // 样式
        bool bold = Block.GetBold(SelectStartIndex);
        bool italic = Block.GetItalic(SelectStartIndex);
        Bar_Style2.LoadProperty();
        Bar_Style2.AddCheckBox(GetIcon("Bold"), "Bold");
        Bar_Style2.AddCheckBox(GetIcon("Italic"), "Italic");
        Bar_Style2.SetChecked("Bold", bold);
        Bar_Style2.SetChecked("Italic", italic);
        // 链接
        string link = Block.GetLink(SelectStartIndex);
        Bar_Link.LoadProperty(link);

        // 监听字体、颜色
        Bar_Font2.SelectionChanged += Font_SelectionChanged_Select;
        Bar_Color2.ColorChanged += Color_ColorChanged_Select;
        // 监听样式
        Bar_Style2.BoxChecked += Style_BoxChecked_Select;
        Bar_Style2.BoxUnchecked += Bar_Style_BoxUnchecked_Select;
        // 监听链接
        Bar_Link.TextChanged += Link_TextChanged;
    }

    private void Font_SelectionChanged_Select(string text)
    {
        AppendFont style = new AppendFont
        {
            FontFamily = text
        };
        Block.SetSubStyle(style, SelectStartIndex, SelectEndIndex);
        PropertyChanged?.Invoke();
    }

    private void Color_ColorChanged_Select(Color color)
    {
        AppendColor style = new AppendColor
        {
            R = color.R,
            G = color.G,
            B = color.B,
        };
        Block.SetSubStyle(style, SelectStartIndex, SelectEndIndex);
        PropertyChanged?.Invoke();
    }

    private void Style_BoxChecked_Select(string name)
    {
        if (name == "Bold")
        {
            AppendBold style = new AppendBold { Enable = true };
            Block.SetSubStyle(style, SelectStartIndex, SelectEndIndex);
        }
        else if (name == "Italic")
        {
            AppendItalic style = new AppendItalic { Enable = true };
            Block.SetSubStyle(style, SelectStartIndex, SelectEndIndex);
        }
        PropertyChanged?.Invoke();
    }

    private void Bar_Style_BoxUnchecked_Select(string name)
    {
        if (name == "Bold")
        {
            AppendBold style = new AppendBold { Enable = false };
            Block.SetSubStyle(style, SelectStartIndex, SelectEndIndex);
        }
        else if (name == "Italic")
        {
            AppendItalic style = new AppendItalic { Enable = false };
            Block.SetSubStyle(style, SelectStartIndex, SelectEndIndex);
        }
        PropertyChanged?.Invoke();
    }

    private void Link_TextChanged(string text)
    {
        string url = "";
        MatchCollection matches = Regex.Matches(text, @"https?://[^\s""'<>]+");
        if (matches.Count > 0) url = matches[0].Value;
        if (url == "")
            Block.RemoveSubStyle(AppendStyleType.Link, SelectStartIndex, SelectEndIndex);
        else
        {
            AppendLink style = new AppendLink { Url = url };
            Block.SetSubStyle(style, SelectStartIndex, SelectEndIndex);
        }
        PropertyChanged?.Invoke();
    }

    private void Font_SelectionChanged(string font)
    {
        Block.FontFamily = font;
        Block.ClearSubStyle(AppendStyleType.Font);
        PropertyChanged?.Invoke();
    }

    private void Size_TextChanged(string text)
    {
        if (int.TryParse(text, out int size))
        {
            if (size < 1 || size > 512)
            {
                WM.ShowErrorTip("有效字号范围：1 - 512");
                return;
            }
            Block.FontSize = size;
            PropertyChanged?.Invoke();
        }
    }

    private void Color_ColorChanged(Color color)
    {
        Block.Color = $"{color.R:X2}{color.G:X2}{color.B:X2}";
        Block.ClearSubStyle(AppendStyleType.Color);
        PropertyChanged?.Invoke();
    }

    private void Style_BoxChecked(string name)
    {
        if (name == "Bold") Block.ClearSubStyle(AppendStyleType.Bold);
        else Block.ClearSubStyle(AppendStyleType.Italic);
        UpdateTextStyle();
    }

    private void Bar_Style_BoxUnchecked(string name)
    {
        if (name == "Bold") Block.ClearSubStyle(AppendStyleType.Bold);
        else Block.ClearSubStyle(AppendStyleType.Italic);
        UpdateTextStyle();
    }

    private void CustonIndent_Opened()
    {
        Block.UseCustomFirstLineIndent = true;
        PropertyChanged?.Invoke();
    }

    private void CustonIndent_Closed()
    {
        Block.UseCustomFirstLineIndent = false;
        PropertyChanged?.Invoke();
    }

    private void Indent_TextChanged(string text)
    {
        if (int.TryParse(text, out int indent))
        {
            Block.CustomFirstLineIndent = indent;
            PropertyChanged?.Invoke();
        }
    }

    private void LineSpace_TextChanged(string text)
    {
        if (int.TryParse(text, out int lineSpace))
        {
            if (lineSpace < 0) return;
            Block.LineSpace = lineSpace;
            PropertyChanged?.Invoke();
        }
    }

    private void MarginTop_TextChanged(string text)
    {
        if (int.TryParse(text, out int marginTop))
        {
            if (marginTop < 0) return;
            Block.MarginTop = marginTop;
            PropertyChanged?.Invoke();
        }
    }

    private void MarginBottom_TextChanged(string text)
    {
        if (int.TryParse(text, out int marginBottom))
        {
            if (marginBottom < 0) return;
            Block.MarginBottom = marginBottom;
            PropertyChanged?.Invoke();
        }
    }

    private void UpdateTextStyle()
    {
        // 获取按钮状态
        bool isBold = Bar_Style.GetChecked("Bold");
        bool isItalic = Bar_Style.GetChecked("Italic");
        // 更新文本样式
        if (isBold && isItalic) Block.TStyle = TextStyle.BoldItalic;
        else if (isBold) Block.TStyle = TextStyle.Bold;
        else if (isItalic) Block.TStyle = TextStyle.Italic;
        else Block.TStyle = TextStyle.Normal;
        // 通知属性变更
        PropertyChanged?.Invoke();
    }
}