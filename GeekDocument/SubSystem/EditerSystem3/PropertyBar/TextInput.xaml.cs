using System.Windows.Input;

namespace GeekDocument.SubSystem.EditerSystem3.PropertyBar
{
    public partial class TextInput : PropertyBarBase
    {
        public TextInput()
        {
            InitializeComponent();
            Input_Value.KeyDown += Input_Value_KeyDown;
            Input_Value.LostFocus += Input_Value_LostFocus;
        }

        public string Text { get; set; } = "";

        public bool ReadOnly { get; set; } = false;

        public event Action<string> TextChanged;

        /// <summary>
        /// 加载属性
        /// </summary>
        public void LoadProperty(string text)
        {
            Block_Title.Text = Title;
            Input_Value.Text = text;
            if (ReadOnly) Input_Value.IsReadOnly = true;
        }

        private void MainGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Block_Title.Foreground = _hovered;
        }

        private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Block_Title.Foreground = _default;
        }

        private void Input_Value_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Input_Value.Text == Text) return;

                TextChanged?.Invoke(Input_Value.Text);
                Text = Input_Value.Text;

                e.Handled = true;
            }
        }

        private void Input_Value_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Input_Value.Text == Text) return;
            TextChanged?.Invoke(Input_Value.Text);
            Text = Input_Value.Text;
        }
    }
}