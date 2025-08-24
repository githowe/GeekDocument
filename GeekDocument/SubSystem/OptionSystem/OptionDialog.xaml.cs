using GeekDocument.SubSystem.EventSystem;
using System.Windows;
using XLogic.Wpf.Window;

namespace GeekDocument.SubSystem.OptionSystem
{
    public partial class OptionDialog : XDialog
    {
        public OptionDialog() => InitializeComponent();

        private void XDialog_Loaded(object sender, RoutedEventArgs e)
        {
            // 页面选项
            Panel_PageOption.Init(Options.Instance.Page);
            // 段落选项
            Panel_ParagraphOption.Init();
            // 视图选项
            Panel_ViewOption.Init();
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            // 应用页面选项
            PageOption page = Options.Instance.Page;
            page.PageWidth = Panel_PageOption.PageWidth;
            page.PagePadding.Top = Panel_PageOption.Top;
            page.PagePadding.Bottom = Panel_PageOption.Bottom;
            page.PagePadding.Left = Panel_PageOption.Left;
            page.PagePadding.Right = Panel_PageOption.Right;
            // 应用段落选项
            ParagraphOption paragraph = Options.Instance.Paragraph;
            paragraph.FirstLineIndent = Panel_ParagraphOption.FirstLineIndent;
            paragraph.ParagraphInterval = Panel_ParagraphOption.ParagraphInterval;
            // 应用视图选项
            ViewOption view = Options.Instance.View;
            view.ShowPaddingMark = Panel_ViewOption.Check_PaddingMark.IsChecked == true;
            view.ShowRowLine = Panel_ViewOption.Check_RowLine.IsChecked == true;
            // 保存选项、关闭对话框、触发事件
            Options.Instance.Save();
            Close();
            EM.Instance.Invoke(EventType.Option_Changed);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}