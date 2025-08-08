using GeekDocument.SubSystem.EditerSystem.Control.Layer;
using GeekDocument.SubSystem.EditerSystem.Define;
using GeekDocument.SubSystem.OptionSystem;
using System.Windows.Controls;

namespace GeekDocument.SubSystem.EditerSystem.Control
{
    public partial class Page : UserControl
    {
        public Page() => InitializeComponent();

        #region 公开方法

        public void Init()
        {

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
            // 绘制块
            DrawBlock();
        }

        #endregion

        #region 私有方法

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

        /// <summary>块图层列表</summary>
        private readonly List<BlockLayer> _blockLayerList = new List<BlockLayer>();

        #endregion
    }
}