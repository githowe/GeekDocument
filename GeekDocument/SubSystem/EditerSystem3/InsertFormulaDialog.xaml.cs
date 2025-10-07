using System.Windows;
using XLogic.Wpf.Window;

namespace GeekDocument.SubSystem.EditerSystem3
{
    public partial class InsertFormulaDialog : XDialog
    {
        public InsertFormulaDialog()
        {
            InitializeComponent();
            Loaded += InsertFormulaDialog_Loaded;
            OK.Click += OK_Click;
        }

        public string Latex { get; set; } = "";

        private void InsertFormulaDialog_Loaded(object sender, RoutedEventArgs e)
        {
            Input_Latex.Focus();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Latex = Input_Latex.Text;
            DialogResult = true;
        }
    }
}