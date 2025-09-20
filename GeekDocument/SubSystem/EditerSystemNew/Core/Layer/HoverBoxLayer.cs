using System.Windows;
using System.Windows.Media;
using XLogic.Wpf.Drawing;

namespace GeekDocument.SubSystem.EditerSystemNew.Core.Layer
{
    /// <summary>
    /// 悬停框图层
    /// </summary>
    public class HoverBoxLayer : SingleBoard
    {
        public List<Rect> RectList { get; set; } = new List<Rect>();

        public override void Init()
        {
            _brush.Freeze();
        }

        public void UpdateRect(Rect rect)
        {
            RectList.Clear();
            RectList.Add(rect);
        }

        protected override void OnUpdate()
        {
            foreach (var rect in RectList) _dc.DrawRectangle(_brush, null, rect);
        }

        private Brush _brush = new SolidColorBrush(Color.FromArgb(128, 85, 111, 181));
    }
}