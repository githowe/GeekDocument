using GeekDocument.AppTool.Ex;
using GeekDocument.SubSystem.WindowSystem.ColorPick.Tool;
using System.Windows.Media;
using XLogic.Base.Ex;
using MyColor = XLogic.Base.Color;

namespace GeekDocument.SubSystem.WindowSystem.ColorPick.Component
{
    public class ColorBarComponent : Base
    {
        protected override void Init()
        {
            _host.Bar_S.Init();
            _host.Bar_Br.Init();
            _host.Bar_R.Init();
            _host.Bar_G.Init();
            _host.Bar_B.Init();

            _host.Bar_S.ValueChanged = (value) =>
            {
                _mainColor.Saturation = value / 255f * 100f;
                Update();
                NotifySync();
            };
            _host.Bar_Br.ValueChanged = (value) =>
            {
                _mainColor.Brightness = value / 255f * 100f;
                Update();
                NotifySync(ColorElement.Brightness);
            };
            _host.Bar_R.ValueChanged = (value) =>
            {
                _mainColor.Red = (byte)value;
                Update();
                NotifySync();
            };
            _host.Bar_G.ValueChanged = (value) =>
            {
                _mainColor.Green = (byte)value;
                Update();
                NotifySync();
            };
            _host.Bar_B.ValueChanged = (value) =>
            {
                _mainColor.Blue = (byte)value;
                Update();
                NotifySync();
            };
        }

        public override void InitColor(Color color)
        {
            _mainColor = new MyColor(color.R, color.G, color.B);
            Update();
        }

        public override void SyncColor(Color color, ColorElement element)
        {
            _mainColor = new MyColor(color.R, color.G, color.B);
            Update();
        }

        private void Update()
        {
            _updateOnly = true;
            FillSaturationBar();
            FillBrightnessBar();
            FillRedBar();
            FillGreenBar();
            FillBlueBar();
            _updateOnly = false;
        }

        private void NotifySync(ColorElement element = ColorElement.Full)
        {
            if (_updateOnly) return;
            _host.SyncTool.UpdateColor(this, _mainColor.ToMediaColor(), element);
        }

        /// <summary>
        /// 填充饱和度条
        /// </summary>
        private void FillSaturationBar()
        {
            List<Color> colorList = new List<Color>();
            for (int x = 0; x < 256; x++)
            {
                // 计算饱和度
                float saturation = x / 255f * 100f;
                MyColor color = new MyColor(_mainColor.Hue, saturation, _mainColor.Brightness);
                colorList.Add(Color.FromRgb(color.Red, color.Green, color.Blue));
            }
            _host.Bar_S.ColorList = colorList;
            _host.Bar_S.Value = (_mainColor.Saturation / 100f * 255f).RoundInt();
        }

        /// <summary>
        /// 填充亮度条
        /// </summary>
        private void FillBrightnessBar()
        {
            List<Color> colorList = new List<Color>();
            for (int x = 0; x < 256; x++)
            {
                // 计算亮度
                float brightness = x / 255f * 100f;
                MyColor color = new MyColor(_mainColor.Hue, _mainColor.Saturation, brightness);
                colorList.Add(Color.FromRgb(color.Red, color.Green, color.Blue));
            }
            _host.Bar_Br.ColorList = colorList;
            _host.Bar_Br.Value = (_mainColor.Brightness / 100f * 255f).RoundInt();
        }

        /// <summary>
        /// 填充红色条
        /// </summary>
        private void FillRedBar()
        {
            List<Color> colorList = new List<Color>();
            for (int x = 0; x < 256; x++)
                colorList.Add(Color.FromRgb((byte)x, _mainColor.Green, _mainColor.Blue));
            _host.Bar_R.ColorList = colorList;
            _host.Bar_R.Value = _mainColor.Red;
        }

        /// <summary>
        /// 填充绿色条
        /// </summary>
        private void FillGreenBar()
        {
            List<Color> colorList = new List<Color>();
            for (int x = 0; x < 256; x++)
                colorList.Add(Color.FromRgb(_mainColor.Red, (byte)x, _mainColor.Blue));
            _host.Bar_G.ColorList = colorList;
            _host.Bar_G.Value = _mainColor.Green;
        }

        /// <summary>
        /// 填充蓝色条
        /// </summary>
        private void FillBlueBar()
        {
            List<Color> colorList = new List<Color>();
            for (int x = 0; x < 256; x++)
                colorList.Add(Color.FromRgb(_mainColor.Red, _mainColor.Green, (byte)x));
            _host.Bar_B.ColorList = colorList;
            _host.Bar_B.Value = _mainColor.Blue;
        }

        private MyColor _mainColor;
    }
}