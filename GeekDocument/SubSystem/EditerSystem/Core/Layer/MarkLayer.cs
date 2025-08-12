using System.Windows;
using System.Windows.Media;
using XLogic.Wpf.Drawing;

namespace GeekDocument.SubSystem.EditerSystem.Core.Layer
{
    /// <summary>
    /// 标记图层。用于绘制输入光标与选中区域
    /// </summary>
    public class MarkLayer : SingleBoard
    {
        /// <summary>光标坐标</summary>
        public Point IBeamPoint { get; set; } = new Point();

        public double LineHeight { get; set; } = 16;

        public int Offset
        {
            get => _offset;
            set
            {
                _offset = value;
                Margin = new Thickness(0, -_offset + 16, 0, 0);
            }
        }

        public override void Init()
        {
            _iBeam.Freeze();
            Offset = 0;
        }

        protected override void OnUpdate()
        {
            double x = IBeamPoint.X;
            double y1 = IBeamPoint.Y;
            double y2 = IBeamPoint.Y + LineHeight;
            _dc.DrawLine(_iBeam, new Point(x, y1), new Point(x, y2));
        }

        private readonly Pen _iBeam = new Pen(new SolidColorBrush(Color.FromRgb(249, 202, 124)), 2);
        private int _offset;
    }
}