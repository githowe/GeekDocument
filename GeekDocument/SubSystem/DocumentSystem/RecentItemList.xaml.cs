using GeekDocument.SubSystem.CacheSystem;
using GeekDocument.SubSystem.CacheSystem.Define;
using GeekDocument.SubSystem.WindowSystem;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using XLogic.Base;
using XLogic.Base.Ex;

namespace GeekDocument.SubSystem.DocumentSystem
{
    public partial class RecentItemList : UserControl
    {
        #region 构造方法

        public RecentItemList()
        {
            InitializeComponent();
            Loaded += RecentItemList_Loaded;
        }

        #endregion

        #region 控件事件

        private void RecentItemList_Loaded(object sender, RoutedEventArgs e)
        {
            // 添加起始线
            AddLine();
            // 匹配控件
            MatchControlList();
            // 更新最近项目
            UpdateRecentProject();

            SizeChanged += RecentItemList_SizeChanged;
            ItemScrollBar.ValueChanged += ScrollBar_ValueChanged;
        }

        private void RecentItemList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.HeightChanged)
            {
                MatchControlList();
                UpdateRecentProject();
            }
        }

        private void MainGrid_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ItemScrollBar.Value -= e.Delta / 120;
        }

        private void ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var scrollBar = (ScrollBar)sender;
            var newValue = Math.Round(e.NewValue, 0);
            if (newValue > scrollBar.Maximum) newValue = scrollBar.Maximum;
            scrollBar.Value = newValue;

            _documentList.Top = (int)newValue;
            UpdateListView();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 匹配控件列表
        /// </summary>
        private void MatchControlList()
        {
            int capacity = (int)MainGrid.ActualHeight / (_itemHeight + 1);
            int current = _controlList.Count;
            int createCount = capacity + 1 - current;

            for (int counter = 0; counter < createCount; counter++)
            {
                // 添加项控件
                RecentItem control = new RecentItem { Height = _itemHeight };
                Stack_ItemList.Children.Add(control);
                // 保存控件引用
                _controlList.Add(control);
                // 监听控件
                control.Click = Item_Click;
                control.MarkClick = Item_MarkClick;
                control.RemoveClick = Item_RemoveClick;
                // 添加分割线
                AddLine();
            }

            _documentList.WindowHeight = (int)MainGrid.ActualHeight / (_itemHeight + 1);
            UpdateScrollBar();
        }

        /// <summary>
        /// 添加横线
        /// </summary>
        private void AddLine()
        {
            Grid line = new Grid
            {
                Height = 1,
                Background = Brushes.White,
                Opacity = 0.12,
            };
            Stack_ItemList.Children.Add(line);
            _lineList.Add(line);
        }

        /// <summary>
        /// 更新最近项目
        /// </summary>
        private void UpdateRecentProject()
        {
            // 更新源数据
            UpdateSourceData();
            // 更新滚动条
            UpdateScrollBar();
            // 更新列表视图
            UpdateListView();
        }

        /// <summary>
        /// 更新源数据
        /// </summary>
        private void UpdateSourceData()
        {
            _documentList.Data = CacheManager.Instance.Cache.DocumentManager.GetSortedList();
        }

        /// <summary>
        /// 更新滚动条
        /// </summary>
        private void UpdateScrollBar()
        {
            int dataCount = _documentList.Data.Count;
            int windowHeight = _documentList.WindowHeight;
            ItemScrollBar.ViewportSize = windowHeight;
            ItemScrollBar.Maximum = dataCount > windowHeight ? dataCount - windowHeight : 0;
        }

        /// <summary>
        /// 更新列表视图
        /// </summary>
        private void UpdateListView()
        {
            // 获取可视数据
            List<RecentDocument> visibleData = _documentList.GetVisibleDataNew((int)MainGrid.ActualHeight % (_itemHeight + 1) - 1 > 0);
            // 刷新项
            for (int index = 0; index < _controlList.Count; index++)
            {
                // 设置实例
                _controlList[index].Instance = index < visibleData.Count ? visibleData[index] : null;
                // 更新项
                _controlList[index].Update();
                // 设置分割线显隐
                _lineList[index + 1].Visibility = index < visibleData.Count ? Visibility.Visible : Visibility.Hidden;
            }
            // 显隐默认项
            DefaultItem.Visibility = visibleData.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnAskFinish(AskDialog dialog, object? obj)
        {
            if (dialog.Result == AskResult.Yes)
            {
                CacheManager.Instance.Cache.DocumentManager.RemoveRecentDocument(((RecentItem)obj).Instance.FullPath);
                CacheManager.Instance.SaveCache();
                UpdateRecentProject();
            }
        }

        #endregion

        #region 项事件

        private void Item_Click(RecentItem sender)
        {
            // 不存在项目文件
            if (!File.Exists(sender.Instance.FullPath))
            {
                (string, string) pathInfo = sender.Instance.FullPath.ParsePath();
                WM.ShowAskWindow($"不存在“{pathInfo.Item2}”。是否从最近打开列表中移除对它的引用？", OnAskFinish, sender, useCancel: false, level: TipLevel.Error);
                return;
            }
            // 打开文档
            WM.Main.OpenDocument(sender.Instance.FullPath);
        }

        private void Item_MarkClick(RecentItem sender)
        {
            sender.Instance.Marked = !sender.Instance.Marked;
            CacheManager.Instance.SaveCache();
            UpdateRecentProject();
        }

        private void Item_RemoveClick(RecentItem sender)
        {
            CacheManager.Instance.Cache.DocumentManager.RemoveRecentDocument(sender.Instance.FullPath);
            CacheManager.Instance.SaveCache();
            UpdateRecentProject();
        }

        #endregion

        #region 字段

        /// <summary>项高</summary>
        private readonly int _itemHeight = 60;

        /// <summary>项控件列表</summary>
        private readonly List<RecentItem> _controlList = new List<RecentItem>();
        /// <summary>横线列表</summary>
        private readonly List<Grid> _lineList = new List<Grid>();
        /// <summary>最近文档数据</summary>
        private readonly DataWindow<RecentDocument> _documentList = new DataWindow<RecentDocument>();

        #endregion
    }
}