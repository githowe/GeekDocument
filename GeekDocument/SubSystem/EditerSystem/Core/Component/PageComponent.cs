using GeekDocument.SubSystem.EditerSystem.Control.Layer;
using GeekDocument.SubSystem.EditerSystem.Core.Layer;
using GeekDocument.SubSystem.EditerSystem.Define;
using GeekDocument.SubSystem.OptionSystem;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using XLogic.Base.UI;

namespace GeekDocument.SubSystem.EditerSystem.Core.Component
{
    /// <summary>
    /// 页面组件
    /// </summary>
    public class PageComponent : Component<Editer>, IPage
    {
        #region 属性

        /// <summary>页面宽度（包含内边距）</summary>
        public int PageWidth { get; set; } = 0;

        /// <summary>页面高度</summary>
        public int PageHeight => _pageHeight;

        public int Offset
        {
            get => _offset;
            set
            {
                _offset = value;
                _canvas.Margin = new Thickness(0, -_offset + 16, 0, 0);
                _backLayer.Margin = new Thickness(0, -_offset + 16, 0, 0);
            }
        }

        /// <summary>页面内边距</summary>
        public PageThickness Padding { get; set; } = new PageThickness();

        /// <summary>首行缩进</summary>
        public int FirstLineIndent { get; set; } = 32;

        /// <summary>段间距</summary>
        public int ParagraphInterval { get; set; } = 16;

        /// <summary>光标横坐标。此横坐标是为了上下移动光标时保持在一条直线上</summary>
        public int IBeamX { get; set; } = 0;

        #endregion

        #region 生命周期

        protected override void Init()
        {
            // 添加块画布
            _canvas = new Canvas
            {
                Background = new SolidColorBrush(Color.FromRgb(38, 38, 38)),
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 16, 0, 0),
            };
            _host.PageBox.Children.Add(_canvas);
            // 添加背景图层
            _backLayer = new BackLayer
            {
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 16, 0, 0),
            };
            _backLayer.Init();
            _host.PageBox.Children.Add(_backLayer);
        }

        #endregion

        #region IPage 接口

        public void 移动光标(int x, int y, int height)
        {
            GetComponent<IBeamComponent>().MoveIBeam(x, y, height);
        }

        public void 更新光标横坐标() => IBeamX = GetComponent<IBeamComponent>().IBeamX;

        public int 获取光标横坐标() => IBeamX;

        public void 移动光标至前块末尾(BlockLayer block)
        {
            BlockLayer? prevBlock = 获取上一个块(block);
            if (prevBlock == null) throw new Exception("获取上一个块失败");
            prevBlock.MoveIBeamToEnd();
            _currentBlockLayer = prevBlock;
        }

        public void 移动光标至后块开头(BlockLayer block)
        {
            BlockLayer? nextBlock = 获取下一个块(block);
            if (nextBlock == null) throw new Exception("获取下一个块失败");
            nextBlock.MoveIBeamToHead();
            _currentBlockLayer = nextBlock;
        }

        public void 移动光标至前块最后一行(BlockLayer block)
        {
            BlockLayer? prevBlock = 获取上一个块(block);
            if (prevBlock == null) throw new Exception("获取上一个块失败");
            prevBlock.MoveIBeamToLastLine(IBeamX);
            _currentBlockLayer = prevBlock;
        }

        public void 移动光标至后块第一行(BlockLayer block)
        {
            BlockLayer? nextBlock = 获取下一个块(block);
            if (nextBlock == null) throw new Exception("获取下一个块失败");
            nextBlock.MoveIBeamToFirstLine(IBeamX);
            _currentBlockLayer = nextBlock;
        }

        public void 设置当前块(BlockLayer block) => _currentBlockLayer = block;

        public int 获取块索引(BlockLayer block) => _blockLayerList.IndexOf(block);

        public bool 有上一个块(BlockLayer block)
        {
            int index = _blockLayerList.IndexOf(block);
            return index > 0;
        }

        public bool 有下一个块(BlockLayer block)
        {
            int index = _blockLayerList.IndexOf(block);
            return index < _blockLayerList.Count - 1;
        }

        public BlockLayer? 获取上一个块(BlockLayer block)
        {
            int index = _blockLayerList.IndexOf(block);
            if (index > 0) return _blockLayerList[index - 1];
            return null;
        }

        public BlockLayer? 获取下一个块(BlockLayer block)
        {
            int index = _blockLayerList.IndexOf(block);
            if (index < _blockLayerList.Count - 1) return _blockLayerList[index + 1];
            return null;
        }

        public void 插入块(Block block, int index)
        {
            // 生成块图层
            BlockLayer? layer = GenerateBlockLayer(block);
            if (layer == null) throw new Exception("生成块图层失败");
            // 插入至画布、列表
            _canvas.Children.Insert(index, layer);
            _blockLayerList.Insert(index, layer);
            // 更新块图层
            layer.Update();
            // 插入块后须立即更新块坐标，因为移动光标依赖块坐标
            UpdateBlockPoint();
            // 将新块设置为当前块，并移动光标至块开头
            _currentBlockLayer = layer;
            _currentBlockLayer.MoveIBeamToHead();
            更新光标横坐标();
        }

        public void 移除块(BlockLayer block)
        {
            // 移除块控件
            _canvas.Children.Remove(block);
            // 移除块实例
            _blockLayerList.Remove(block);
        }

        #endregion

        #region 公开方法

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
                _canvas.Children.Add(layer);
                _blockLayerList.Add(layer);
            }
            // 绘制块并更新页面高度
            DrawBlock();
            UpdatePageHeight();
            // 更新背景
            UpdateBack();
        }

        /// <summary>
        /// 初始化光标
        /// </summary>
        public void InitIBeam()
        {
            if (_blockLayerList.Count == 0) throw new Exception("页面没有块图层");

            _currentBlockLayer = _blockLayerList[0];
            _currentBlockLayer.MoveIBeamToHead();
            GetComponent<IBeamComponent>().ReqEnable();
        }

        /// <summary>
        /// 处理鼠标按下
        /// </summary>
        public void HandleMouseDown(Point point)
        {
            // 获取命中块并设为当前块
            _currentBlockLayer = GetBlockByPoint(point);
            // 移动光标
            _currentBlockLayer.MoveIBeamToPoint(point);
        }

        /// <summary>
        /// 处理编辑键
        /// </summary>
        public void HandleEditKey(EditKey key)
        {
            if (_currentBlockLayer == null) throw new Exception("无当前块");
            // 交由当前块处理编辑键
            _currentBlockLayer.HandleEditKey(key);
            // 如果是退格键、删除键或回车键，则需要更新块坐标与页面高度
            if (key == EditKey.Backspace || key == EditKey.Delete || key == EditKey.Enter)
            {
                UpdateBlockPoint();
                UpdatePageHeight();
                UpdateBack();
                GetComponent<ScrollBarComponent>().UpdateScrollBar();
            }
        }

        /// <summary>
        /// 处理 Ctrl + 键
        /// </summary>
        public void HandleCtrl_Key(Key key)
        {
            switch (key)
            {
                // 全选
                case Key.A:
                    break;
                // 剪切
                case Key.X:
                    break;
                // 复制
                case Key.C:
                    break;
                // 粘贴
                case Key.V:
                    string text = Clipboard.GetText();
                    if (string.IsNullOrEmpty(text)) return;
                    HandleTextInput(text);
                    break;
                // 撤销
                case Key.Z:
                    break;
                // 重做
                case Key.Y:
                    break;
                // 保存
                case Key.S:
                    break;
                // 回车
                case Key.Enter:
                    break;
            }
        }

        /// <summary>
        /// 处理 Shift + Ctrl + 键
        /// </summary>
        public void HandleShift_Ctrl_Key(Key key)
        {

        }

        /// <summary>
        /// 处理文本输入
        /// </summary>
        public void HandleTextInput(string text)
        {
            // 忽略空字符、退格、回车、Esc
            if (text is "" or "\b" or "\r" or "\u001b") return;
            // 将回车与换行转换为可视文本
            text = text.Replace("\r", "\\r");
            text = text.Replace("\n", "\\n");

            _currentBlockLayer?.InputText(text);
            更新光标横坐标();

            UpdateBlockPoint();
            UpdatePageHeight();
            UpdateBack();
            GetComponent<ScrollBarComponent>().UpdateScrollBar();
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
                    layer = new TextBlockLayer { SourceBlock = block, Block = (BlockText)block };
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
            if (layer != null)
            {
                layer.Page = this;
                layer.BlockWidth = PageWidth - Padding.Horizontal;
                layer.Init();
                block.UpdateViewData(layer.BlockWidth);
            }

            return layer;
        }

        /// <summary>
        /// 绘制块
        /// </summary>
        private void DrawBlock()
        {
            // 块起始坐标
            int x = Padding.Left;
            int y = Padding.Top;
            // 块宽度 = 画布宽度 - 左右内边距
            int blockWidth = (int)(_canvas.ActualWidth - Padding.Left - Padding.Right);
            // 遍历块图层
            foreach (var layer in _blockLayerList)
            {
                // 设置块坐标
                Canvas.SetLeft(layer, x);
                Canvas.SetTop(layer, y);
                // 更新块图层，在此方法中绘制块内容
                layer.Update();
                // 记录块区域
                _blockRectDict[layer] = new Rect(x, y, blockWidth, layer.BlockHeight);
                // 累加纵坐标
                y += layer.BlockHeight + ParagraphInterval;
            }
        }

        /// <summary>
        /// 更新块坐标
        /// </summary>
        private void UpdateBlockPoint()
        {
            // 起始坐标
            int x = Padding.Left;
            int y = Padding.Top;
            // 块宽度 = 画布宽度 - 左右内边距
            int blockWidth = (int)(_canvas.ActualWidth - Padding.Left - Padding.Right);
            // 遍历块图层
            foreach (var layer in _blockLayerList)
            {
                // 设置块坐标
                Canvas.SetLeft(layer, x);
                Canvas.SetTop(layer, y);
                // 记录块区域
                _blockRectDict[layer] = new Rect(x, y, blockWidth, layer.BlockHeight);
                // 累加纵坐标
                y += layer.BlockHeight + ParagraphInterval;
            }
        }

        /// <summary>
        /// 更新页面高度
        /// </summary>
        private void UpdatePageHeight()
        {
            int height = 0;
            // 累加块高度
            foreach (var layer in _blockLayerList)
                height += layer.BlockHeight;
            // 累加块间距
            height += (_blockLayerList.Count - 1) * ParagraphInterval;
            // 累加上下内边距
            height += Padding.Top + Padding.Bottom;
            // 更新画布高度
            _canvas.Height = height;
            _backLayer.PageHeight = height;
            // 累加上下外边距
            height += 32;
            // 更新页面高度
            _pageHeight = height;
        }

        /// <summary>
        /// 更新背景
        /// </summary>
        private void UpdateBack()
        {
            // 大小
            _backLayer.PageWidth = PageWidth;
            _backLayer.PageHeight = _pageHeight - 32;
            // 内边距
            _backLayer.TopMargin = Padding.Top;
            _backLayer.BottomMargin = Padding.Bottom;
            _backLayer.LeftMargin = Padding.Left;
            _backLayer.RightMargin = Padding.Right;
            // 更新
            _backLayer.Update();
        }

        /// <summary>
        /// 根据坐标获取块
        /// </summary>
        private BlockLayer GetBlockByPoint(Point point)
        {
            if (_blockLayerList.Count == 0) throw new Exception("页面没有块");

            // 纵坐标位于第一个块之上，返回第一个块
            if (point.Y < _blockRectDict[_blockLayerList[0]].Top)
                return _blockLayerList[0];
            // 纵坐标位于最后一个块之下，返回最后一个块
            if (point.Y >= _blockRectDict[_blockLayerList[_blockLayerList.Count - 1]].Bottom)
                return _blockLayerList[_blockLayerList.Count - 1];
            // 遍历块区域，找到坐标所处的块
            foreach (var block in _blockLayerList)
            {
                Rect rect = _blockRectDict[block];
                // 纵坐标向上偏移一半的块间距
                rect.Y -= ParagraphInterval / 2;
                // 高度增加一个块间距
                rect.Height += ParagraphInterval;
                if (point.Y >= rect.Top && point.Y < rect.Bottom)
                    return block;
            }

            throw new Exception("未找到对应的块");
        }

        #endregion

        #region 字段

        /// <summary>背景图层</summary>
        private BackLayer _backLayer;
        /// <summary>块画布</summary>
        private Canvas _canvas;

        /// <summary>块图层列表</summary>
        private readonly List<BlockLayer> _blockLayerList = new List<BlockLayer>();
        /// <summary>块区域表</summary>
        private readonly Dictionary<BlockLayer, Rect> _blockRectDict = new Dictionary<BlockLayer, Rect>();

        private BlockLayer? _currentBlockLayer = null;

        /// <summary>页面高度（包含上下外边距）</summary>
        private int _pageHeight = 0;

        private int _offset = 0;

        #endregion
    }
}