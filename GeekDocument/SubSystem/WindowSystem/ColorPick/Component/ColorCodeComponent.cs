using GeekDocument.SubSystem.WindowSystem.ColorPick.Tool;
using System.Windows.Media;

namespace GeekDocument.SubSystem.WindowSystem.ColorPick.Component
{
    public class ColorCodeComponent : Base
    {
        protected override void Init()
        {
            _host.Text_Code.TextChanged += Text_Code_TextChanged;
        }

        private void Text_Code_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                if (_updateOnly) return;
                _color = (Color)ColorConverter.ConvertFromString(_host.Text_Code.Text);
                _host.SyncTool.UpdateColor(this, _color);
            }
            catch (Exception) { }
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
            _host.Text_Code.Text = $"#{_color.R:X2}{_color.G:X2}{_color.B:X2}";
            _updateOnly = false;
        }
    }
}