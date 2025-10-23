using GeekDocument.SubSystem.ImageSystem;
using GeekDocument.SubSystem.ResourceSystem;
using GeekDocument.SubSystem.WindowSystem;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfMath;
using WpfMath.Parsers;

namespace GeekDocument.SubSystem.LayoutEngine.Element
{
    public class 公式 : 行内元素
    {
        public 公式()
        {
            Name = "公式";
            Icon = "Sqrt";
        }

        #region 属性

        public string Latex { get; set; } = @"f(x)=\sqrt{x^2+y^2}";

        public int Size { get; set; } = 24;

        public string Color { get; set; } = "FFFFFF";

        #endregion

        #region 运行时属性

        public int SourceWidth { get; set; } = 0;

        public int SourceHeight { get; set; } = 0;

        /// <summary>像素数据</summary>
        public byte[] PixelData { get; set; } = Array.Empty<byte>();

        #endregion

        #region 布局元素方法

        public override void Init()
        {
            try
            {
                // 将公式渲染为图片
                XamlMath.TexFormulaParser 解析器 = WpfTeXFormulaParser.Instance;
                XamlMath.TexFormula 公式 = 解析器.Parse(Latex);
                byte[] sourceData = 公式.RenderToPng(Size, 0, 0, "Arial");
                // 加载图片数据
                ImageInfo? imageInfo = ImageLoader.Instance.LoadImageFile(sourceData, "png");
                SourceWidth = imageInfo.Width;
                SourceHeight = imageInfo.Height;
                PixelData = imageInfo.FrameList[0].PixelData;
                UpdateColor(255, 255, 255);
            }
            catch (Exception ex)
            {
                WM.ShowErrorTip("公式渲染失败：" + ex.Message);
                _errorImage = ImageResManager.Instance.GetIcon16("ErrorArea.png");
                if (_errorImage == null) return;
                SourceWidth = _errorImage.PixelWidth;
                SourceHeight = _errorImage.PixelHeight;
                PixelData = new byte[SourceWidth * SourceHeight * 4];
                _errorImage.CopyPixels(PixelData, SourceWidth * 4, 0);
            }

            // 初始化显示器
            InitDisplay();
            // 公式添加边距以区分正文
            LeftMargin = 4;
            RightMargin = 4;
        }

        public override void 测量()
        {
            // 注意：公式元素忽略宽高限制
            ActualWidth = SourceWidth;
            ActualHeight = SourceHeight;
        }

        public override void 渲染(DrawingContext? dc)
        {
            // 公式元素使用元素行的绘图上下文
            if (dc == null) return;
            double x = Math.Round(Left);
            double y = Math.Round(Top);
            dc.DrawImage(_display, new Rect(x, y, ActualWidth, ActualHeight));
        }

        public override 命中信息? 获取命中信息(Point point)
        {
            命中信息? result = null;
            Rect imageRect = new Rect(Left, Top, ActualWidth, ActualHeight);
            if (imageRect.Contains(point))
            {
                result = new 命中信息
                {
                    坐标 = point,
                    命中元素 = this,
                    命中区域 = imageRect,
                    区域名称 = "公式",
                };
                return result;
            }
            return null;
        }

        public override Rect GetViewRect() => new Rect(Left, Top, ActualWidth, ActualHeight);

        #endregion

        #region 私有方法

        /// <summary>
        /// 更新颜色
        /// </summary>
        private void UpdateColor(byte r, byte g, byte b)
        {
            if (PixelData == null) return;

            int pixelCount = PixelData.Length / 4;
            for (int index = 0; index < pixelCount; index++)
            {
                int offset = index * 4;
                PixelData[offset + 0] = b;
                PixelData[offset + 1] = g;
                PixelData[offset + 2] = r;
            }
        }

        private void InitDisplay()
        {
            _display = new WriteableBitmap(SourceWidth, SourceHeight, 96, 96, PixelFormats.Bgra32, null);
            Int32Rect rect = new Int32Rect(0, 0, SourceWidth, SourceHeight);
            _display.WritePixels(rect, PixelData, SourceWidth * 4, 0);
            _display.Freeze();
        }

        #endregion

        #region 字段

        private WriteableBitmap? _display = null;
        private BitmapImage? _errorImage = null!;

        #endregion
    }
}