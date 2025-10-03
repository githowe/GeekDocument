using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GeekDocument.SubSystem.EditerSystem3.PropertyBar
{
    public partial class RadioButtonBar : PropertyBarBase
    {
        public RadioButtonBar() => InitializeComponent();

        public event Action<string> ButtonChecked;

        public void LoadProperty()
        {
            Block_Title.Text = Title;
        }

        public void AddRadioButton(ImageSource? icon, string name)
        {
            // 创建单选框
            RadioButton button = new RadioButton
            {
                Width = 25,
                Height = 25,
                Content = new Image { Source = icon, Width = 15, Height = 15 },
                Name = name,
                Style = Application.Current.FindResource("RadioToolButton2") as Style,
                Foreground = new SolidColorBrush(Color.FromRgb(249, 202, 124)),
            };
            // 添加单选框
            Stack_RadioList.Children.Add(button);
            // 监听单选框
            button.Checked += Button_Checked;
        }

        public void SetChecked(string name)
        {
            foreach (var item in Stack_RadioList.Children)
            {
                if (item is RadioButton radioButton && radioButton.Name == name)
                {
                    _updateOnly = true;
                    radioButton.IsChecked = true;
                    _updateOnly = false;
                }
            }
        }

        private void MainGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Block_Title.Foreground = _hovered;
        }

        private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Block_Title.Foreground = _default;
        }

        private void Button_Checked(object sender, RoutedEventArgs e)
        {
            if (_updateOnly) return;
            if (sender is RadioButton button) ButtonChecked?.Invoke(button.Name);
        }

        private bool _updateOnly = false;
    }
}