using GeekDocument.SubSystem.CacheSystem;
using GeekDocument.SubSystem.OptionSystem;
using System.IO;
using System.Windows.Controls;
using XLogic.WpfControl;

namespace GeekDocument.SubSystem.DocLibSystem
{
    public partial class DocLibPanel : UserControl
    {
        public DocLibPanel() => InitializeComponent();

        public Action<string>? DoubleClickItem { get; set; } = null;

        public void Init() => DocTree.Init();

        /// <summary>
        /// 加载文档库
        /// </summary>
        public void LoadDocumentLib()
        {
            // 设置根节点
            DocTree.TreeRoot = DocumentTree.Instance.Root;
            // 恢复树项状态
            foreach (var item in DocTree.TreeRoot.ItemList) RecoverTreeItemState(item);

            // 更新项列表
            DocTree.UpdateItemList();
            // 更新项视图
            DocTree.UpdateItemView();

            // 监听项事件
            foreach (var item in DocTree.TreeRoot.ItemList) ListenTreeItem(item);
            // 监听树事件
            DocTree.DoubleClickItem = Tree_DoubleClickItem;
        }

        /// <summary>
        /// 刷新文档库
        /// </summary>
        public void RefreshDocumentLib()
        {
            // 移除监听
            foreach (var item in DocTree.TreeRoot.ItemList) RemoveListen(item);
            // 刷新文档库数据
            DocumentTree.Instance.Rrfresh();
            // 加载文档库
            LoadDocumentLib();
        }

        /// <summary>
        /// 恢复项状态
        /// </summary>
        private void RecoverTreeItemState(TreeItem item)
        {
            item.IsExpanded = CacheManager.Instance.Cache.DocumentLib.IsExpanded(item.GetFullPath());
            foreach (var subItem in item.ItemList) RecoverTreeItemState(subItem);
        }

        /// <summary>
        /// 监听项事件
        /// </summary>
        private void ListenTreeItem(TreeItem item)
        {
            // 监听项
            item.ItemExpanded = TreeItem_Expanded;
            item.ItemCollapsed = TreeItem_Collapsed;
            // 监听子项
            foreach (var subItem in item.ItemList) ListenTreeItem(subItem);
        }

        private void Tree_DoubleClickItem(TreeItem item)
        {
            string path = GetItemFullPath(item) + ".gdoc";
            DoubleClickItem?.Invoke(path);
        }

        private string GetItemFullPath(TreeItem item)
        {
            if (item.Parent == null) return "";
            if (item.Content is DocumentLib lib) return lib.Path;
            return Path.Combine(GetItemFullPath(item.Parent), item.Text);
        }

        private void RemoveListen(TreeItem item)
        {
            item.ItemExpanded = null;
            item.ItemCollapsed = null;
            foreach (var subItem in item.ItemList) RemoveListen(subItem);
        }

        /// <summary>
        /// 树项展开
        /// </summary>
        private void TreeItem_Expanded(TreeItem treeItem)
        {
            CacheManager.Instance.Cache.DocumentLib.Expand(treeItem.GetFullPath());
        }

        /// <summary>
        /// 树项折叠
        /// </summary>
        private void TreeItem_Collapsed(TreeItem treeItem)
        {
            CacheManager.Instance.Cache.DocumentLib.Fold(treeItem.GetFullPath());
        }
    }
}