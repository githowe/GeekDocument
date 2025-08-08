using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace XLogic.WpfControl
{
    public partial class TreeView : UserControl
    {
        #region 构造方法

        public TreeView() => InitializeComponent();

        #endregion

        #region 属性

        /// <summary>树根</summary>
        public TreeItem TreeRoot { get; set; } = new TreeItem();

        #endregion

        #region 公开方法

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            MatchControlList();
            SizeChanged += TreeView_SizeChanged;
            MainScrollBar.ValueChanged += MainScrollBar_ValueChanged;
            MainGrid.MouseWheel += MainGrid_MouseWheel;
        }

        /// <summary>
        /// 更新项列表
        /// </summary>
        public void UpdateItemList()
        {
            _itemList.Clear();
            LoadTreeItem(TreeRoot);
        }

        /// <summary>
        /// 更新项视图
        /// </summary>
        public void UpdateItemView()
        {
            // 计算可视区域能显示的最大项数量
            int visiableMax = ((int)MainGrid.ActualHeight - 20) / _controlHeight;
            if ((MainGrid.ActualHeight - 20) % _controlHeight > 0) visiableMax++;
            // 计算最后一个索引
            int lastIndex = _firstIndex + visiableMax;
            if (lastIndex > _itemList.Count) lastIndex = _itemList.Count;
            // 计算索引范围
            int rangeLength  = lastIndex - _firstIndex;
            // 获取可视项列表
            List<TreeItem> visiableItemList = _itemList.GetRange(_firstIndex, rangeLength);
            // 更新每个控件
            for (int index = 0; index < _controlList.Count; index++)
            {
                _controlList[index].Instance = index < visiableItemList.Count ? visiableItemList[index] : null;
                _controlList[index].Update();
            }
        }

        /// <summary>
        /// 更新滚动条
        /// </summary>
        private void UpdateScrollBar()
        {
            if (MainGrid.ActualHeight == 0) return;

            MainScrollBar.ViewportSize = ((int)MainGrid.ActualHeight - 20) / _controlHeight;
            // 数据量 > 可视数据量
            if (_itemList.Count > MainScrollBar.ViewportSize)
                MainScrollBar.Maximum = _itemList.Count - MainScrollBar.ViewportSize;
            else MainScrollBar.Maximum = 0;
        }

        #endregion

        #region 控件事件

        private void TreeView_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (e.HeightChanged)
            {
                MatchControlList();
                UpdateItemView();
                UpdateScrollBar();
            }
        }

        private void MainScrollBar_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            // 限定滚动条值为整数
            var scrollBar = (ScrollBar)sender;
            var newValue = Math.Round(e.NewValue, 0);
            if (newValue > scrollBar.Maximum) newValue = scrollBar.Maximum;
            scrollBar.Value = newValue;
            // 设置第一个可视项索引
            _firstIndex = (int)newValue;
            // 更新项视图
            UpdateItemView();
        }

        private void MainGrid_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            MainScrollBar.Value -= e.Delta / 120;
        }

        private void TreeItemView_OnItemExpand()
        {
            UpdateItemList();
            UpdateItemView();
            UpdateScrollBar();
        }

        private void TreeItemView_OnItemFurl()
        {
            UpdateItemList();
            UpdateItemView();
            UpdateScrollBar();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 加载树项
        /// </summary>
        private void LoadTreeItem(TreeItem parent)
        {
            // 遍历子项
            foreach (var subItem in parent.ItemList)
            {
                // 加载子项
                _itemList.Add(subItem);
                // 子项已展开，加载子项的子项
                if (subItem.IsExpanded) LoadTreeItem(subItem);
            }
        }

        /// <summary>
        /// 匹配控件列表
        /// </summary>
        private void MatchControlList()
        {
            // 计算能容纳的控件数量
            int count = (int)(ActualHeight / _controlHeight);
            // 添加一项保证填满空白部分
            count++;
            // 当控件列表数量小于能容纳的数量时，添加控件
            while (_controlList.Count < count)
            {
                TreeItemView control = new TreeItemView();
                _controlList.Add(control);
                ItemBox.Children.Add(control);
                control.OnItemExpand = TreeItemView_OnItemExpand;
                control.OnItemFurl = TreeItemView_OnItemFurl;
            }
        }

        #endregion

        #region 字段

        /// <summary>项列表</summary>
        private readonly List<TreeItem> _itemList = new List<TreeItem>();

        /// <summary>控件列表</summary>
        private readonly List<TreeItemView> _controlList = new List<TreeItemView>();
        /// <summary>控件高度</summary>
        private readonly int _controlHeight = 23;

        /// <summary>第一个可视项索引</summary>
        private int _firstIndex = 0;

        #endregion
    }
}