using GeekDocument.AppTool;
using GeekDocument.SubSystem.LayoutEngine;
using GeekDocument.SubSystem.WindowSystem;
using System.Windows.Media;

namespace GeekDocument.SubSystem.EditerSystem3.PropertyPanel
{
    public partial class ParagraphPropertyPanel : PropertyPanel
    {
        public ParagraphPropertyPanel() => InitializeComponent();

        public 段落 段落 { get; set; } = null!;

        public override void Init()
        {
            // 字体、字号、颜色
            Bar_Font.LoadProperty(FontManager.FontList, 段落.字体);
            Bar_Size.LoadProperty(段落.字号.ToString());
            Bar_Color.LoadProperty(255, 255, 255);
            // 样式
            Bar_Style.LoadProperty();
            Bar_Style.AddCheckBox(GetIcon("Bold"), "Bold");
            Bar_Style.AddCheckBox(GetIcon("Italic"), "Italic");
            // 水平对齐方式
            Bar_AlignH.LoadProperty();
            Bar_AlignH.AddRadioButton(GetIcon("AlignLeft"), "Left");
            Bar_AlignH.AddRadioButton(GetIcon("AlignCenter"), "Center");
            Bar_AlignH.AddRadioButton(GetIcon("AlignRight"), "Right");
            Bar_AlignH.AddRadioButton(GetIcon("AlignJustify"), "Justify");
            Bar_AlignH.SetChecked(段落.水平对齐.ToString());
            // 垂直对齐方式
            Bar_AlignV.LoadProperty();
            Bar_AlignV.AddRadioButton(GetIcon("AlignTop"), "Top");
            Bar_AlignV.AddRadioButton(GetIcon("AlignCenter2"), "Center");
            Bar_AlignV.AddRadioButton(GetIcon("AlignBottom"), "Bottom");
            Bar_AlignV.SetChecked(段落.垂直对齐.ToString());
            // 首行缩进、左右缩进
            Bar_CustonIndent.LoadProperty(段落.使用自定义首行缩进);
            Bar_Indent.LoadProperty(段落.自定义首行缩进.ToString());
            Bar_LeftIndent.LoadProperty(段落.左缩进.ToString());
            Bar_RightIndent.LoadProperty(段落.右缩进.ToString());
            // 段间距
            Bar_CustonParagraphSpace.LoadProperty(段落.使用自定义段间距);
            Bar_ParagraphSpace.LoadProperty(段落.自定义段间距.ToString());
            // 行间距、段前距、段后距
            Bar_LineSpace.LoadProperty(段落.行间距.ToString());
            Bar_MarginTop.LoadProperty(段落.段前距.ToString());
            Bar_MarginBottom.LoadProperty(段落.段后距.ToString());

            // 监听字体、字号、颜色
            Bar_Font.SelectionChanged += Font_SelectionChanged;
            Bar_Size.TextChanged += Size_TextChanged;
            Bar_Color.ColorChanged += Color_ColorChanged;
            // 监听样式
            Bar_Style.BoxChecked += Style_BoxChecked;
            Bar_Style.BoxUnchecked += Style_BoxUnchecked;
            // 监听对齐方式
            Bar_AlignH.ButtonChecked += AlignH_ButtonChecked;
            Bar_AlignV.ButtonChecked += AlignV_ButtonChecked;
            // 监听首行缩进、左右缩进
            Bar_CustonIndent.Opened += CustonIndent_Opened;
            Bar_CustonIndent.Closed += CustonIndent_Closed;
            Bar_Indent.TextChanged += Indent_TextChanged;
            Bar_LeftIndent.TextChanged += LeftIndent_TextChanged;
            Bar_RightIndent.TextChanged += RightIndent_TextChanged;
            // 监听行间距、段前距、段后距
            Bar_LineSpace.TextChanged += LineSpace_TextChanged;
            Bar_MarginTop.TextChanged += MarginTop_TextChanged;
            Bar_MarginBottom.TextChanged += MarginBottom_TextChanged;
        }

        /// <summary>
        /// 字体
        /// </summary>
        private void Font_SelectionChanged(string text)
        {

        }

        /// <summary>
        /// 字号
        /// </summary>
        private void Size_TextChanged(string text)
        {
            if (int.TryParse(text, out int size))
            {
                if (size < 1 || size > 512)
                {
                    WM.ShowErrorTip("有效字号范围：1 - 512");
                    return;
                }
                段落.更新字号(size);
            }
        }

        /// <summary>
        /// 颜色
        /// </summary>
        private void Color_ColorChanged(Color color)
        {

        }

        private void Style_BoxChecked(string name)
        {

        }

        private void Style_BoxUnchecked(string name)
        {

        }

        private void AlignH_ButtonChecked(string name)
        {
            水平对齐方式 水平对齐 = Enum.Parse<水平对齐方式>(name);
            段落.更新水平对齐方式(水平对齐);
        }

        private void AlignV_ButtonChecked(string name)
        {
            垂直对齐方式 垂直对齐 = Enum.Parse<垂直对齐方式>(name);
            段落.更新垂直对齐方式(垂直对齐);
        }

        private void CustonIndent_Opened()
        {
            段落.更新使用自定义首行缩进(true);
        }

        private void CustonIndent_Closed()
        {
            段落.更新使用自定义首行缩进(false);
        }

        private void Indent_TextChanged(string text)
        {
            if (double.TryParse(text, out double indent))
            {
                段落.更新自定义首行缩进(indent);
            }
        }

        private void LeftIndent_TextChanged(string text)
        {
            if (double.TryParse(text, out double indent))
                段落.更新左缩进(indent);
        }

        private void RightIndent_TextChanged(string text)
        {
            if (double.TryParse(text, out double indent))
                段落.更新右缩进(indent);
        }

        private void LineSpace_TextChanged(string text)
        {

        }

        private void MarginTop_TextChanged(string text)
        {

        }

        private void MarginBottom_TextChanged(string text)
        {

        }
    }
}