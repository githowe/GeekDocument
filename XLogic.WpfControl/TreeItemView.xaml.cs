using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace XLogic.WpfControl
{
    public partial class TreeItemView : UserControl
    {
        #region 构造方法

        public TreeItemView() => InitializeComponent();

        #endregion

        #region 属性

        public TreeItem? Instance { get; set; } = null;

        #endregion

        #region 事件

        public Action<TreeItem>? OnItemEnter { get; set; }

        public Action<TreeItem>? OnItemLeave { get; set; }

        /// <summary>当项展开时</summary>
        public Action? OnItemExpand { get; set; }

        /// <summary>当项折叠时</summary>
        public Action? OnItemFurl { get; set; }

        /// <summary>当命中项时</summary>
        public Action<TreeItem>? OnItemHited { get; set; }

        /// <summary>当双击项时</summary>
        public Action<TreeItem>? OnDoubleClick { get; set; }

        #endregion

        #region 公开方法

        public void Update()
        {
            // 显示或隐藏控件
            if (Instance == null)
            {
                Visibility = Visibility.Hidden;
                return;
            }
            Visibility = Visibility.Visible;
            // 背景
            UpdateBackground();
            // 左边距
            MainGrid.Margin = new Thickness(Instance.Deep * 23, 0, 0, 0);
            // 箭头
            if (Instance.CanExpand && Instance.ItemList.Count > 0) Arrow.Visibility = Visibility.Visible;
            else Arrow.Visibility = Visibility.Hidden;
            Arrow.IsChecked = Instance.IsExpanded;
            // 图标
            Image_Icon.Source = Instance.Icon;
            // 文本
            TB_Text.Text = Instance.Text;
        }

        #endregion

        #region 控件事件

        private void Back_MouseEnter(object sender, MouseEventArgs e)
        {
            _isMouseHover = true;
            UpdateBackground();
            OnItemEnter?.Invoke(Instance);
        }

        private void Back_MouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseHover = false;
            UpdateBackground();
            OnItemLeave?.Invoke(Instance);
        }

        private void Back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Instance == null) return;

            OnItemHited?.Invoke(Instance);
            // 双击
            if (e.ClickCount == 2)
            {
                // 可展开项
                if (Instance.CanExpand)
                {
                    if (Arrow.IsChecked == true)
                    {
                        Instance.IsExpanded = false;
                        OnItemFurl?.Invoke();
                    }
                    else if (Arrow.IsChecked == false)
                    {
                        Instance.IsExpanded = true;
                        OnItemExpand?.Invoke();
                    }
                }
                else OnDoubleClick?.Invoke(Instance);
            }

            e.Handled = true;
        }

        private void Arrow_Click(object sender, RoutedEventArgs e)
        {
            if (Instance == null) return;
            if (Arrow.IsChecked == true)
            {
                Instance.IsExpanded = true;
                OnItemExpand?.Invoke();
            }
            else if (Arrow.IsChecked == false)
            {
                Instance.IsExpanded = false;
                OnItemFurl?.Invoke();
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 更新背景
        /// </summary>
        private void UpdateBackground()
        {
            Back.Background = _default;
            if (_isMouseHover) Back.Background = _hover;
            if (Instance != null && Instance.IsSelected) Back.Background = _selected;
        }

        #endregion

        #region 字段

        /// <summary>鼠标悬停</summary>
        private bool _isMouseHover = false;

        /// <summary>默认背景</summary>
        private readonly Brush _default = Brushes.Transparent;
        /// <summary>悬停时背景</summary>
        private readonly Brush _hover = new SolidColorBrush(Color.FromArgb(25, 255, 255, 255));
        /// <summary>选中时背景</summary>
        private readonly Brush _selected = new SolidColorBrush(Color.FromArgb(51, 255, 255, 255));

        #endregion
    }
}