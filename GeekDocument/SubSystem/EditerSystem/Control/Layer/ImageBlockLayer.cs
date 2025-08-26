using GeekDocument.SubSystem.EditerSystem.Control.LayerTool;
using GeekDocument.SubSystem.EditerSystem.Define;
using GeekDocument.SubSystem.EditerSystem.Define.BlockDerive;
using GeekDocument.SubSystem.ImageSystem;
using GeekDocument.SubSystem.LayoutSystem;
using GeekDocument.SubSystem.TimeSystem;
using System.Windows;
using System.Windows.Controls;
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

        public override int CharIndex => _charIndex;

        public override int CharIndexMax
        {
            get
            {
                if (Block.Caption == null) return 1;
                else return 1 + Block.Caption.Length + 1;
            }
        }

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
                _timer.Interval = TimeSpan.FromMilliseconds(1000 / 40.0);
                _timer.Tick += Timer_Tick;
                _timer.Start();
                _startMs = AppWatch.Instance.Milliseconds;
            }
            _stateTree.Init(this);
        }

        #endregion

        #region 公开方法

        #endregion

        #region BlockLayer 方法

        public override void MoveIBeamToHead()
        {
            _charIndex = 0;
            SyncIBeam();
        }

        public override void MoveIBeamToEnd()
        {
            if (Block.Caption == null) _charIndex = 1;
            else _charIndex = 1 + Block.Caption.Length + 1;
            SyncIBeam();
        }

        public override void MoveIBeamToFirstLine(double mouse_x)
        {
            _subArea = SubArea.Image;
            MoveIBeamToImage(mouse_x);
        }

        public override void MoveIBeamToLastLine(double mouse_x)
        {
            if (Block.Caption != null)
            {
                _subArea = SubArea.Caption;
                MoveIBeamToCaption(mouse_x);
            }
            else
            {
                _subArea = SubArea.Image;
                MoveIBeamToImage(mouse_x);
            }
        }

        public override void SyncIBeam()
        {
            // 获取块坐标
            int left = (int)Canvas.GetLeft(this);
            int top = (int)Canvas.GetTop(this);
            // 确定当前所在子区域
            if (_charIndex <= 1) _subArea = SubArea.Image;
            else
            {
                if (Block.Caption != null) _subArea = SubArea.Caption;
                else throw new Exception("字符索引异常：没有图注时，字符索引只能为零或一");
            }
            // 图片区
            if (_subArea == SubArea.Image)
            {
                int x;
                int imagex = left + Block.ImageX;
                // 图片前
                if (_charIndex == 0) x = imagex;
                // 图片后
                else x = imagex + Block.RenderWidth;
                // 移动光标
                Page.移动光标(x, top, Block.RenderHeight);
            }
            // 图注区
            else
            {
                int y = top + Block.TextY;
                // 图注为空字符串
                if (Block.Caption == "")
                {
                    // 将光标移动至图片中间
                    double imageCenter = left + Block.ImageX + Block.RenderWidth / 2.0;
                    Page.移动光标(imageCenter.RoundInt(), y, Block.FontSize);
                }
                else
                {
                    // 获取图注的字符横坐标列表
                    List<double> xList = Block.TextLine.GetXList(left);
                    // 图注内索引 = 总字符索引 - 图片索引数
                    int x = xList[_charIndex - 2].RoundInt();
                    // 移动光标
                    Page.移动光标(x, y, Block.FontSize);
                }
            }
        }

        public override void HandleEditKey(EditKey key)
        {
            _stateTree.HandleEditKey(key);
        }

        public override void MoveIBeamToPoint(Point point)
        {
            // 确定子区域
            UpdateSubAreaByPoint(point);
            // 图片区
            if (_subArea == SubArea.Image) MoveIBeamToImage(point.X);
            // 图注区
            else MoveIBeamToCaption(point.X);
            Page.更新光标横坐标();
        }

        #endregion

        #region 状态树接口

        public bool HasPrevLine => _subArea == SubArea.Caption;

        public bool HasNextLine => _subArea == SubArea.Image && Block.Caption != null;

        public void 上移光标()
        {
            _subArea = SubArea.Image;
            MoveIBeamToImage(Page.获取光标横坐标());
        }

        public void 下移光标()
        {
            _subArea = SubArea.Caption;
            MoveIBeamToCaption(Page.获取光标横坐标());
        }

        public void 左移光标()
        {
            _charIndex--;
            SyncIBeam();
            Page.更新光标横坐标();
        }

        public void 右移光标()
        {
            _charIndex++;
            SyncIBeam();
            Page.更新光标横坐标();
        }

        public void 移动光标至行首()
        {
            if (_subArea == SubArea.Image) _charIndex = 0;
            else _charIndex = 2;
            SyncIBeam();
            Page.更新光标横坐标();
        }

        public void 移动光标至行尾()
        {
            if (_subArea == SubArea.Image) _charIndex = 1;
            else _charIndex = CharIndexMax;
            SyncIBeam();
            Page.更新光标横坐标();
        }

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
            int milliseconds = (int)((AppWatch.Instance.Milliseconds - _startMs) % Block.Duration);
            ImageFrame? render = null;
            foreach (var frame in Block.FrameList)
            {
                if (milliseconds >= frame.Timestamp) render = frame;
                else break;
            }
            if (render == null) return;
            _display?.WritePixels(_sourceIntRect, render.PixelData, Block.SourceWidth * 4, 0);
        }

        /// <summary>
        /// 根据坐标更新子区域
        /// </summary>
        private void UpdateSubAreaByPoint(Point point)
        {
            // 没有图注，则只能在图片区
            if (Block.Caption == null)
            {
                _subArea = SubArea.Image;
                return;
            }
            int top = (int)Canvas.GetTop(this);
            // 图片区范围
            double imageMaxY = top + Block.RenderHeight + Block.CaptionTop / 2.0;
            // 根据坐标确定子区域
            if (point.Y < imageMaxY) _subArea = SubArea.Image;
            else _subArea = SubArea.Caption;
        }

        private void MoveIBeamToImage(double x)
        {
            int left = (int)Canvas.GetLeft(this);
            int top = (int)Canvas.GetTop(this);
            double imageCenter = left + Block.ImageX + Block.RenderWidth / 2.0;
            if (x < imageCenter)
            {
                _charIndex = 0;
                Page.移动光标(left + Block.ImageX, top, Block.RenderHeight);
            }
            else
            {
                _charIndex = 1;
                Page.移动光标(left + Block.ImageX + Block.RenderWidth, top, Block.RenderHeight);
            }
        }

        /// <summary>
        /// 移动光标至图注
        /// </summary>
        private void MoveIBeamToCaption(double x)
        {
            int left = (int)Canvas.GetLeft(this);
            int top = (int)Canvas.GetTop(this);
            if (Block.Caption == "")
            {
                _charIndex = 2;
                double imageCenter = left + Block.ImageX + Block.RenderWidth / 2.0;
                Page.移动光标(imageCenter.RoundInt(), top + Block.TextY, Block.FontSize);
            }
            else
            {
                // 获取图注的字符横坐标列表
                List<double> xList = Block.TextLine.GetXList(left);
                // 计算命中区间索引
                int hitedIndex = xList.GetHitedRange(x);
                // 计算命中横坐标
                double xleft = xList[hitedIndex];
                double xright = xList[hitedIndex + 1];
                double center = xleft + (xright - xleft) / 2;
                double hitedx = x < center ? xleft : xright;
                // 过半时定位至右索引
                if (x >= center) hitedIndex++;
                // 字符索引 = 命中索引 + 图片索引数
                _charIndex = hitedIndex + 2;
                Page.移动光标(hitedx.RoundInt(), top + Block.TextY, Block.FontSize);
            }
        }

        #endregion

        #region 内部类

        /// <summary>
        /// 子区域
        /// </summary>
        private enum SubArea
        {
            /// <summary>图片区</summary>
            Image,
            /// <summary>图注区</summary>
            Caption,
        }

        #endregion

        #region 字段

        /// <summary>可写位图。当作图片显示器</summary>
        private WriteableBitmap? _display = null;

        private Int32Rect _sourceIntRect = new Int32Rect();

        private readonly DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Normal);
        private long _startMs = 0;

        private int _charIndex = 0;
        private SubArea _subArea = SubArea.Image;

        private readonly STImageBlock _stateTree = new STImageBlock();

        #endregion
    }
}