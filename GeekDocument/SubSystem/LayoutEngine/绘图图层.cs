using System.Windows;
using System.Windows.Media;

namespace GeekDocument.SubSystem.LayoutEngine
{
    public class 绘图图层 : FrameworkElement
    {
        protected override int VisualChildrenCount => _绘图对象列表.Count;

        protected override Visual GetVisualChild(int index) => _绘图对象列表[index];

        public void 更新绘图对象列表(List<绘图对象> 新列表)
        {
            foreach (var item in _绘图对象列表)
            {
                RemoveVisualChild(item);
                RemoveLogicalChild(item);
            }
            _绘图对象列表.Clear();
            foreach (var item in 新列表)
            {
                AddVisualChild(item);
                AddLogicalChild(item);
                _绘图对象列表.Add(item);
            }
        }

        private readonly List<绘图对象> _绘图对象列表 = new List<绘图对象>();
    }
}