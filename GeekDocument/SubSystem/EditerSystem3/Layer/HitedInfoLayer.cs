using GeekDocument.SubSystem.LayoutEngine;
using System.Windows;
using System.Windows.Media;
using XLogic.Wpf.Drawing;

namespace GeekDocument.SubSystem.EditerSystem3.Layer
{
    /// <summary>
    /// 命中信息图层
    /// </summary>
    public class HitedInfoLayer : SingleBoard
    {
        public 命中信息? HitedInfo { get; set; } = null;

        public override void Init()
        {
            _pen.Freeze();
            _brush.Freeze();
        }

        protected override void OnUpdate()
        {
            if (HitedInfo == null) return;

            // 绘制命中元素区域
            _dc.DrawRectangle(_brush, null, HitedInfo.命中区域);
            // 绘制命中点
            Point hitedPoint = HitedInfo.坐标;
            double x1 = hitedPoint.X - 20;
            double x2 = hitedPoint.X + 21;
            double y1 = hitedPoint.Y - 20;
            double y2 = hitedPoint.Y + 21;
            _dc.DrawLine(_pen, new Point(x1, hitedPoint.Y + 0.5), new Point(x2, hitedPoint.Y + 0.5));
            _dc.DrawLine(_pen, new Point(hitedPoint.X + 0.5, y1), new Point(hitedPoint.X + 0.5, y2));
        }

        private readonly Pen _pen = new Pen(new SolidColorBrush(Color.FromArgb(255, 255, 221, 103)), 1);
        private readonly Brush _brush = new SolidColorBrush(Color.FromArgb(128, 85, 111, 181));
    }
}