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
    public partial class PageOptionPanel : UserControl
    {
        public PageOptionPanel()
        {
            InitializeComponent();
        }

        public void Init()
        {
            PageOption pageOption = Options.Instance.Page;
            Input_PageWidth.Text = pageOption.PageWidth.ToString();
            Input_Padding_Top.Text = pageOption.PageMargin.Top.ToString();
            Input_Padding_Bottom.Text = pageOption.PageMargin.Bottom.ToString();
            Input_Padding_Left.Text = pageOption.PageMargin.Left.ToString();
            Input_Padding_Right.Text = pageOption.PageMargin.Right.ToString();
        }
    }
}