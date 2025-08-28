using GeekDocument.SubSystem.EditerSystem.Define.BlockDerive;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GeekDocument.SubSystem.EditerSystem.Control.Layer
{
    public class FormulaBlockLayer : BlockLayer
    {
        #region 属性

        public BlockFormula Block { get; set; }

        public override int BlockHeight => Block.GetViewHeight();

        public override int CharIndex => 0;

        public override int CharIndexMax => 1;

        #endregion

        public override void Init()
        {
            
        }

        protected override void OnUpdate()
        {
            _display = new WriteableBitmap(Block.SourceWidth, Block.SourceHeight, 96, 96, PixelFormats.Bgra32, null);
            _sourceIntRect = new Int32Rect(0, 0, Block.SourceWidth, Block.SourceHeight);
            _display.WritePixels(_sourceIntRect, Block.PixelData, Block.SourceWidth * 4, 0);
            _dc.DrawImage(_display, new Rect(Block.ImageX, 0, Block.RealRenderWidth, Block.RenderHeight));
        }

        /// <summary>可写位图。当作图片显示器</summary>
        private WriteableBitmap? _display = null;

        private Int32Rect _sourceIntRect = new Int32Rect();
    }
}