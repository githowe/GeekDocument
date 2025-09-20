using System.Windows;
using System.Windows.Media;
using XLogic.Wpf.Drawing;

namespace GeekDocument.SubSystem.EditerSystemNew.Core.Layer
{
    /// <summary>
    /// 命中框图层
    /// </summary>
    public class HitBoxLayer : SingleBoard
    {
        public Rect HitedRect { get; set; } = Rect.Empty;

        public override void Init()
        {
            _brush.Freeze();
        }

        protected override void OnUpdate()
        {
            if (!HitedRect.IsEmpty)
                _dc.DrawRectangle(_brush, null, HitedRect);
        }

        private Brush _brush = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255));
    }
}