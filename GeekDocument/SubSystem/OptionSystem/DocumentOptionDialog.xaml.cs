using GeekDocument.SubSystem.CacheSystem;
using GeekDocument.SubSystem.EditerSystem.Define;
using System.Windows;
using XLogic.Wpf.Window;

namespace GeekDocument.SubSystem.OptionSystem
{
    public partial class DocumentOptionDialog : XDialog
    {
        public DocumentOptionDialog() => InitializeComponent();

        public void Init(Document document)
        {
            // 页面宽度
            Panel_PageOption.Input_PageWidth.Text = document.PageWidth.ToString();
            // 内边距
            Panel_PageOption.Input_Padding_Top.Text = document.Padding.Top.ToString();
            Panel_PageOption.Input_Padding_Bottom.Text = document.Padding.Bottom.ToString();
            Panel_PageOption.Input_Padding_Left.Text = document.Padding.Left.ToString();
            Panel_PageOption.Input_Padding_Right.Text = document.Padding.Right.ToString();
            // 同时调整所有边距
            Panel_PageOption.Toggle_Link.IsChecked = CacheManager.Instance.Cache.Application.PagePaddingLink;
            // 首行缩进
            Panel_ParagraphOption.Input_FirstLineIndent.Text = document.FirstLineIndent.ToString();
            // 段间距
            Panel_ParagraphOption.Input_ParagraphInterval.Text = document.ParagraphInterval.ToString();
        }

        private void Default_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}