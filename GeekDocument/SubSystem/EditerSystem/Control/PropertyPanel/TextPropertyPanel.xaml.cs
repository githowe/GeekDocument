using GeekDocument.SubSystem.EditerSystem.Define;
using GeekDocument.SubSystem.EditerSystem.Define.BlockDerive;
using GeekDocument.SubSystem.ResourceSystem;
using GeekDocument.SubSystem.WindowSystem;
using System.Drawing.Text;
using System.Windows.Media;

namespace GeekDocument.SubSystem.EditerSystem.Control.PropertyPanel
{
    public partial class TextPropertyPanel : PropertyPanel
    {
        public TextPropertyPanel() => InitializeComponent();

        #region 属性

        public BlockText Block { get; set; }

        #endregion

        public override void Init()
        {
            // 获取已安装字体列表
            List<string> fontNameList = new List<string>();
            InstalledFontCollection fonts = new InstalledFontCollection();
            foreach (var font in fonts.Families) fontNameList.Add(font.Name);
            // 字体、字号
            Bar_Font.LoadProperty(fontNameList, Block.FontFamily);
            Bar_Size.LoadProperty(Block.FontSize.ToString());
            // 颜色
            byte r = Convert.ToByte(Block.Color.Substring(0, 2), 16);
            byte g = Convert.ToByte(Block.Color.Substring(2, 4 - 2), 16);
            byte b = Convert.ToByte(Block.Color.Substring(4, 6 - 4), 16);
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
            // 行间距、段前距、段后距
            Bar_LineSpace.LoadProperty(Block.LineSpace.ToString());
            Bar_MarginTop.LoadProperty("0");
            Bar_MarginBottom.LoadProperty("0");

            // 监听字体、字号
            Bar_Font.SelectionChanged += Font_SelectionChanged;
            Bar_Size.TextChanged += Size_TextChanged;
            // 监听样式
            Bar_Style.BoxChecked += Style_BoxChecked;
            Bar_Style.BoxUnchecked += Bar_Style_BoxUnchecked;
            // 监听缩进
            Bar_CustonIndent.Opened += CustonIndent_Opened;
            Bar_CustonIndent.Closed += CustonIndent_Closed;
            Bar_Indent.TextChanged += Indent_TextChanged;
            // 监听行间距
            Bar_LineSpace.TextChanged += LineSpace_TextChanged;
        }

        private void Font_SelectionChanged(string font)
        {
            Block.FontFamily = font;
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

        private void Style_BoxChecked(string name)
        {
            UpdateTextStyle();
        }

        private void Bar_Style_BoxUnchecked(string name)
        {
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
                Block.LineSpace = lineSpace;
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

        private ImageSource GetIcon(string name) => ImageResManager.Instance.GetIcon15($"{name}.png");
    }
}