using System.Windows.Controls;

namespace GeekDocument.SubSystem.OptionSystem.Panel
{
    public partial class ParagraphOptionPanel : UserControl
    {
        public ParagraphOptionPanel() => InitializeComponent();

        public int FirstLineIndent => int.Parse(Input_FirstLineIndent.Text);

        public int ParagraphInterval => int.Parse(Input_ParagraphInterval.Text);

        public void Init()
        {
            ParagraphOption paragraphOption = Options.Instance.Paragraph;
            Input_FirstLineIndent.Text = paragraphOption.FirstLineIndent.ToString();
            Input_ParagraphInterval.Text = paragraphOption.ParagraphInterval.ToString();
        }
    }
}