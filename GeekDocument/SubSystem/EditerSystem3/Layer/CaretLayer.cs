using System.Windows;
using System.Windows.Media;
using XLogic.Wpf.Drawing;

namespace GeekDocument.SubSystem.EditerSystem3.Layer
{
    /// <summary>
    /// 光标图层
    /// </summary>
    public class CaretLayer : SingleBoard
    {
        public double CaretX { get; set; } = 0;

        public double CaretY { get; set; } = 0;

        public double CaretHeight { get; set; } = 16;

        public double CaretWidth { get; set; } = 1;

        /// <summary>溢出行</summary>
        public double OverLine { get; set; } = 0;

        public override void Init()
        {
            _pen.Freeze();
        }

        protected override void OnUpdate()
        {
            double x = Math.Round(CaretX);
            double y1 = CaretY;
            double y2 = CaretY + CaretHeight;
            _dc.DrawLine(_pen, new Point(x + 0.5, y1 - OverLine), new Point(x + 0.5, y2 + OverLine));
        }

        private readonly Pen _pen = new Pen(new SolidColorBrush(Color.FromRgb(249, 202, 124)), 1);
    }
}