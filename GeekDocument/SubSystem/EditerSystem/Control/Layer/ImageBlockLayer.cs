using GeekDocument.SubSystem.EditerSystem.Define.BlockDerive;
using GeekDocument.SubSystem.ImageSystem;
using GeekDocument.SubSystem.LayoutSystem;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using XLogic.Base.Ex;

namespace GeekDocument.SubSystem.EditerSystem.Control.Layer
{
    public class ImageBlockLayer : BlockLayer
    {
        #region 属性

        /// <summary>图片块实例</summary>
        public BlockImage Block { get; set; }

        public override int BlockHeight => Block.GetViewHeight();

        public override int CharIndex => 0;

        public override int CharIndexMax => 0;

        #endregion

        #region object 方法

        public override string ToString() => Block.Caption ?? $"Image_{Block.SourceWidth}x{Block.SourceHeight}";

        #endregion

        #region 生命周期

        public override void Init()
        {
            // 创建显示器
            _display = new WriteableBitmap(Block.SourceWidth, Block.SourceHeight, 96, 96, PixelFormats.Bgra32, null);
            // 将第一帧写入显示器
            ImageFrame frameData = Block.FrameList[0];
            _sourceIntRect = new Int32Rect(0, 0, Block.SourceWidth, Block.SourceHeight);
            _display.WritePixels(_sourceIntRect, frameData.PixelData, Block.SourceWidth * 4, 0);
            // 动态图片，则启动定时器
            if (Block.FrameList.Count > 1)
            {
                _timer.Interval = TimeSpan.FromMilliseconds(1000 / 50.0);
                _timer.Tick += Timer_Tick;
                _timer.Start();
                _watch.Start();
            }
        }

        #endregion

        #region 公开方法

        #endregion

        #region BlockLayer 方法



        #endregion

        #region 内部方法

        protected override void OnUpdate()
        {
            // 绘制图片
            _dc.DrawImage(_display, new Rect(Block.ImageX, 0, Block.RenderWidth, Block.RenderHeight));
            // 绘制图注
            if (Block.Caption == null) return;
            int y = Block.TextY;
            int index = 0;
            foreach (var word in Block.TextLine.WordList)
            {
                // 不绘制空格
                if (word.WordType == WordType.Space)
                {
                    index++;
                    continue;
                }
                // 字横坐标
                double word_x = Block.TextLine.XList[index];
                // 绘制字的字形
                foreach (var image in word.GlyphImageList)
                {
                    Point leftTop = new Point((word_x + image.Origin.X).Round(), y + image.Origin.Y);
                    _dc.DrawImage(image.GetBitmap(255, 255, 255), new Rect(leftTop, new Size(image.RenderWidth, image.RenderHeight)));
                    word_x += image.GlyphWidth;
                }
                // 移动至下一个字
                index++;
            }
        }

        #endregion

        #region 私有方法

        private void Timer_Tick(object? sender, EventArgs e)
        {
            int milliseconds = (int)_watch.ElapsedMilliseconds % Block.Duration;
            ImageFrame? render = null;
            foreach (var frame in Block.FrameList)
            {
                if (milliseconds >= frame.Timestamp) render = frame;
                else break;
            }
            if (render == null) return;
            _display?.WritePixels(_sourceIntRect, render.PixelData, Block.SourceWidth * 4, 0);
        }

        #endregion

        #region 字段

        /// <summary>可写位图。当作图片显示器</summary>
        private WriteableBitmap? _display = null;

        private Int32Rect _sourceIntRect = new Int32Rect();

        private readonly DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Normal);
        private readonly Stopwatch _watch = new Stopwatch();

        #endregion
    }
}