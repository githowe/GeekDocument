using GeekDocument.SubSystem.EditerSystem.Control.Layer;
using GeekDocument.SubSystem.EditerSystem.Define;
using GeekDocument.SubSystem.OptionSystem;
using System.Windows;
using System.Windows.Controls;

namespace GeekDocument.SubSystem.EditerSystem.Control
{
    public partial class Page : UserControl
    {
        public Page() => InitializeComponent();

        /// <summary>块列表</summary>
        public List<BlockLayer> BlockList => _blockLayerList;

        /// <summary>页面高度</summary>
        public int PageHeight { get; private set; } = 0;

        /// <summary>页面垂直偏移</summary>
        public int PageOffset
        {
            get => _pageOffset;
            set
            {
                _pageOffset = value;
                BlockCanvas.Margin = new Thickness(0, -_pageOffset + 16, 0, 0);
            }
        }

        public Action? PageHeightChanged { get; set; } = null;

        #region 公开方法

        public void Init()
        {
            InitBack();
        }

        /// <summary>
        /// 加载块
        /// </summary>
        public void LoadBlock(List<Block> blockList)
        {
            // 将块加载至画布
            foreach (var block in blockList)
            {
                BlockLayer? layer = GenerateBlockLayer(block);
                if (layer == null) continue;
                BlockCanvas.Children.Add(layer);
                _blockLayerList.Add(layer);
            }
            // 绘制块并更新页面高度
            DrawBlock();
            UpdatePageHeight();
            PageOffset = 0;
            // 更新背景
            UpdateBack();
        }

        /// <summary>
        /// 更新页面高度
        /// </summary>
        public void UpdatePageHeight()
        {
            int height = 0;
            // 累加块高度
            foreach (var blockLayer in _blockLayerList)
                height += blockLayer.BlockHeight;
            // 累加块间距
            height += (_blockLayerList.Count - 1) * Options.Instance.Page.BlockInterval;
            // 累加页面上下内边距
            height += Options.Instance.Page.PageMargin.Top + Options.Instance.Page.PageMargin.Bottom;
            // 累加页面上下外边距
            height += 32;

            BlockCanvas.Height = height - 32;

            PageHeight = height;
            PageHeightChanged?.Invoke();
        }

        /// <summary>
        /// 获取第一个块图层
        /// </summary>
        public BlockLayer? GetFirstBlock()
        {
            if (_blockLayerList.Count == 0) return null;
            return _blockLayerList[0];
        }

        /// <summary>
        /// 设置当前块图层
        /// </summary>
        public void SetCurrentBlock(BlockLayer layer)
        {
            _currentBlockLayer = layer;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 初始化背景
        /// </summary>
        private void InitBack()
        {
            _backLayer = new BackLayer();
            _backLayer.Init();
            BlockCanvas.Children.Add(_backLayer);
        }

        /// <summary>
        /// 更新背景
        /// </summary>
        private void UpdateBack()
        {
            PageOption pageOption = Options.Instance.Page;
            _backLayer.PageWidth = pageOption.PageWidth + pageOption.PageMargin.Left + pageOption.PageMargin.Right;
            _backLayer.PageHeight = PageHeight - 32;
            _backLayer.TopMargin = pageOption.PageMargin.Top;
            _backLayer.BottomMargin = pageOption.PageMargin.Bottom;
            _backLayer.LeftMargin = pageOption.PageMargin.Left;
            _backLayer.RightMargin = pageOption.PageMargin.Right;
            _backLayer.Update();
        }

        /// <summary>
        /// 生成块图层
        /// </summary>
        private BlockLayer? GenerateBlockLayer(Block block)
        {
            BlockLayer? layer = null;

            switch (block.Type)
            {
                case BlockType.Text:
                    if (block is BlockText blockText)
                        layer = new TextBlockLayer { Block = blockText };
                    break;
                case BlockType.SplitLine:
                    break;
                case BlockType.Code:
                    break;
                case BlockType.List:
                    break;
                case BlockType.Image:
                    break;
                case BlockType.Table:
                    break;
                case BlockType.Formula:
                    break;
                case BlockType.Chart:
                    break;
                case BlockType.Model:
                    break;
                case BlockType.Audio:
                    break;
            }
            layer?.Init();

            return layer;
        }

        /// <summary>
        /// 绘制块
        /// </summary>
        private void DrawBlock()
        {
            // 起始坐标
            int x = Options.Instance.Page.PageMargin.Left;
            int y = Options.Instance.Page.PageMargin.Top;
            // 遍历块图层
            foreach (var layer in _blockLayerList)
            {
                // 设置块坐标
                Canvas.SetLeft(layer, x);
                Canvas.SetTop(layer, y);
                // 更新块图层，在此方法中绘制块内容
                layer.Update();
                // 累加纵坐标
                y += layer.BlockHeight + Options.Instance.Page.BlockInterval;
            }
        }

        #endregion

        #region 字段

        /// <summary>背景图层</summary>
        private BackLayer _backLayer;
        /// <summary>块图层列表</summary>
        private readonly List<BlockLayer> _blockLayerList = new List<BlockLayer>();

        /// <summary>当前块图层</summary>
        private BlockLayer? _currentBlockLayer = null;

        private int _pageOffset = 0;

        #endregion
    }
}