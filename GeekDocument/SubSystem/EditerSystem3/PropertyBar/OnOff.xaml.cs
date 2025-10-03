using System.Windows;
using System.Windows.Input;

namespace GeekDocument.SubSystem.EditerSystem3.PropertyBar
{
    public partial class OnOff : PropertyBarBase
    {
        public OnOff() => InitializeComponent();

        public event Action Opened;

        public event Action Closed;

        public void LoadProperty(bool isChecked)
        {
            Block_Title.Text = Title;
            Check_Value.IsChecked = isChecked;
        }

        private void MainGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Block_Title.Foreground = _hovered;
        }

        private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Block_Title.Foreground = _default;
        }

        private void Check_Value_Click(object sender, RoutedEventArgs e)
        {
            if (Check_Value.IsChecked == true) Opened?.Invoke();
            else if (Check_Value.IsChecked == false) Closed?.Invoke();
        }
    }
}