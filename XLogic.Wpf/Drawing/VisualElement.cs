using System.Windows.Media;

namespace XLogic.Wpf.Drawing
{
    /// <summary>
    /// 可视元素
    /// </summary>
    public abstract class VisualElement : DrawingVisual
    {
        /// <summary>
        /// 更新可视元素
        /// </summary>
        public void Update()
        {
            using DrawingContext context = RenderOpen();
            OnUpdate(context);
        }

        protected abstract void OnUpdate(DrawingContext context);
    }
}