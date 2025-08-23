using System.Windows;
using System.Windows.Media;
using XLogic.Wpf.Drawing;

namespace GeekDocument.SubSystem.EditerSystem.Core.Layer
{
    public class SelectionLayer : SingleBoard
    {
        public List<Rect> RectList { get; set; } = new List<Rect>();

        public override void Init()
        {
            _brush.Freeze();
        }

        protected override void OnUpdate()
        {
            foreach (Rect rect in RectList)
                _dc!.DrawRectangle(_brush, null, rect);
        }

        private readonly Brush _brush = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255));
    }
}