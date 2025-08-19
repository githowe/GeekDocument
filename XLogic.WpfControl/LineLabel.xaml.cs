using System.Windows.Controls;
using System.Windows.Media;

namespace XLogic.WpfControl
{
    public partial class LineLabel : UserControl
    {
        public LineLabel() => InitializeComponent();

        public string Text
        {
            get => Block_Text.Text;
            set => Block_Text.Text = value;
        }

        public Brush TextColor
        {
            get => Block_Text.Foreground;
            set => Block_Text.Foreground = value;
        }

        public Brush LineColor
        {
            get => _lineColor;
            set
            {
                _lineColor = value;
                Line_Left.Background = _lineColor;
                Line_Right.Background = _lineColor;
            }
        }

        private Brush _lineColor = Brushes.White;
    }
}