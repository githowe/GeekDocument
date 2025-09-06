using GeekDocument.SubSystem.EditerSystem.Define.BlockDerive;
using GeekDocument.SubSystem.ResourceSystem;
using GeekDocument.SubSystem.WindowSystem;
using System.Windows.Media;

namespace GeekDocument.SubSystem.EditerSystem.Control.PropertyPanel
{
    public partial class FormulaPropertyPanel : PropertyPanel
    {
        public FormulaPropertyPanel() => InitializeComponent();

        public BlockFormula Block { get; set; }

        public override void Init()
        {
            Bar_Latex.LoadProperty(Block.Latex);
            Bar_Size.LoadProperty(Block.Size.ToString());
            byte r = Convert.ToByte(Block.Color.Substring(0, 2), 16);
            byte g = Convert.ToByte(Block.Color.Substring(2, 4 - 2), 16);
            byte b = Convert.ToByte(Block.Color.Substring(4, 6 - 4), 16);
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

        private ImageSource GetIcon(string name) => ImageResManager.Instance.GetIcon15($"{name}.png");
    }
}