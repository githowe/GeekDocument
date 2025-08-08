using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace XLogic.Wpf.Drawing
{
    /// <summary>
    /// 简单画板。仅包含一个可视对象
    /// </summary>
    public abstract class SingleBoard : FrameworkElement
    {
        public SingleBoard()
        {
            AddVisualChild(_visual);
            AddLogicalChild(_visual);
        }

        public Point Point { get; set; } = new Point();

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index) => _visual;

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init() { }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            _dc = _visual.RenderOpen();
            if (IsEnabled) OnUpdate();
            _dc.Close();
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear() => _visual.RenderOpen().Close();

        public void SaveToPng(string path)
        {
            RenderTargetBitmap render = new RenderTargetBitmap((int)Width, (int)Height, 96, 96, PixelFormats.Pbgra32);
            render.Render(this);
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(render));
            Stream stream = File.Create(path);
            png.Save(stream);
            stream.Close();
        }

        protected abstract void OnUpdate();

        private readonly DrawingVisual _visual = new DrawingVisual();
        protected DrawingContext? _dc;
    }
}