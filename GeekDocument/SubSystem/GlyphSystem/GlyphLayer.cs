using System.Windows.Media;
using XLogic.Wpf.Drawing;

namespace GeekDocument.SubSystem.GlyphSystem
{
    public class GlyphLayer : SingleBoard
    {
        public GlyphRun? Run { get; set; } = null;

        protected override void OnUpdate()
        {
            if (Run == null) return;
            _dc.DrawGeometry(Brushes.White, null, Run.BuildGeometry());
        }
    }
}