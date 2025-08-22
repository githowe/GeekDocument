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
            PageOption pageOption = new PageOption
            {
                PageWidth = document.PageWidth,
                PagePadding = new PageThickness
                {
                    Top = document.Padding.Top,
                    Bottom = document.Padding.Bottom,
                    Left = document.Padding.Left,
                    Right = document.Padding.Right
                }
            };
            Panel_PageOption.Init(pageOption);
            // 同时调整所有边距
            Panel_PageOption.Toggle_Link.IsChecked = CacheManager.Instance.Cache.Application.PagePaddingLink;
            // 首行缩进、段间距
            Panel_ParagraphOption.Input_FirstLineIndent.Text = document.FirstLineIndent.ToString();
            Panel_ParagraphOption.Input_ParagraphInterval.Text = document.ParagraphInterval.ToString();
        }

        private void Default_Click(object sender, RoutedEventArgs e)
        {
            PageOption page = Options.Instance.Page;
            page.PageWidth = Panel_PageOption.PageWidth;
            page.PagePadding = new PageThickness
            {
                Top = Panel_PageOption.Top,
                Bottom = Panel_PageOption.Bottom,
                Left = Panel_PageOption.Left,
                Right = Panel_PageOption.Right
            };
            ParagraphOption paragraph = Options.Instance.Paragraph;
            paragraph.FirstLineIndent = int.Parse(Panel_ParagraphOption.Input_FirstLineIndent.Text);
            paragraph.ParagraphInterval = int.Parse(Panel_ParagraphOption.Input_ParagraphInterval.Text);
            // 保存选项
            Options.Instance.Save();
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}