using System.Windows.Controls;
using System.Windows.Media;

namespace SmartLib.WPFStyle
{
    /// <summary>
    /// 控件扩展方法
    /// </summary>
    public static class ControlEx
    {
        /// <summary>
        /// 获取树视图项的深度
        /// </summary>
        public static int GetDepth(this TreeViewItem item)
        {
            TreeViewItem parent;
            while ((parent = GetParent(item)) != null)
                return GetDepth(parent) + 1;
            return 0;
        }

        public static T? GetParentControl<T>(this Control control) where T : Control
        {
            if (control.Parent == null) return null;

            Control? parent = control.Parent as Control;
            while (parent != null)
            {
                if (parent is T t) return t;
                parent = parent.Parent as Control;
            }

            return null;
        }

        /// <summary>
        /// 获取树视图项的父项
        /// </summary>
        private static TreeViewItem? GetParent(TreeViewItem item)
        {
            var parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem || parent is TreeView))
                parent = VisualTreeHelper.GetParent(parent);
            return parent as TreeViewItem;
        }
    }
}