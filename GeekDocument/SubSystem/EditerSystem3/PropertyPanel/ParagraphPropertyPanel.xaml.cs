using GeekDocument.AppTool;
using GeekDocument.SubSystem.LayoutEngine;

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
            // 垂直对齐方式
            Bar_AlignV.LoadProperty();
            Bar_AlignV.AddRadioButton(GetIcon("AlignTop"), "Top");
            Bar_AlignV.AddRadioButton(GetIcon("AlignCenter2"), "Center");
            Bar_AlignV.AddRadioButton(GetIcon("AlignBottom"), "Bottom");
            // 首行缩进、左右缩进
            Bar_CustonIndent.LoadProperty(段落.使用自定义首行缩进);
            Bar_Indent.LoadProperty(段落.自定义首行缩进.ToString());
            Bar_LeftIndent.LoadProperty(段落.左缩进.ToString());
            Bar_RightIndent.LoadProperty(段落.右缩进.ToString());
            // 行间距、上边距、下边距
            Bar_LineSpace.LoadProperty(段落.行间距.ToString());
            Bar_MarginTop.LoadProperty(段落.段前距.ToString());
            Bar_MarginBottom.LoadProperty(段落.段后距.ToString());
        }
    }
}