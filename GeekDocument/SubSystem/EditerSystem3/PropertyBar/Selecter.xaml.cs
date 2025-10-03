using System.Windows.Controls;
using System.Windows.Input;

namespace GeekDocument.SubSystem.EditerSystem3.PropertyBar
{
    public partial class Selecter : PropertyBarBase
    {
        public Selecter()
        {
            InitializeComponent();
        }

        public string Value
        {
            get
            {
                int index = Box_ItemList.SelectedIndex;
                ComboBoxItem boxItem = (ComboBoxItem)Box_ItemList.Items[index];
                return boxItem.Content.ToString();
            }
        }

        public event Action<string> SelectionChanged;

        public void LoadProperty(List<string> range, string value)
        {
            Block_Title.Text = Title;
            foreach (var item in range)
            {
                ComboBoxItem boxItem = new ComboBoxItem { Content = item };
                Box_ItemList.Items.Add(boxItem);
            }
            Box_ItemList.SelectedIndex = range.IndexOf(value);
        }

        private void MainGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Block_Title.Foreground = _hovered;
        }

        private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Block_Title.Foreground = _default;
        }

        private void Box_ItemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Box_ItemList.SelectedItem is ComboBoxItem item)
                SelectionChanged?.Invoke((string)item.Content);
        }
    }
}