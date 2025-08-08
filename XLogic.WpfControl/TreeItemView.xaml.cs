using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XLogic.WpfControl
{
    public partial class TreeItemView : UserControl
    {
        public TreeItemView()
        {
            InitializeComponent();
        }

        public TreeItem? Instance { get; set; } = null;

        #region 事件

        /// <summary>当项展开时</summary>
        public Action? OnItemExpand { get; set; }

        /// <summary>当项折叠时</summary>
        public Action? OnItemFurl { get; set; }

        #endregion

        public void Update()
        {
            // 显示或隐藏控件
            if (Instance == null)
            {
                Visibility = Visibility.Hidden;
                return;
            }
            Visibility = Visibility.Visible;
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

        #region 控件事件

        private void Back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Instance == null) return;

            // 如果是可展开项，并进行了双击
            if (Instance.CanExpand && e.ClickCount == 2)
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
    }
}