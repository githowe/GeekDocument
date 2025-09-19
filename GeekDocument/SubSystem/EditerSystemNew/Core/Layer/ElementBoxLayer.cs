using System.Windows;
using System.Windows.Media;
using XLogic.Wpf.Drawing;

namespace GeekDocument.SubSystem.EditerSystemNew.Core.Layer
{
    public class ElementBoxLayer : SingleBoard
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

        private Brush _brush = new SolidColorBrush(Color.FromArgb(128, 249, 202, 124));
    }
}