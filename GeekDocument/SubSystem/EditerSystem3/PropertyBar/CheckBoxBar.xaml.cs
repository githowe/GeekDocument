using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GeekDocument.SubSystem.EditerSystem3.PropertyBar
{
    public partial class CheckBoxBar : PropertyBarBase
    {
        public CheckBoxBar() => InitializeComponent();

        public event Action<string> BoxChecked;

        public event Action<string> BoxUnchecked;

        public void LoadProperty()
        {
            Block_Title.Text = Title;
        }

        public void AddCheckBox(ImageSource? icon, string name)
        {
            // 创建复选框
            CheckBox box = new CheckBox
            {
                Width = 25,
                Height = 25,
                Content = new Image { Source = icon, Width = 15, Height = 15 },
                Name = name,
                Style = Application.Current.FindResource("LayoutToggle") as Style,
                Foreground = new SolidColorBrush(Color.FromRgb(249, 202, 124)),
            };
            if (Stack_CheckBoxList.Children.Count > 0) box.Margin = new Thickness(2, 0, 0, 0);
            // 添加复选框
            Stack_CheckBoxList.Children.Add(box);
            // 监听复选框
            box.Checked += Box_Checked;
            box.Unchecked += Box_Unchecked;
        }

        public void SetChecked(string name, bool isChecked)
        {
            foreach (var item in Stack_CheckBoxList.Children)
            {
                if (item is CheckBox checkBox && checkBox.Name == name)
                {
                    _updateOnly = true;
                    checkBox.IsChecked = isChecked;
                    _updateOnly = false;
                }
            }
        }

        public bool GetChecked(string name)
        {
            foreach (var item in Stack_CheckBoxList.Children)
            {
                if (item is CheckBox checkBox && checkBox.Name == name)
                    return checkBox.IsChecked == true;
            }
            return false;
        }

        private void MainGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Block_Title.Foreground = _hovered;
        }

        private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Block_Title.Foreground = _default;
        }

        private void Box_Checked(object sender, RoutedEventArgs e)
        {
            if (_updateOnly) return;
            if (sender is CheckBox checkBox) BoxChecked?.Invoke(checkBox.Name);
        }

        private void Box_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_updateOnly) return;
            if (sender is CheckBox checkBox) BoxUnchecked?.Invoke(checkBox.Name);
        }

        private bool _updateOnly = false;
    }
}