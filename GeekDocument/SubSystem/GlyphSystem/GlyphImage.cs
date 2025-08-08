using System.Windows;
using System.Windows.Media.Imaging;

namespace GeekDocument.SubSystem.GlyphSystem
{
    /// <summary>
    /// 字形图片
    /// </summary>
    public class GlyphImage
    {
        /// <summary>字符</summary>
        public char C { get; set; } = default;

        public double GlyphWidth { get; set; } = 0;

        public double GlyphHeight { get; set; } = 0;

        public int RenderWidth { get; set; } = 0;

        public int RenderHeight { get; set; } = 0;

        public byte[] AlphaData { get; set; } = Array.Empty<byte>();

        public Point Origin { get; set; } = new Point(0, 0);

        public override string ToString() => C.ToString();

        public BitmapSource? GetBitmap(byte r, byte g, byte b)
        {
            if (AlphaData.Length == 0) return null;

            string colorCode = $"{r:X2}{g:X2}{b:X2}";
            if (_bitmapDict.TryGetValue(colorCode, out BitmapSource? value)) return value;

            GenerateImage(r, g, b);
            return _bitmapDict[colorCode];
        }

        private void GenerateImage(byte r, byte g, byte b)
        {
            if (AlphaData.Length == 0) return;

            // 创建位图
            WriteableBitmap bitmap = new WriteableBitmap(RenderWidth, RenderHeight, 96, 96, System.Windows.Media.PixelFormats.Bgra32, null);
            // 生成像素数据
            byte[] pixelData = new byte[RenderWidth * RenderHeight * 4];
            for (int alphaIndex = 0; alphaIndex < AlphaData.Length; alphaIndex++)
            {
                if (AlphaData[alphaIndex] == 0) continue;
                int pixelIndex = alphaIndex * 4;
                pixelData[pixelIndex] = b;
                pixelData[pixelIndex + 1] = g;
                pixelData[pixelIndex + 2] = r;
                pixelData[pixelIndex + 3] = AlphaData[alphaIndex];
            }
            // 写入像素数据
            bitmap.WritePixels(new Int32Rect(0, 0, RenderWidth, RenderHeight), pixelData, RenderWidth * 4, 0);
            // 冻结以提升绘制性能
            bitmap.Freeze();
            // 添加到字典中
            _bitmapDict.Add($"{r:X2}{g:X2}{b:X2}", bitmap);
        }

        private readonly Dictionary<string, BitmapSource> _bitmapDict = new Dictionary<string, BitmapSource>();
    }
}