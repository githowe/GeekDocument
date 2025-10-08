using GeekDocument.SubSystem.LayoutEngine;
using GeekDocument.SubSystem.ResourceSystem;
using System.Windows.Controls;
using XLogic.WpfControl;

namespace GeekDocument.SubSystem.EditerSystem3
{
    public partial class LayoutTreePanel : UserControl
    {
        public LayoutTreePanel() => InitializeComponent();

        public event Action<IDocElement?> HoverElementChanged;

        public event Action<IDocElement?> SelectElementChanged;

        public void Init()
        {
            LayoutTree.Init();
            LayoutTree.HoverItemChanged = (item) =>
            {
                if (item == null) HoverElementChanged?.Invoke(null);
                else HoverElementChanged?.Invoke(item.Content as IDocElement);
            };
        }

        public void LoadLayoutTree(页面 page)
        {
            _elementItemDict.Add(page, _root);
            page.ChildrenChanged = Children_Changed;

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
                _elementItemDict.Add(段落, blockItem);
                段落.ChildrenChanged = Children_Changed;
                段落.Removed = Element_Removed;
                LoadSubItem(blockItem, 段落);
            }

            LayoutTree.TreeRoot = _root;
            LayoutTree.UpdateItemList();
            LayoutTree.UpdateItemView();
        }

        private void LoadSubItem(TreeItem parent, IDocElement 元素)
        {
            foreach (var subElement in 元素.ChildrenElement)
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
                _elementItemDict.Add(subElement, elementItem);
                subElement.ChildrenChanged = Children_Changed;
                subElement.Removed = Element_Removed;
                LoadSubItem(elementItem, subElement);
            }
        }

        private void Children_Changed(IDocElement sender)
        {
            // 解除所有子元素引用
            foreach (var item in GetAllSubElement(sender))
            {
                if (_elementItemDict.ContainsKey(item))
                {
                    item.ChildrenChanged = null;
                    item.Removed = null;
                    _elementItemDict.Remove(item);
                }
            }
            // 获取元素对应的树项
            TreeItem treeItem = _elementItemDict[sender];
            // 清空子项
            treeItem.ItemList.Clear();
            // 重新加载全部子元素
            LoadSubItem(treeItem, sender);
            // 刷新树视图
            LayoutTree.UpdateItemList();
            LayoutTree.UpdateItemView();
        }

        private void Element_Removed(IDocElement sender)
        {
            // 解除自身引用
            if (_elementItemDict.ContainsKey(sender))
            {
                sender.ChildrenChanged = null;
                sender.Removed = null;
                _elementItemDict.Remove(sender);
            }
            // 解除所有子元素引用
            List<IDocElement> allSub = GetAllSubElement(sender);
            foreach (var item in allSub)
            {
                if (_elementItemDict.ContainsKey(item))
                {
                    item.ChildrenChanged = null;
                    item.Removed = null;
                    _elementItemDict.Remove(item);
                }
            }
        }

        private List<IDocElement> GetAllSubElement(IDocElement sender)
        {
            List<IDocElement> result = new List<IDocElement>();
            foreach (var item in sender.ChildrenElement)
            {
                result.Add(item);
                result.AddRange(GetAllSubElement(item));
            }
            return result;
        }

        private readonly TreeItem _root = new TreeItem { Text = "页面" };
        private readonly Dictionary<IDocElement, TreeItem> _elementItemDict = new Dictionary<IDocElement, TreeItem>();
    }
}