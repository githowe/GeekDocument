using System.Windows;
using System.Windows.Media;
using XLogic.Wpf.Drawing;

namespace GeekDocument.SubSystem.EditerSystem.Core.Layer
{
    /// <summary>
    /// 光标图层
    /// </summary>
    public class IBeamLayer : SingleBoard
    {
        /// <summary>光标坐标</summary>
        public Point IBeamPoint { get; set; } = new Point();

        /// <summary>光标高度</summary>
        public double LineHeight { get; set; } = 16;

        public override void Init()
        {
            _iBeam.Freeze();
        }

        protected override void OnUpdate()
        {
            double x = IBeamPoint.X;
            double y1 = IBeamPoint.Y;
            double y2 = IBeamPoint.Y + LineHeight;
            _dc.DrawLine(_iBeam, new Point(x, y1), new Point(x, y2));
        }

        private readonly Pen _iBeam = new Pen(new SolidColorBrush(Color.FromRgb(249, 202, 124)), 2);
    }
}