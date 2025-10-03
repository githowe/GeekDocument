using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GeekDocument.SubSystem.EditerSystem3.PropertyBar
{
    public partial class ToolBar : PropertyBarBase
    {
        public ToolBar() => InitializeComponent();

        public event Action<string> ToolClick;

        public void LoadProperty()
        {
            Block_Title.Text = Title;
        }

        public Button AddTool(ImageSource? icon, string name, string toolTip = "")
        {
            // 创建按钮
            Button tool = new Button
            {
                Width = 25,
                Height = 25,
                Content = new Image { Source = icon, Width = 15, Height = 15 },
                Name = name,
                Style = Application.Current.FindResource("ToolBarButton") as Style,
            };
            if (toolTip != "") tool.ToolTip = toolTip;
            // 添加按钮
            Stack_ToolList.Children.Add(tool);
            // 监听按钮
            tool.Click += Button_Click;
            // 返回按钮
            return tool;
        }

        private void MainGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Block_Title.Foreground = _hovered;
        }

        private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Block_Title.Foreground = _default;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button) ToolClick?.Invoke(button.Name);
        }
    }
}