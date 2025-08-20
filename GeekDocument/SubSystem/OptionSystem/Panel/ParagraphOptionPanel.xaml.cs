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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GeekDocument.SubSystem.OptionSystem.Panel
{
    public partial class ParagraphOptionPanel : UserControl
    {
        public ParagraphOptionPanel()
        {
            InitializeComponent();
        }

        public void Init()
        {
            ParagraphOption paragraphOption = Options.Instance.Paragraph;
            Input_FirstLineIndent.Text = paragraphOption.FirstLineIndent.ToString();
            Input_ParagraphInterval.Text = paragraphOption.ParagraphInterval.ToString();
        }
    }
}