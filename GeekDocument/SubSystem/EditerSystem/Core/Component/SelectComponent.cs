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

        public CharCursor? Start => _selection.Start;

        public CharCursor? End => _selection.End;

        public int Offset
        {
            get => _offset;
            set
            {
                _offset = value;
                _layer.Margin = new Thickness(0, -_offset + 16, 0, 0);
            }
        }

        public event Action? SelectionChanged = null;

        public event Action? SelectionCanceled = null;

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
            if (_selection.HasSelection) SelectionChanged?.Invoke();
            else SelectionCanceled?.Invoke();
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
            SelectionCanceled?.Invoke();
        }

        /// <summary>
        /// 获取当前块的选择范围
        /// </summary>
        public (int start, int end) GetCurrentBlock_SelectRange()
        {
            if (_selection.HasSelection)
            {
                // 确定前后顺序
                CharCursor first, second;
                if (_selection.Start.CompareTo(_selection.End) <= 0)
                {
                    first = _selection.Start;
                    second = _selection.End;
                }
                else
                {
                    first = _selection.End;
                    second = _selection.Start;
                }

                // 起始索引
                int startIndex = -1;
                int blockIndex = GetComponent<PageComponent>().获取当前块索引();
                if (first.BlockIndex < blockIndex) startIndex = 0;
                else if (first.BlockIndex == blockIndex) startIndex = first.CharIndex;
                // 结束索引
                int endIndex = -1;
                if (second.BlockIndex == blockIndex) endIndex = second.CharIndex;
                else if (second.BlockIndex > blockIndex) endIndex = GetComponent<PageComponent>().获取块(blockIndex).CharIndexMax;

                return (startIndex, endIndex);
            }
            return (-1, -1);
        }

        #endregion

        private readonly Selection _selection = new Selection();
        private SelectionLayer _layer;
        private int _offset = 0;
    }
}