using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using XLogic.Wpf.Window;

namespace GeekDocument.SubSystem.OptionSystem
{
    public partial class OptionDialog : XDialog
    {
        public OptionDialog() => InitializeComponent();

        private void XDialog_Loaded(object sender, RoutedEventArgs e)
        {
            Panel_PageOption.Init(Options.Instance.Page);
            Panel_ParagraphOption.Init();
            Panel_ViewOption.Init();
        }
    }
}