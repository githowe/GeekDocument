using GeekDocument.SubSystem.WindowSystem;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GeekDocument.SubSystem.EditerSystem.Control.PropertyBar
{
    public partial class ColorBar : PropertyBarBase
    {
        public ColorBar() => InitializeComponent();

        public Color Color { get; set; } = Colors.White;

        public event Action<Color> ColorChanged;

        public void LoadProperty(byte r, byte g, byte b)
        {
            Block_Title.Text = Title;
            Color = Color.FromRgb(r, g, b);
            Input_Code.Text = $"#{r:X2}{g:X2}{b:X2}";
            Grid_Color.Background = new SolidColorBrush(Color);
        }

        private void MainGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Block_Title.Foreground = _hovered;
        }

        private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Block_Title.Foreground = _default;
        }

        private void Input_Code_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Color color = (Color)ColorConverter.ConvertFromString(Input_Code.Text);
                Grid_Color.Background = new SolidColorBrush(color);
                ColorChanged?.Invoke(color);
            }
            catch (Exception) { }
        }

        private void Grid_Color_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WM.ShowColorPicker(Color, UpdateColor, UpdateColor);
        }

        private void UpdateColor(Color color)
        {
            Color = color;
            Input_Code.Text = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
            Grid_Color.Background = new SolidColorBrush(color);
            ColorChanged?.Invoke(color);
        }
    }
}