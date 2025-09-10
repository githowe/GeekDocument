using GeekDocument.SubSystem.WindowSystem;
using System.Text.RegularExpressions;
using System.Windows;
using XLogic.Wpf.Window;

namespace GeekDocument.SubSystem.EditerSystem.Core
{
    public partial class InsertLinkDialog : XDialog
    {
        public InsertLinkDialog()
        {
            InitializeComponent();
            Loaded += InsertLinkDialog_Loaded;
        }

        public string Url { get; set; } = "";

        private void InsertLinkDialog_Loaded(object sender, RoutedEventArgs e)
        {
            Input_Url.Text = Url;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (Input_Url.Text.Trim() == "")
            {
                WM.ShowErrorTip("链接不能为空", this);
                return;
            }
            MatchCollection matches = Regex.Matches(Input_Url.Text, @"https?://[^\s""'<>]+");
            if (matches.Count == 0)
            {
                WM.ShowErrorTip("链接格式不正确", this);
                return;
            }
            Url = Input_Url.Text.Trim();
            DialogResult = true;
        }
    }
}