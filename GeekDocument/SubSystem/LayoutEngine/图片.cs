using GeekDocument.SubSystem.ImageSystem;
using GeekDocument.SubSystem.TimeSystem;
using GeekDocument.SubSystem.WindowSystem;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XLogic.Wpf;

namespace GeekDocument.SubSystem.LayoutEngine
{
    public enum 图注宽度模式
    {
        图片实际宽度,
        图片最大宽度,
        指定最大宽度,
        固定宽度,
    }

    public class 图片 : 行内元素, ITimerHandler
    {
        public 图片()
        {
            Name = "图片";
            Icon = "Image";
        }

        #region 属性

        /// <summary>图片源哈希值</summary>
        public string SourceHash { get; set; } = "";

        public int ImageWidth { get; set; } = -1;

        public int ImageHeight { get; set; } = -1;

        /// <summary>是否为像素画</summary>
        public bool PixelArt { get; set; } = false;

        /// <summary>图注</summary>
        public string? Caption { get; set; } = null;

        public 图注宽度模式 CaptionWidthMode { get; set; } = 图注宽度模式.图片最大宽度;

        /// <summary>图注最大宽度</summary>
        public double CaptionMaxWidth { get; set; } = double.NaN;

        /// <summary>图注固定宽度</summary>
        public double CaptionWidth { get; set; } = double.NaN;

        /// <summary>图注顶边距</summary>
        public double CaptionTopMargin { get; set; } = 4;

        /// <summary>图注字体</summary>
        public string Font { get; set; } = "霞鹜文楷";

        /// <summary>图注字号</summary>
        public int FontSize { get; set; } = 16;

        #endregion

        #region 运行时属性

        public int SourceWidth { get; set; } = 480;

        public int SourceHeight { get; set; } = 270;

        public int MaxHeight { get; set; } = -1;

        /// <summary>帧列表</summary>
        public List<ImageFrame> FrameList { get; set; } = new List<ImageFrame>();

        /// <summary>总持续时长。单位：毫秒</summary>
        public int Duration { get; set; } = 0;

        /// <summary>帧率</summary>
        public double Fps
        {
            get
            {
                if (FrameList.Count == 0) return double.NaN;
                if (FrameList.Count == 1) return 0;
                // 帧率 = 帧数 / 秒
                return FrameList.Count / (Duration / 1000.0);
            }
        }

        public 段落? 图注段落 => _图注;

        #endregion

        #region 布局元素方法

        public override List<绘图对象> 获取绘图对象()
        {
            List<绘图对象> result = new List<绘图对象>();
            _绘图对象.Name = GetPath();
            result.Add(_绘图对象);
            if (_图注 != null)
                result.AddRange(_图注.获取绘图对象());
            return result;
        }

        public override 命中信息? 获取命中信息(Point point)
        {
            命中信息? result = null;
            // 先检测图片区域
            Rect imageRect = new Rect(Left + _imageOffset, Top, _imageActualWidth, _imageActualHeight);
            if (imageRect.Contains(point))
            {
                result = new 命中信息
                {
                    坐标 = point,
                    命中元素 = this,
                    命中区域 = imageRect,
                    区域名称 = "图片",
                };
                return result;
            }
            // 再检测图注区域
            if (_图注 != null)
            {
                result = _图注.获取命中信息(point);
                if (result != null) return result;
            }
            // 检测自己
            Rect rect = new Rect(Left, Top, ActualWidth, ActualHeight);
            if (rect.Contains(point))
            {
                result = new 命中信息
                {
                    坐标 = point,
                    命中元素 = this,
                    命中区域 = rect,
                    区域名称 = "图片元素区域",
                };
            }
            return result;
        }

        public override 元素行 获取最近元素行(Point point) => _图注.获取最近元素行(point);

        public override Rect GetViewRect() => new Rect(Left, Top, ActualWidth, ActualHeight);

        #endregion

        #region 布局元素核心方法

        public override void Init()
        {
            if (SourceHash == "") throw new Exception("图片源哈希值不能为空");
            LoadImage();
            if (Caption != null)
            {
                CanInput = true;
                _图注 = new 段落
                {
                    文本 = Caption,
                    字体 = Font,
                    字号 = FontSize,
                    纯文本模式 = true,
                    首行缩进 = 0,
                    水平对齐 = 水平对齐方式.Center,
                };
                _图注.Init();
                AddChild(_图注);
            }
        }

        public override void 测量()
        {
            if (!double.IsNaN(MaxWidth))
            {
                适配图片大小();
                约束图注宽度();
            }
            else throw new Exception("图片未设置最大宽度");
            _图注?.测量();
            ActualWidth = _imageActualWidth;
            ActualHeight = _imageActualHeight;
            if (_图注 != null)
            {
                if (_图注.ActualWidth > ActualWidth) ActualWidth = _图注.ActualWidth;
                ActualHeight += CaptionTopMargin + _图注.ActualHeight;
            }
        }

        public override void 重新测量()
        {
            // 当前尺寸
            double oldWidth = ActualWidth;
            double oldHeight = ActualHeight;
            // 重新测量
            测量();
            // 尺寸没有变化，则只需重新排列自身并渲染即可
            if (ActualWidth == oldWidth && ActualHeight == oldHeight)
            {
                排列();
                渲染(null);
            }
            else Parent?.重新测量();
        }

        public override void 排列()
        {
            if (_图注 != null)
            {
                // 图注居中至图片
                if (_图注.ActualWidth < _imageActualWidth)
                    _图注.Left = Left + (_imageActualWidth - _图注.ActualWidth) / 2;
                // 图片居中至图注
                else if (_图注.ActualWidth > _imageActualWidth)
                {
                    _imageOffset = (_图注.ActualWidth - _imageActualWidth) / 2;
                    _图注.Left = Left;
                }
                else _图注.Left = Left;
                _图注.Top = Top + _imageActualHeight + CaptionTopMargin;
                _图注.排列();
            }
        }

        public override void 渲染(DrawingContext? dc)
        {
            double x = Math.Round(Left + _imageOffset);
            double y = Math.Round(Top);
            // 设置图片缩放模式
            // if (PixelArt) RenderOptions.SetBitmapScalingMode(_绘图对象, BitmapScalingMode.NearestNeighbor);
            // else RenderOptions.SetBitmapScalingMode(_绘图对象, BitmapScalingMode.HighQuality);
            // 绘制图片
            DrawingContext self_dc = _绘图对象.RenderOpen();
            self_dc.DrawImage(_display, new Rect(x, y, _imageActualWidth, _imageActualHeight));
            self_dc.Close();
            // 绘制图注
            _图注?.渲染(dc);
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
                WM.ShowErrorTip($"没有找到哈希值为“{SourceHash}”的图片");
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
            // 只有一帧时，冻结以提升性能
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
            // 优先使用指定宽高
            if (ImageWidth != -1 || ImageHeight != -1)
            {
                // 宽高比
                double ratio = SourceWidth / (double)SourceHeight;
                // 仅指定宽度
                if (ImageWidth != -1 && ImageHeight == -1)
                {
                    _imageActualWidth = ImageWidth;
                    _imageActualHeight = (int)Math.Round(ImageWidth / ratio);
                }
                // 仅指定高度
                else if (ImageWidth == -1 && ImageHeight != -1)
                {
                    _imageActualHeight = ImageHeight;
                    _imageActualWidth = (int)Math.Round(ImageHeight * ratio);
                }
                // 指定宽高
                else if (ImageWidth != -1 && ImageHeight != -1)
                {
                    _imageActualWidth = ImageWidth;
                    _imageActualHeight = ImageHeight;
                }
                return;
            }
            // 仅限制最大宽度
            if (!double.IsNaN(MaxWidth) && MaxHeight == -1)
            {
                if (SourceWidth <= MaxWidth)
                {
                    _imageActualWidth = SourceWidth;
                    _imageActualHeight = SourceHeight;
                }
                else
                {
                    _imageActualWidth = (int)MaxWidth;
                    double ratio = (double)_imageActualWidth / SourceWidth;
                    _imageActualHeight = (int)Math.Round(SourceHeight * ratio);
                }
            }
            // 仅限制最大高度
            else if (double.IsNaN(MaxWidth) && MaxHeight > -1)
            {
                if (SourceHeight <= MaxHeight)
                {
                    _imageActualWidth = SourceWidth;
                    _imageActualHeight = SourceHeight;
                }
                else
                {
                    _imageActualHeight = MaxHeight;
                    double ratio = (double)_imageActualHeight / SourceHeight;
                    _imageActualWidth = (int)Math.Round(SourceWidth * ratio);
                }
            }
            // 限制了最大宽高
            else
            {
                if (SourceWidth <= MaxWidth && SourceHeight <= MaxHeight)
                {
                    _imageActualWidth = SourceWidth;
                    _imageActualHeight = SourceHeight;
                }
                else
                {
                    if (SourceWidth / (double)SourceHeight > MaxWidth / MaxHeight) 适配最大宽度();
                    else 适配最大高度();
                }
            }
        }

        private void 适配最大宽度()
        {
            if (SourceWidth <= MaxWidth)
            {
                _imageActualWidth = SourceWidth;
                _imageActualHeight = SourceHeight;
            }
            else
            {
                double ratio = MaxWidth / SourceWidth;
                _imageActualWidth = (int)MaxWidth;
                _imageActualHeight = (int)Math.Round(SourceHeight * ratio);
            }
        }

        private void 适配最大高度()
        {
            if (SourceHeight <= MaxHeight)
            {
                _imageActualWidth = SourceWidth;
                _imageActualHeight = SourceHeight;
            }
            else
            {
                double ratio = MaxHeight / (double)SourceHeight;
                _imageActualHeight = MaxHeight;
                _imageActualWidth = (int)Math.Round(SourceWidth * ratio);
            }
        }

        private void 约束图注宽度()
        {
            if (_图注 == null) return;
            switch (CaptionWidthMode)
            {
                case 图注宽度模式.图片实际宽度:
                    _图注.MaxWidth = _imageActualWidth;
                    break;
                case 图注宽度模式.图片最大宽度:
                    _图注.MaxWidth = MaxWidth;
                    break;
                case 图注宽度模式.指定最大宽度:
                    if (double.IsNaN(CaptionMaxWidth)) throw new Exception("未指定图注最大宽度");
                    _图注.MaxWidth = CaptionMaxWidth;
                    break;
                case 图注宽度模式.固定宽度:
                    if (double.IsNaN(CaptionWidth)) throw new Exception("未指定图注固定宽度");
                    _图注.Width = CaptionWidth;
                    break;
            }
        }

        #endregion

        #region 字段

        private int _imageActualWidth = 0;
        private int _imageActualHeight = 0;

        /// <summary>可写位图。当作图片显示器</summary>
        private WriteableBitmap? _display = null;
        private Int32Rect _sourceIntRect = new Int32Rect();

        private long _startMs = 0;

        private 段落? _图注 = null;

        private readonly 绘图对象 _绘图对象 = new 绘图对象();
        private double _imageOffset = 0;

        #endregion
    }
}