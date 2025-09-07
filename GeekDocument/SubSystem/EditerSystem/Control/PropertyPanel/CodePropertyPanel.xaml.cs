using GeekDocument.SubSystem.EditerSystem.Define.BlockDerive;
using GeekDocument.SubSystem.WindowSystem;
using System.Drawing.Text;

namespace GeekDocument.SubSystem.EditerSystem.Control.PropertyPanel;

public partial class CodePropertyPanel : PropertyPanel
{
    public CodePropertyPanel() => InitializeComponent();

    public BlockCode Block { get; set; }

    public override void Init()
    {
        // 语言
        Bar_Language.LoadProperty(Block.Language);
        // 获取已安装字体列表
        List<string> fontNameList = new List<string>();
        InstalledFontCollection fonts = new InstalledFontCollection();
        foreach (var font in fonts.Families) fontNameList.Add(font.Name);
        // 字体、字号
        Bar_Font.LoadProperty(fontNameList, Block.FontFamily);
        Bar_Font2.LoadProperty(fontNameList, Block.SecondFontFamily);
        Bar_Size.LoadProperty(Block.FontSize.ToString());
        // 行间距、上边距、下边距
        Bar_LineSpace.LoadProperty(Block.LineSpace.ToString());
        Bar_MarginTop.LoadProperty(Block.MarginTop.ToString());
        Bar_MarginBottom.LoadProperty(Block.MarginBottom.ToString());
        // 显示行号、语言
        Bar_ShowLineNumber.LoadProperty(Block.ShowLineNumber);
        Bar_ShowLanguage.LoadProperty(Block.ShowLanguage);

        // 监听语言
        Bar_Language.TextChanged += Language_TextChanged;
        // 监听字体、字号
        Bar_Font.SelectionChanged += Font_SelectionChanged;
        Bar_Font2.SelectionChanged += Font2_SelectionChanged;
        Bar_Size.TextChanged += Size_TextChanged;
        // 监听行间距、上边距、下边距
        Bar_LineSpace.TextChanged += LineSpace_TextChanged;
        Bar_MarginTop.TextChanged += MarginTop_TextChanged;
        Bar_MarginBottom.TextChanged += MarginBottom_TextChanged;
        // 监听显示行号、语言
        Bar_ShowLineNumber.Opened += ShowLineNumber_Opened;
        Bar_ShowLineNumber.Closed += ShowLineNumber_Closed;
        Bar_ShowLanguage.Opened += ShowLanguage_Opened;
        Bar_ShowLanguage.Closed += ShowLanguage_Closed;
    }

    private void Language_TextChanged(string text)
    {
        Block.Language = text;
        Block.UpdateLanguageLine();
        PropertyChanged?.Invoke();
    }

    private void Font_SelectionChanged(string text)
    {
        Block.FontFamily = text;
        PropertyChanged?.Invoke();
    }

    private void Font2_SelectionChanged(string text)
    {

    }

    private void Size_TextChanged(string text)
    {
        if (int.TryParse(text, out int size))
        {
            if (size < 1 || size > 32)
            {
                WM.ShowErrorTip("有效字号范围：1 - 32");
                return;
            }
            Block.FontSize = size;
            Block.UpdateLanguageLine();
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

    private void ShowLineNumber_Opened()
    {
        Block.ShowLineNumber = true;
        PropertyChanged?.Invoke();
    }

    private void ShowLineNumber_Closed()
    {
        Block.ShowLineNumber = false;
        PropertyChanged?.Invoke();
    }

    private void ShowLanguage_Opened()
    {
        Block.ShowLanguage = true;
        PropertyChanged?.Invoke();
    }

    private void ShowLanguage_Closed()
    {
        Block.ShowLanguage = false;
        PropertyChanged?.Invoke();
    }
}