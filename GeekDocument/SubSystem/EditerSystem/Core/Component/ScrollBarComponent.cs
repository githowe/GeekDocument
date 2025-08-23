using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using XLogic.Base.UI;

namespace GeekDocument.SubSystem.EditerSystem.Core.Component
{
    /// <summary>
    /// 滚动条组件
    /// </summary>
    public class ScrollBarComponent : Component<Editer>
    {
        #region 生命周期

        protected override void Init()
        {
            _scrollBar = _host.PageScrollBar;
        }

        protected override void Enable()
        {
            // 更新滚动条
            UpdateScrollBar();
            // 监听滚动条、文档区域高度
            _scrollBar.ValueChanged += ScrollBar_ValueChanged;
            _host.DocArea.SizeChanged += DocArea_SizeChanged;
        }

        protected override void Disable()
        {
            _scrollBar.ValueChanged -= ScrollBar_ValueChanged;
        }

        private void DocArea_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.HeightChanged) UpdateScrollBar();
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 更新滚动条
        /// </summary>
        public void UpdateScrollBar()
        {
            _scrollBar.ViewportSize = _host.DocArea.ActualHeight;
            _scrollBar.Maximum = GetComponent<PageComponent>().PageHeight - _host.DocArea.ActualHeight;
        }

        public void HandleMouseWheel(MouseWheelEventArgs e) => _scrollBar.Value -= e.Delta / 120 * 64;

        public void Scroll(int delta) => _scrollBar.Value -= delta * 64;

        #endregion

        #region 控件事件

        private void ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GetComponent<IBeamComponent>().Offset = (int)_scrollBar.Value;
            GetComponent<PageComponent>().Offset = (int)_scrollBar.Value;
            GetComponent<SelectComponent>().Offset = (int)_scrollBar.Value;
        }

        #endregion

        #region 字段

        private ScrollBar _scrollBar;

        #endregion
    }
}