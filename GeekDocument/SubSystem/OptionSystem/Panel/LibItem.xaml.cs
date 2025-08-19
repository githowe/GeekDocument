using System.Windows.Controls;
using System.Windows.Media;

namespace GeekDocument.SubSystem.OptionSystem.Panel
{
    public partial class LibItem : UserControl
    {
        public LibItem()
        {
            InitializeComponent();
            _defaultBrush.Freeze();
        }

        public bool IsDefault
        {
            get => _isDefault;
            set
            {
                _isDefault = value;
                if (_isDefault)
                {
                    Block_Title.Foreground = _defaultBrush;
                    Block_Path.Foreground = _defaultBrush;
                }
                else
                {
                    Block_Title.Foreground = Brushes.White;
                    Block_Path.Foreground = Brushes.White;
                }
            }
        }

        private readonly Brush _defaultBrush = new SolidColorBrush(Color.FromRgb(254, 210, 103));

        private bool _isDefault = false;
    }
}