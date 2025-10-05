using System.Windows;
using XLogic.Wpf.Window;

namespace GeekDocument.SubSystem.EditerSystem3
{
    public partial class InsertFormulaDialog : XDialog
    {
        public InsertFormulaDialog()
        {
            InitializeComponent();
            OK.Click += OK_Click;
        }

        public string Latex { get; set; } = "";

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Latex = Input_Latex.Text;
            DialogResult = true;
        }
    }
}