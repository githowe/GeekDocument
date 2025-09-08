using GeekDocument.AppTool.Ex;
using GeekDocument.SubSystem.WindowSystem.ColorPick.Tool;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XLogic.Base.Ex;
using MyColor = XLogic.Base.Color;

namespace GeekDocument.SubSystem.WindowSystem.ColorPick.Component
{
    public class ColorCubeComponent : Base
    {
        protected override void Init()
        {
            _host.ColorCube.Source = _colorCube;
        }

        public override void InitColor(Color color)
        {
            _myColor = new MyColor(color.R, color.G, color.B)
            {
                BrightnessUpdated = UpdateColorCube
            };
            UpdateColorCube();
            UpdatePickFrame();
        }

        public override void SyncColor(Color color, ColorElement element)
        {
            _myColor.UpdateTo(color.R, color.G, color.B);
            if (element != ColorElement.Brightness) UpdatePickFrame();
        }

        public void MovePickFrame(double x, double y)
        {
            _host.PickFrame.Margin = new Thickness(x - 8, y - 8, 0, 0);
            _myColor.Hue = (x / 255.0 * 359).RoundInt();
            _myColor.Saturation = ((255 - y) / 255.0 * 100).RoundInt();
            _host.SyncTool.UpdateColor(this, _myColor.ToMediaColor());
        }

        private void UpdateColorCube()
        {
            int bitsPerPixel = _colorCube.Format.BitsPerPixel;
            Int32Rect rect = new Int32Rect(0, 0, 256, 256);
            byte[] pixels = new byte[256 * 256 * bitsPerPixel / 8];
            int pixelOffset;

            // 色相、饱和度
            float hue, saturation;
            // 遍历纵坐标
            for (int y = 0; y < 256; y++)
            {
                // 计算饱和度
                saturation = (255 - y) / 255f * 100f;
                // 遍历横坐标
                for (int x = 0; x < 256; x++)
                {
                    // 计算色相
                    hue = x / 255f * 359f;
                    // 创建颜色
                    MyColor color = new MyColor(hue, saturation, _myColor.Brightness);
                    pixelOffset = (x + 256 * y) * bitsPerPixel / 8;
                    pixels[pixelOffset] = color.Blue;
                    pixels[pixelOffset + 1] = color.Green;
                    pixels[pixelOffset + 2] = color.Red;
                    pixels[pixelOffset + 3] = 255;
                }
            }
            int stride = 256 * bitsPerPixel / 8;
            _colorCube.WritePixels(rect, pixels, stride, 0);
        }

        /// <summary>
        /// 更新拾取框
        /// </summary>
        private void UpdatePickFrame()
        {
            int x = (_myColor.Hue / 359.0 * 255.0).RoundInt();
            int y = 255 - (_myColor.Saturation / 100.0 * 255.0).RoundInt();
            _host.PickFrame.Margin = new Thickness(x - 8, y - 8, 0, 0);
        }

        private MyColor _myColor;
        private readonly WriteableBitmap _colorCube = new WriteableBitmap(256, 256, 96, 96, PixelFormats.Pbgra32, null);
    }
}