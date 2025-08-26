using System.Windows.Controls;

namespace GeekDocument.SubSystem.OptionSystem.Panel
{
    public partial class ViewOptionPanel : UserControl
    {
        public ViewOptionPanel() => InitializeComponent();

        public void Init()
        {
            ViewOption viewOption = Options.Instance.View;
            Check_PageBorder.IsChecked = viewOption.ShowPageBorder;
            Check_PaddingMark.IsChecked = viewOption.ShowPaddingMark;
            Check_RowLine.IsChecked = viewOption.ShowRowLine;
        }
    }
}