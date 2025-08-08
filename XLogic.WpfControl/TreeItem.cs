using System.Windows.Media.Imaging;
using XLogic.Windows.shlwapi;

namespace XLogic.WpfControl
{
    public class TreeItem : IComparable<TreeItem>
    {
        private bool _isExpanded = false;

        public TreeItem? Parent { get; set; } = null;

        public List<TreeItem> ItemList { get; set; } = new List<TreeItem>();

        /// <summary>深度</summary>
        public int Deep => Parent == null ? -1 : Parent.Deep + 1;

        public BitmapImage? Icon { get; set; } = null;

        public string Text { get; set; } = "";

        public bool CanExpand { get; set; } = false;

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded == value) return;
                _isExpanded = value;
                if (_isExpanded) ItemExpanded?.Invoke(this);
                else ItemCollapsed?.Invoke(this);
            }
        }

        public bool IsSelected { get; set; } = false;

        #region 事件

        /// <summary>项展开</summary>
        public Action<TreeItem>? ItemExpanded { get; set; } = null;

        /// <summary>项折叠</summary>
        public Action<TreeItem>? ItemCollapsed { get; set; } = null;

        #endregion

        #region IComparable 接口

        public int CompareTo(TreeItem? other)
        {
            if (other == null) return 1;
            return shlwapiInterop.NaturalCompare(ToString(), other.ToString());
        }

        #endregion

        #region 公开方法

        public string GetFullPath()
        {
            if (Parent == null) return Text;
            return $"{Parent.GetFullPath()}\\{Text}";
        }

        #endregion
    }
}