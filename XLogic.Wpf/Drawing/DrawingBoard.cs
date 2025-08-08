using System.Windows;
using System.Windows.Media;

namespace XLogic.Wpf.Drawing
{
    /// <summary>
    /// 绘图板
    /// </summary>
    public class DrawingBoard : FrameworkElement
    {
        protected override int VisualChildrenCount => _visualList.Count;

        protected override Visual GetVisualChild(int index) => _visualList[index];

        /// <summary>
        /// 添加可视对象
        /// </summary>
        public void AddVisual(VisualElement visual)
        {
            _visualList = new List<VisualElement>(_visualList) { visual };

            AddVisualChild(visual);
            AddLogicalChild(visual);
        }

        /// <summary>
        /// 移除可视对象
        /// </summary>
        public void RemoveVisual(VisualElement visual)
        {
            var newList = new List<VisualElement>(_visualList);
            newList.Remove(visual);
            _visualList = newList;

            RemoveVisualChild(visual);
            RemoveLogicalChild(visual);
        }

        /// <summary>
        /// 清空可视对象
        /// </summary>
        public void ClearVisual()
        {
            List<VisualElement> oldList = new List<VisualElement>(_visualList);
            _visualList = new List<VisualElement>();
            foreach (var visual in oldList)
            {
                RemoveVisualChild(visual);
                RemoveLogicalChild(visual);
            }
        }

        /// <summary>
        /// 更新画板
        /// </summary>
        public virtual void Update()
        {
            foreach (var item in _visualList) item.Update();
        }

        /// <summary>可视元素列表</summary>
        private List<VisualElement> _visualList = new List<VisualElement>();
    }
}