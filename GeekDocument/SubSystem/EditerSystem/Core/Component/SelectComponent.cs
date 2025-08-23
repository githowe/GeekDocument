using GeekDocument.SubSystem.EditerSystem.Core.Layer;
using GeekDocument.SubSystem.EditerSystem.Define;
using System.Windows;
using XLogic.Base.UI;

namespace GeekDocument.SubSystem.EditerSystem.Core.Component
{
    /// <summary>
    /// 选择组件
    /// </summary>
    public class SelectComponent : Component<Editer>
    {
        #region 属性

        public bool HasSelection => _selection.HasSelection;

        public int Offset
        {
            get => _offset;
            set
            {
                _offset = value;
                _layer.Margin = new Thickness(0, -_offset + 16, 0, 0);
            }
        }

        #endregion

        #region 生命周期

        protected override void Init()
        {
            _layer = new SelectionLayer { Margin = new Thickness(0, 16, 0, 0) };
            _layer.Init();
            _host.LayerBox.Children.Add(_layer);
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 处理鼠标按下
        /// </summary>
        public void HandleMouseDown()
        {
            // 设置选区起点
            _selection.Start = GetComponent<PageComponent>().GetCharCursor();
        }

        /// <summary>
        /// 更新选区
        /// </summary>
        public void UpdateSelection()
        {
            // 设置选区终点
            _selection.End = GetComponent<PageComponent>().GetCharCursor();
            // 更新选区包含的区域列表
            _layer.RectList = GetComponent<PageComponent>().GetSelectionRectList(_selection.Start, _selection.End);
            // 更新图层
            _layer.Update();
        }

        /// <summary>
        /// 取消选区
        /// </summary>
        public void CancelSelection()
        {
            // 清空选区图层
            _layer.Clear();
            _layer.RectList.Clear();
            // 置空选区
            _selection.Start = null;
            _selection.End = null;
        }

        #endregion

        private readonly Selection _selection = new Selection();
        private SelectionLayer _layer;
        private int _offset = 0;
    }
}