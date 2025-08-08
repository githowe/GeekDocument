using System.Windows.Controls;

namespace XLogic.WpfControl
{
    public partial class TabBar : UserControl
    {
        public TabBar() => InitializeComponent();

        public Action<TabItem?>? SelectChanged { get; set; } = null;

        public TabItem AddTabItem(string header)
        {
            TabItem tabItem = new TabItem
            {
                Header = header
            };
            Stack_Item.Children.Add(tabItem);
            _itemList.Add(tabItem);

            tabItem.Click = OnItemClick;
            tabItem.Close = OnItemClose;

            return tabItem;
        }

        /// <summary>
        /// 更新选中项
        /// </summary>
        public void UpdateSelect(int index)
        {
            _selectRecord.UpdateSelect(_itemList[index]);
        }

        public void UpdateSelect(TabItem tabItem)
        {
            _selectRecord.UpdateSelect(tabItem);
        }

        private void OnItemClick(TabItem tabItem)
        {
            UpdateSelect(_itemList.IndexOf(tabItem));
            SelectChanged?.Invoke(tabItem);
        }

        private void OnItemClose(TabItem tabItem)
        {
            // 从选中记录中移除该项
            _selectRecord.RemoveItem(tabItem);
            // 移除控件
            Stack_Item.Children.Remove(tabItem);
            _itemList.Remove(tabItem);
            // 如果选中记录为空，但仍有其他项，则选中第一项
            if (_selectRecord.Empty && _itemList.Count > 0)
                _selectRecord.UpdateSelect(_itemList[0]);
            // 如果移除的是已选中项，则触发选中改变
            if (tabItem.Selected)
                SelectChanged?.Invoke(_selectRecord.CurrentSelected);
        }

        private readonly List<TabItem> _itemList = new List<TabItem>();
        private readonly SelectRecord _selectRecord = new SelectRecord();
    }
}