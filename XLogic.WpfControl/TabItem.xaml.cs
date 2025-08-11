using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using TabContrlItem = System.Windows.Controls.TabItem;

namespace XLogic.WpfControl
{
    public partial class TabItem : UserControl
    {
        #region 构造方法

        public TabItem() => InitializeComponent();

        #endregion

        #region 属性

        public string Header
        {
            get => _header;
            set
            {
                _header = value;
                Block_Title.Text = _header;
            }
        }

        public bool Selected { get; set; } = false;

        public bool Actived { get; set; } = false;

        public TabContrlItem? ItemInstance { get; set; } = null;

        #endregion

        #region 事件

        public Action<TabItem>? Click { get; set; } = null;

        public Action<TabItem>? Close { get; set; } = null;

        #endregion

        #region 公开方法

        public void UpdateItem()
        {
            UpdateBackground();
            UpdateForeground();
        }

        #endregion

        #region 控件事件

        private void MainGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            _isMouseHover = true;
            UpdateBackground();
            Button_Close.Visibility = System.Windows.Visibility.Visible;
        }

        private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseHover = false;
            UpdateBackground();
            Button_Close.Visibility = System.Windows.Visibility.Hidden;
        }

        private void MainGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Click?.Invoke(this);
        }

        private void Button_Close_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Close?.Invoke(this);
        }

        #endregion

        #region 私有方法

        private void UpdateBackground()
        {
            Grid_Back.Background = _default;
            if (_isMouseHover) Grid_Back.Background = _hover;
            if (Selected) Grid_Back.Background = _selected;
        }

        private void UpdateForeground()
        {
            Block_Title.Foreground = _textDefault;
            if (Selected) Block_Title.Foreground = _textSelected;
        }

        #endregion

        #region 字段

        /// <summary>鼠标悬停</summary>
        private bool _isMouseHover = false;

        /// <summary>默认背景</summary>
        private readonly Brush _default = Brushes.Transparent;
        /// <summary>悬停时背景</summary>
        private readonly Brush _hover = new SolidColorBrush(Color.FromArgb(255, 53, 53, 53));
        /// <summary>选中时背景</summary>
        private readonly Brush _selected = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30));

        private readonly Brush _textDefault = new SolidColorBrush(Color.FromArgb(64, 255, 255, 255));
        private readonly Brush _textSelected = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

        private string _header = "标签标题";

        #endregion
    }
}