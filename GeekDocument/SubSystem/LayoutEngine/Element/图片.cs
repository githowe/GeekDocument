using GeekDocument.SubSystem.ImageSystem;
using GeekDocument.SubSystem.TimeSystem;
using GeekDocument.SubSystem.WindowSystem;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XLogic.Wpf;

public enum 图注最大宽度
{
    跟随图片,
    指定宽度
}

namespace GeekDocument.SubSystem.LayoutEngine.Element
{
    public class 图片 : 布局元素, ITimerHandler
    {
        #region 构造方法

        public 图片() => 类型 = 元素类型.图片;

        #endregion

        #region IDocumentElement 成员

        public override string Icon { get; set; } = "Image";

        public override string Name
        {
            get
            {
                return $"图片_{ActualWidth}x{ActualHeight}";
            }
            set { }
        }

        #endregion

        #region 属性

        /// <summary>源哈希值</summary>
        public string SourceHash { get; set; } = "";

        public double ImageWidth { get; set; } = 0;

        /// <summary>是否为像素画</summary>
        public bool PixelArt { get; set; } = false;

        /// <summary>图注</summary>
        public string? Caption { get; set; } = null;

        public 图注最大宽度 CaptionMaxWidthType { get; set; } = 图注最大宽度.跟随图片;

        /// <summary>图注最大宽度</summary>
        public double CaptionMaxWidth { get; set; } = double.NaN;

        public string Font { get; set; } = "霞鹜文楷";

        public int FontSize { get; set; } = 16;

        #endregion

        #region 运行时属性

        public int SourceWidth { get; set; } = 480;

        public int SourceHeight { get; set; } = 270;

        /// <summary>帧列表</summary>
        public List<ImageFrame> FrameList { get; set; } = new List<ImageFrame>();

        /// <summary>总持续时长。单位：毫秒</summary>
        public int Duration { get; set; } = 0;

        /// <summary>帧率</summary>
        public double Fps
        {
            get
            {
                if (FrameList.Count < 2) return 0;
                // 帧率 = 帧数 / 秒
                return FrameList.Count / (Duration / 1000.0);
            }
        }

        #endregion

        #region 布局元素方法

        public override void Init()
        {
            if (SourceHash != "") LoadImage();
        }

        public override List<ElementLayer> GetLayerList() => new List<ElementLayer> { _layer };

        public override void Measure()
        {
            适配图片大小();
            _imageActualWidth = ActualWidth;
            _imageActualHeight = ActualHeight;
            if (Caption != null)
            {
                图注 = new 段落
                {
                    Text = Caption,
                    首行缩进 = 0,
                    Font = Font,
                    FontSize = FontSize,
                    PlainText = true,
                };
                图注.Init();

                // 跟随图片时，设置图注最大宽度为图片宽度
                if (CaptionMaxWidthType == 图注最大宽度.跟随图片)
                    图注.MaxWidth = _imageActualWidth;
                else
                {
                    if (double.IsNaN(CaptionMaxWidth)) throw new Exception("图注未设置最大宽度");
                    if (CaptionMaxWidth <= 0) 图注.MaxWidth = double.PositiveInfinity;
                    else 图注.MaxWidth = CaptionMaxWidth;
                }
                // 计算图注大小并匹配内容宽度
                图注.Measure();
                图注.FitContentWidth();
                // 更新实际大小
                if (图注.ActualWidth > ActualWidth) ActualWidth = 图注.ActualWidth;
                ActualHeight += 图注.ActualHeight + 4;
            }
        }

        public override void Arrange()
        {
            if (图注 != null)
            {
                // 图注居中至图片
                if (图注.ActualWidth < _imageActualWidth)
                {
                    double offset = (_imageActualWidth - 图注.ActualWidth) / 2;
                    图注.Left = Left + offset;
                }
                // 图片居中至图注
                else if (图注.ActualWidth > _imageActualWidth)
                {
                    _imageOffset = (图注.ActualWidth - _imageActualWidth) / 2;
                    图注.Left = Left;
                }
                else 图注.Left = Left;
                图注.Top = Top + _imageActualHeight + 4;
                图注.Arrange();
            }
        }

        public override void 绘图(DrawingContext dc)
        {
            double x = Math.Round(Left + _imageOffset);
            double y = Math.Round(Top);

            // 设置图片缩放模式
            if (PixelArt) RenderOptions.SetBitmapScalingMode(_layer, BitmapScalingMode.NearestNeighbor);
            else RenderOptions.SetBitmapScalingMode(_layer, BitmapScalingMode.HighQuality);
            // 绘制图片
            DrawingContext self_dc = _layer.Open();
            self_dc.DrawImage(_display, new Rect(x, y, _imageActualWidth, _imageActualHeight));
            self_dc.Close();
            // 绘制图注
            图注?.绘图(dc);
        }

        #endregion

        #region ITimerHandler 方法

        public void Tick()
        {
            int milliseconds = (int)((AppWatch.Instance.Milliseconds - _startMs) % Duration);
            ImageFrame? render = null;
            foreach (var frame in FrameList)
            {
                if (milliseconds >= frame.Timestamp) render = frame;
                else break;
            }
            if (render == null) return;
            _display?.WritePixels(_sourceIntRect, render.PixelData, SourceWidth * 4, 0);
        }

        #endregion

        #region 私有方法

        private void LoadImage()
        {
            // 获取图片信息
            ImageInfo? imageInfo = ImageManager.Instance.FindImageInfo(SourceHash);
            if (imageInfo == null)
            {
                WM.ShowErrorTip($"加载图片块“{Caption}”失败：找不到图片信息");
                return;
            }
            SourceWidth = imageInfo.Width;
            SourceHeight = imageInfo.Height;
            FrameList = imageInfo.FrameList;
            Duration = imageInfo.Duration;
            // 初始化显示器
            InitDisplay();
        }

        private void InitDisplay()
        {
            // 创建显示器
            _display = new WriteableBitmap(SourceWidth, SourceHeight, 96, 96, PixelFormats.Bgra32, null);
            // 将第一帧写入显示器
            ImageFrame frameData = FrameList[0];
            _sourceIntRect = new Int32Rect(0, 0, SourceWidth, SourceHeight);
            _display.WritePixels(_sourceIntRect, frameData.PixelData, SourceWidth * 4, 0);
            // 只有一帧时，冻结以提升性能（先注释掉，因为冻结后无法修改缩放渲染质量）
            if (FrameList.Count == 1) _display.Freeze();
            // 动态图片，则启动定时器
            if (FrameList.Count > 1)
            {
                AppTimer.Instance.AddTimerHandler(this);
                _startMs = AppWatch.Instance.Milliseconds;
            }
        }

        private void 适配图片大小()
        {
            // 无手动设置图片宽度时，适配原图大小
            if (ImageWidth <= 0)
            {
                适配原图大小();
                return;
            }
            // 先缩放，再适配
            缩放至宽度();
            // 无限制
            if (MaxWidth <= 0 || MaxHeight <= 0 || (double.IsNaN(MaxWidth) && double.IsNaN(MaxHeight)))
                return;
            // 限制了宽度
            if (MaxWidth > 0 && double.IsNaN(MaxHeight))
            {
                if (ActualWidth > MaxWidth) 适配缩放后宽度();
            }
            // 限制了高度
            else if (double.IsNaN(MaxWidth) && MaxHeight > 0)
            {
                if (ActualHeight > MaxHeight) 适配缩放后高度();
            }
            // 限制了宽度和高度
            else if (MaxWidth > 0 && MaxHeight > 0)
            {
                if (ActualWidth / (double)ActualHeight > MaxWidth / (double)MaxHeight) 适配缩放后宽度();
                else 适配缩放后高度();
            }
        }

        private void 缩放至宽度()
        {
            double scale = ImageWidth / SourceWidth;
            ActualWidth = ImageWidth;
            ActualHeight = Math.Round(SourceHeight * scale);
        }

        private void 适配原图大小()
        {
            // 无限制
            if (MaxWidth <= 0 || MaxHeight <= 0 || (double.IsNaN(MaxWidth) && double.IsNaN(MaxHeight)))
            {
                ActualWidth = SourceWidth;
                ActualHeight = SourceHeight;
                return;
            }
            // 限制了宽度
            if (MaxWidth > 0 && double.IsNaN(MaxHeight))
            {
                适配宽度();
            }
            // 限制了高度
            else if (double.IsNaN(MaxWidth) && MaxHeight > 0)
            {
                适配高度();
            }
            // 限制了宽度和高度
            else if (MaxWidth > 0 && MaxHeight > 0)
            {
                if (SourceWidth / (double)SourceHeight > MaxWidth / (double)MaxHeight) 适配宽度();
                else 适配高度();
            }
        }

        private void 适配宽度()
        {
            if (SourceWidth <= MaxWidth)
            {
                ActualWidth = SourceWidth;
                ActualHeight = SourceHeight;
            }
            else
            {
                ActualWidth = MaxWidth;
                ActualHeight = Math.Round(SourceHeight * (MaxWidth / SourceWidth));
            }
        }

        private void 适配缩放后宽度()
        {
            if (ActualWidth <= MaxWidth) return;
            double scale = MaxWidth / ActualWidth;
            ActualWidth = MaxWidth;
            ActualHeight = Math.Round(ActualHeight * scale);
        }

        private void 适配高度()
        {
            if (SourceHeight <= MaxHeight)
            {
                ActualWidth = SourceWidth;
                ActualHeight = SourceHeight;
            }
            else
            {
                ActualHeight = MaxHeight;
                ActualWidth = Math.Round(SourceWidth * (MaxHeight / SourceHeight));
            }
        }

        private void 适配缩放后高度()
        {
            if (ActualHeight <= MaxHeight) return;
            double scale = MaxHeight / ActualHeight;
            ActualHeight = MaxHeight;
            ActualWidth = Math.Round(ActualWidth * scale);
        }

        #endregion

        #region 字段

        private double _imageActualWidth = 0;
        private double _imageActualHeight = 0;

        private readonly ElementLayer _layer = new ElementLayer();

        /// <summary>可写位图。当作图片显示器</summary>
        private WriteableBitmap? _display = null;
        private Int32Rect _sourceIntRect = new Int32Rect();

        private long _startMs = 0;

        private 段落? 图注 = null;

        private double _imageOffset = 0;

        #endregion
    }
}