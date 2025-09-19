using GeekDocument.SubSystem.LayoutEngine;
using GeekDocument.SubSystem.ResourceSystem;
using System.Windows.Controls;
using XLogic.WpfControl;

using Page = GeekDocument.SubSystem.LayoutEngine.Page;

namespace GeekDocument.SubSystem.EditerSystemNew.Control
{
    public partial class LayoutTreePanel : UserControl
    {
        public LayoutTreePanel() => InitializeComponent();

        public event Action<IDocumentElement?> HoverElementChanged;

        public event Action<IDocumentElement?> SelectElementChanged;

        public void Init()
        {
            LayoutTree.Init();
            LayoutTree.HoverItemChanged = (item) =>
            {
                if (item == null) HoverElementChanged?.Invoke(null);
                else HoverElementChanged?.Invoke(item.Content as IDocumentElement);
            };
        }

        public void LoadLayoutTree(Page page)
        {
            // 遍历段落
            foreach (var block in page.BlockList)
            {
                TreeItem blockItem = new TreeItem
                {
                    Content = block.段落,
                    Parent = _root,
                    Icon = ImageResManager.Instance.GetIcon15("Element\\Paragraph.png"),
                    Text = "段落",
                    CanExpand = true,
                };
                _root.ItemList.Add(blockItem);
                LoadSubItem(blockItem, block.段落);
            }

            LayoutTree.TreeRoot = _root;
            LayoutTree.UpdateItemList();
            LayoutTree.UpdateItemView();
        }

        private void LoadSubItem(TreeItem parent, IDocumentElement 元素)
        {
            foreach (var subElement in 元素.GetSubElementList())
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