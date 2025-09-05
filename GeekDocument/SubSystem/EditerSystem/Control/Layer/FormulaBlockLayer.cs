using GeekDocument.SubSystem.EditerSystem.Control.LayerTool;
using GeekDocument.SubSystem.EditerSystem.Define;
using GeekDocument.SubSystem.EditerSystem.Define.BlockDerive;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GeekDocument.SubSystem.EditerSystem.Control.Layer
{
    public class FormulaBlockLayer : BlockLayer
    {
        #region 属性

        public BlockFormula Block { get; set; }

        public override int BlockHeight => Block.GetViewHeight();

        public override int CharIndex => _charIndex;

        public override int CharIndexMax => 1;

        public override bool IsEmpty => false;

        #endregion

        #region SingleBoard 方法

        public override void Init()
        {
            _stateTree.Init(this);
        }

        protected override void OnUpdate()
        {
            _display = new WriteableBitmap(Block.SourceWidth, Block.SourceHeight, 96, 96, PixelFormats.Bgra32, null);
            _sourceIntRect = new Int32Rect(0, 0, Block.SourceWidth, Block.SourceHeight);
            _display.WritePixels(_sourceIntRect, Block.PixelData, Block.SourceWidth * 4, 0);
            _display.Freeze();
            _dc.DrawImage(_display, new Rect(Block.ImageX, 0, Block.RealRenderWidth, Block.RenderHeight));
        }

        #endregion

        #region BlockLayer 方法

        public override void MoveIBeamToHead()
        {
            _charIndex = 0;
            SyncIBeam();
        }

        public override void MoveIBeamToEnd()
        {
            _charIndex = 1;
            SyncIBeam();
        }

        public override void MoveIBeamToFirstLine(double mouse_x)
        {
            MoveIBeamToImage(mouse_x);
        }

        public override void MoveIBeamToLastLine(double mouse_x)
        {
            MoveIBeamToImage(mouse_x);
        }

        public override void SyncIBeam()
        {
            // 获取块坐标
            int left = (int)Canvas.GetLeft(this);
            int top = (int)Canvas.GetTop(this);

            int x;
            int imagex = left + Block.ImageX;
            if (_charIndex == 0) x = imagex - 1;
            else x = imagex + Block.RealRenderWidth + 1;
            Page.移动光标(x, top, Block.RenderHeight);
        }

        public override void HandleEditKey(EditKey key)
        {
            _stateTree.HandleEditKey(key);
        }

        public override void MoveIBeamToPoint(Point point)
        {
            MoveIBeamToImage(point.X);
            Page.更新光标横坐标();
        }

        #endregion

        #region 状态树接口

        public void 左移光标()
        {
            _charIndex = 0;
            SyncIBeam();
            Page.更新光标横坐标();
        }

        public void 右移光标()
        {
            _charIndex = 1;
            SyncIBeam();
            Page.更新光标横坐标();
        }

        public void 移动光标至行首()
        {
            _charIndex = 0;
            SyncIBeam();
            Page.更新光标横坐标();
        }

        public void 移动光标至行尾()
        {
            _charIndex = 1;
            SyncIBeam();
            Page.更新光标横坐标();
        }

        public void 用退格键删除块()
        {
            // 获取上一个块
            BlockLayer? prevBlock = Page.获取上一个块(this);
            if (prevBlock == null) throw new Exception("获取上一个块失败");
            // 移除当前块
            Page.移除块(this);
            // 将上一个块设为当前块
            Page.设置当前块(prevBlock);
            // 移动光标至上一个块末尾
            prevBlock.MoveIBeamToEnd();
            Page.更新光标横坐标();
        }

        public void 替换为空文本块()
        {
            // 移除当前块
            Page.移除块(this);
            // 插入文本块
            BlockText block = new BlockText { FirstLineIndent = Page.FirstLineIndent };
            Page.插入块(block, 0);
        }

        public void 在块前插入空文本块()
        {
            // 获取自身索引
            int blockIndex = Page.获取块索引(this);
            // 创建文本块
            BlockText blockText = new BlockText { FirstLineIndent = Page.FirstLineIndent };
            Page.插入块(blockText, blockIndex);
        }

        public void 在块后插入空文本块()
        {
            // 获取自身索引
            int blockIndex = Page.获取块索引(this);
            // 创建文本块
            BlockText blockText = new BlockText { FirstLineIndent = Page.FirstLineIndent };
            Page.插入块(blockText, blockIndex + 1);
        }

        #endregion

        #region 私有方法

        private void MoveIBeamToImage(double x)
        {
            int left = (int)Canvas.GetLeft(this);
            int top = (int)Canvas.GetTop(this);
            double imageCenter = left + Block.ImageX + Block.RealRenderWidth / 2.0;
            if (x < imageCenter)
            {
                _charIndex = 0;
                Page.移动光标(left + Block.ImageX - 1, top, Block.RenderHeight);
            }
            else
            {
                _charIndex = 1;
                Page.移动光标(left + Block.ImageX + Block.RealRenderWidth + 1, top, Block.RenderHeight);
            }
        }

        #endregion

        /// <summary>可写位图。当作图片显示器</summary>
        private WriteableBitmap? _display = null;

        private Int32Rect _sourceIntRect = new Int32Rect();

        private int _charIndex = 0;

        private readonly STFormulaBlock _stateTree = new STFormulaBlock();
    }
}