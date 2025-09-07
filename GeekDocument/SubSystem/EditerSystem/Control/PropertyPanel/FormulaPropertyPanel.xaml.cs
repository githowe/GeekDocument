using GeekDocument.SubSystem.EditerSystem.Define.BlockDerive;
using GeekDocument.SubSystem.WindowSystem;
using XLogic.Base.Ex;

namespace GeekDocument.SubSystem.EditerSystem.Control.PropertyPanel;

public partial class FormulaPropertyPanel : PropertyPanel
{
    public FormulaPropertyPanel() => InitializeComponent();

    public BlockFormula Block { get; set; }

    public override void Init()
    {
        Bar_Latex.LoadProperty(Block.Latex);
        Bar_Size.LoadProperty(Block.Size.ToString());
        var (r, g, b) = Block.Color.ParseColorCode();
        Bar_Color.LoadProperty(r, g, b);
        Bar_Align.LoadProperty();
        Bar_Align.AddRadioButton(GetIcon("AlignLeft"), "Left");
        Bar_Align.AddRadioButton(GetIcon("AlignCenter"), "Center");
        Bar_Align.AddRadioButton(GetIcon("AlignRight"), "Right");
        Bar_Align.SetChecked(Block.Align.ToString());
        Bar_MarginTop.LoadProperty(Block.MarginTop.ToString());
        Bar_MarginBottom.LoadProperty(Block.MarginBottom.ToString());
        Bar_MarginLeft.LoadProperty(Block.MarginLeft.ToString());
        Bar_MarginRight.LoadProperty(Block.MarginRight.ToString());

        Bar_Latex.TextChanged += Latex_TextChanged;
        Bar_Size.TextChanged += Size_TextChanged;
        Bar_MarginTop.TextChanged += MarginTop_TextChanged;
        Bar_MarginBottom.TextChanged += MarginBottom_TextChanged;
    }

    private void Latex_TextChanged(string text)
    {
        Block.Latex = text;
        PropertyChanged?.Invoke();
    }

    private void Size_TextChanged(string text)
    {
        if (int.TryParse(text, out int size))
        {
            if (size < 1 || size > 64)
            {
                WM.ShowErrorTip("有效字号范围：1 - 64");
                return;
            }
            Block.Size = size;
            PropertyChanged?.Invoke();
        }
    }

    private void MarginTop_TextChanged(string text)
    {
        if (int.TryParse(text, out int marginTop))
        {
            Block.MarginTop = marginTop;
            PropertyChanged?.Invoke();
        }
    }

    private void MarginBottom_TextChanged(string text)
    {
        if (int.TryParse(text, out int marginBottom))
        {
            Block.MarginBottom = marginBottom;
            PropertyChanged?.Invoke();
        }
    }
}