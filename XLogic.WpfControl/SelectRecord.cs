namespace XLogic.WpfControl;

/// <summary>
/// 选择记录
/// </summary>
public class SelectRecord
{
    public List<TabItem> SelectList { get; set; } = new List<TabItem>();

    public TabItem? CurrentSelected
    {
        get
        {
            if (SelectList.Count == 0) return null;
            return SelectList.Last();
        }
    }

    public bool Empty => SelectList.Count == 0;

    /// <summary>
    /// 更新选择
    /// </summary>
    public void UpdateSelect(TabItem tabItem)
    {
        if (CurrentSelected == tabItem) return;

        // 当前选中项不为空，则取消选中状态
        TabItem? current = CurrentSelected;
        if (current != null)
        {
            current.Selected = false;
            current.UpdateItem();
        }
        // 添加选择记录
        SelectList.Add(tabItem);
        // 设置选中状态
        tabItem.Selected = true;
        tabItem.UpdateItem();
    }

    /// <summary>
    /// 移除项，关闭项时调用
    /// </summary>
    public void RemoveItem(TabItem tabItem)
    {
        // 先移除项
        List<TabItem> newList = new List<TabItem>();
        foreach (var item in SelectList)
            if (item != tabItem) newList.Add(item);
        SelectList = newList;
        // 如果项已选中，则切换至最后一个选中记录
        if (tabItem.Selected)
        {
            TabItem? current = CurrentSelected;
            if (current == null) return;
            current.Selected = true;
            current.UpdateItem();
        }
    }
}