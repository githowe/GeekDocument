using GeekDocument.SubSystem.WindowSystem.ColorPick.Tool;
using System.Windows.Media;

namespace GeekDocument.SubSystem.WindowSystem.ColorPick.Component
{
    public class ColorValueComponent : Base
    {
        protected override void Init()
        {
            _host.Number_R.ValueChanged = () =>
            {
                _color.R = (byte)_host.Number_R.Value;
                NotifySync();
            };
            _host.Number_G.ValueChanged = () =>
            {
                _color.G = (byte)_host.Number_G.Value;
                NotifySync();
            };
            _host.Number_B.ValueChanged = () =>
            {
                _color.B = (byte)_host.Number_B.Value;
                NotifySync();
            };
        }

        public override void InitColor(Color color)
        {
            _color = color;
            Update();
        }

        public override void SyncColor(Color color, ColorElement element)
        {
            _color = color;
            Update();
        }

        private void Update()
        {
            _updateOnly = true;
            _host.Number_R.Value = _color.R;
            _host.Number_G.Value = _color.G;
            _host.Number_B.Value = _color.B;
            _updateOnly = false;
        }

        private void NotifySync()
        {
            if (_updateOnly) return;
            _host.SyncTool.UpdateColor(this, _color);
        }
    }
}