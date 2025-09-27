using GeekDocument.SubSystem.LayoutEngine;
using GeekDocument.SubSystem.ResourceSystem;
using System.Windows.Controls;
using XLogic.WpfControl;

namespace GeekDocument.SubSystem.EditerSystem3
{
    public partial class LayoutTreePanel : UserControl
    {
        public LayoutTreePanel() => InitializeComponent();

        public event Action<布局元素?> HoverElementChanged;

        public event Action<布局元素?> SelectElementChanged;

        public void Init()
        {
            LayoutTree.Init();
            LayoutTree.HoverItemChanged = (item) =>
            {
                if (item == null) HoverElementChanged?.Invoke(null);
                else HoverElementChanged?.Invoke(item.Content as 布局元素);
            };
        }

        public void LoadLayoutTree(页面 page)
        {
            // 遍历段落
            foreach (var 段落 in page.段落列表)
            {
                TreeItem blockItem = new TreeItem
                {
                    Content = 段落,
                    Parent = _root,
                    Icon = ImageResManager.Instance.GetIcon15("Element\\Paragraph.png"),
                    Text = "段落",
                    CanExpand = true,
                };
                _root.ItemList.Add(blockItem);
                LoadSubItem(blockItem, 段落);
            }

            LayoutTree.TreeRoot = _root;
            LayoutTree.UpdateItemList();
            LayoutTree.UpdateItemView();
        }

        private void LoadSubItem(TreeItem parent, 布局元素 元素)
        {
            foreach (var subElement in 元素.Children)
            {
                TreeItem elementItem = new TreeItem
                {
                    Content = subElement,
                    Parent = parent,
                    Icon = ImageResManager.Instance.GetIcon15($"Element\\{subElement.Icon}.png"),
                    Text = subElement.Name,
                    CanExpand = true,
                };
                parent.ItemList.Add(elementItem);
                LoadSubItem(elementItem, subElement);
            }
        }

        private readonly TreeItem _root = new TreeItem { Text = "页面" };
    }
}