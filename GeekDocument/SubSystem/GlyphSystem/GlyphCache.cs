using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GeekDocument.SubSystem.GlyphSystem
{
    /// <summary>
    /// 字形缓存
    /// </summary>
    public class GlyphCache
    {
        #region 单例

        private GlyphCache() { }
        public static GlyphCache Instance { get; } = new GlyphCache();

        #endregion

        public GlyphImage? GetCodeGlyphImage(char c) => GetGlyphImage(c, "仿宋", 16, false, false);

        public GlyphImage? GetGlyphImage(char c, string fontFamily, int fontSize, bool bold = false, bool italic = false)
        {
            // 生成节点路径
            List<string> path = GeneratePath(c, fontFamily, fontSize, bold, italic);
            // 查找节点
            GlyphNode? node = FindNode(_baseNode, path);
            node ??= CreateNode(GeneratePath(c, fontFamily, fontSize, bold, italic));
            // 如果有字形图片，则返回
            if (node.Image != null) return node.Image;
            // 否则创建新的字形图片，并返回
            node.Image = GenerateGlyphImage(c, fontFamily, fontSize, bold, italic);
            return node.Image;
        }

        private List<string> GeneratePath(char c, string fontFamily, int fontSize, bool bold, bool italic)
        {
            string boldStr = bold ? "B" : "N";
            string italicStr = italic ? "I" : "N";
            return new List<string> { c.ToString(), fontFamily, fontSize.ToString(), boldStr, italicStr };
        }

        private GlyphNode? FindNode(GlyphNode parent, List<string> path)
        {
            // 路径为空，返回父节点
            if (path.Count == 0) return parent;

            // 获取并移除路径的第一个节点
            string firstPathNode = path[0];
            path.RemoveAt(0);
            // 查找子节点
            GlyphNode? node = parent.FindNode(firstPathNode);
            // 找到了，则递归查找
            if (node != null) return FindNode(node, path);
            // 未找到，返回空
            return null;
        }

        private GlyphNode CreateNode(List<string> path)
        {
            GlyphNode current = _baseNode;
            while (true)
            {
                if (path.Count == 0) break;
                string pathNode = path[0];
                path.RemoveAt(0);
                GlyphNode? node = current.FindNode(pathNode);
                if (node == null)
                {
                    node = new GlyphNode { Name = pathNode };
                    current.NodeList.Add(node);
                    node.Parent = current;
                }
                current = node;
            }
            return current;
        }

        private GlyphImage? GenerateGlyphImage(char c, string fontFamily, int fontSize, bool bold, bool italic)
        {
            FontFamily family = new FontFamily(fontFamily);
            FontStyle style = italic ? FontStyles.Italic : FontStyles.Normal;
            FontWeight weight = bold ? FontWeights.Bold : FontWeights.Normal;
            Typeface typeface = new Typeface(family, style, weight, FontStretches.Normal);
            if (typeface.TryGetGlyphTypeface(out GlyphTypeface glyphTypeface))
            {
                // 生成字形
                GlyphRun glyphRun = GenerateGlyphRun(glyphTypeface, c, fontSize);
                // 创建字形图片
                GlyphImage glyphImage = new GlyphImage
                {
                    C = c,
                    GlyphWidth = glyphRun.AdvanceWidths[0],
                    GlyphHeight = fontSize,
                };
                // 获取墨迹区域。此区域以基线左端点为原点，根据矢量图形计算出来的区域，非真正渲染后的区域
                Rect box = glyphRun.ComputeInkBoundingBox();
                // 没有墨迹区域，则无需生成字形图片的渲染数据
                if (box.IsEmpty) return glyphImage;
                // 计算基线位置：相对于画布左上角
                double baseLine = fontSize * (glyphTypeface.Baseline / glyphTypeface.Height);
                // 制定检测区域。扩展大小暂定为4像素
                double left = Math.Floor(box.X) - 4;
                double top = Math.Floor(baseLine + box.Y) - 4;
                double right = Math.Ceiling(box.X + box.Width) + 4;
                double bottom = Math.Ceiling(baseLine + box.Y + box.Height) + 4;
                Rect checkRect = new Rect(left, top, right - left, bottom - top);
                // 创建字形图层
                GlyphLayer layer = new GlyphLayer
                {
                    Width = glyphRun.AdvanceWidths[0],
                    Height = fontSize,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Run = glyphRun
                };
                // 渲染字形
                layer.Update();
                // layer.SaveToPng($"{c}_{fontSize}.png");
                // 创建画布
                Canvas canvas = new Canvas
                {
                    Width = checkRect.Width,
                    Height = checkRect.Height,
                };
                canvas.Measure(new Size(checkRect.Width, checkRect.Height));
                canvas.Arrange(new Rect(0, 0, checkRect.Width, checkRect.Height));
                // 将字形添加到画布
                canvas.Children.Add(layer);
                // 设置字形位置
                Canvas.SetLeft(layer, -left);
                Canvas.SetTop(layer, -top);
                // 更新画布布局
                canvas.UpdateLayout();
                // 渲染画布
                RenderTargetBitmap canvasRender = new RenderTargetBitmap((int)canvas.Width, (int)canvas.Height, 96, 96, PixelFormats.Pbgra32);
                canvasRender.Render(canvas);
                // 保存图片
                /*PngBitmapEncoder png = new PngBitmapEncoder();
                png.Frames.Add(BitmapFrame.Create(canvasRender));
                Stream stream = File.Create($"{c}_{fontSize}.png");
                png.Save(stream);
                stream.Close();*/

                // 获取渲染数据
                byte[] pixels = new byte[(int)(canvasRender.Width * canvasRender.Height * 4)];
                canvasRender.CopyPixels(pixels, (int)canvasRender.Width * 4, 0);
                // 检测边缘
                Rect edgeRect = 检测边缘(pixels, (int)canvasRender.Width, (int)canvasRender.Height);
                // 计算绘制字形时与图层左上角的偏移
                int offset_x = (int)(edgeRect.Left - Canvas.GetLeft(layer));
                int offset_y = (int)(edgeRect.Top - Canvas.GetTop(layer));
                // 裁剪图片
                Canvas clipCanvas = new Canvas
                {
                    Width = edgeRect.Width,
                    Height = edgeRect.Height,
                };
                clipCanvas.Measure(new Size(edgeRect.Width, edgeRect.Height));
                clipCanvas.Arrange(new Rect(0, 0, edgeRect.Width, edgeRect.Height));
                canvas.Children.Remove(layer);
                clipCanvas.Children.Add(layer);
                Canvas.SetLeft(layer, -left - edgeRect.Left);
                Canvas.SetTop(layer, -top - edgeRect.Top);
                clipCanvas.UpdateLayout();
                RenderTargetBitmap clipRender = new RenderTargetBitmap((int)clipCanvas.Width, (int)clipCanvas.Height, 96, 96, PixelFormats.Pbgra32);
                clipRender.Render(clipCanvas);
                byte[] clipedPixel = new byte[(int)(clipCanvas.Width * clipCanvas.Height * 4)];
                clipRender.CopyPixels(clipedPixel, (int)clipRender.Width * 4, 0);
                byte[] clipedAlpha = new byte[(int)(clipCanvas.Width * clipCanvas.Height)];
                int pixelCount = (int)(clipCanvas.Width * clipCanvas.Height);
                for (int index = 0; index < pixelCount; index++)
                    clipedAlpha[index] = clipedPixel[index * 4 + 3];

                /*png = new PngBitmapEncoder();
                png.Frames.Add(BitmapFrame.Create(clipRender));
                stream = File.Create($"{c}_{fontSize}_Cliped.png");
                png.Save(stream);
                stream.Close();*/

                glyphImage.RenderWidth = (int)edgeRect.Width;
                glyphImage.RenderHeight = (int)edgeRect.Height;
                glyphImage.AlphaData = clipedAlpha;
                glyphImage.Origin = new Point(offset_x, offset_y);
                return glyphImage;
            }
            return null;
        }

        private GlyphRun GenerateGlyphRun(GlyphTypeface typeface, char c, int fontSize)
        {
            double baseLine = fontSize * (typeface.Baseline / typeface.Height);
            double pixelEachEM = fontSize / typeface.Height;

            ushort index = 0;
            try { index = typeface.CharacterToGlyphMap[c]; }
            catch { }

            // 计算像素宽度
            double pixelWidth = typeface.AdvanceWidths[index] * pixelEachEM;
            // 创建字形
            GlyphRun run = new GlyphRun(
                glyphTypeface: typeface,
                bidiLevel: 0,
                isSideways: false,
                renderingEmSize: pixelEachEM,
                pixelsPerDip: 1,
                glyphIndices: [index],
                baselineOrigin: new Point(0, baseLine),
                advanceWidths: [pixelWidth],
                glyphOffsets: [new Point(0, 0)],
                characters: null,
                deviceFontName: null,
                clusterMap: null,
                caretStops: null,
                language: null);

            return run;
        }

        private Rect 检测边缘(byte[] pixelData, int width, int height)
        {
            // 检测左边缘
            int left = 0;
            for (int list = 0; list < width; list++)
            {
                if (!检测列是否透明(pixelData, width, height, list))
                {
                    left = list;
                    break;
                }
            }
            // 检测右边缘
            int right = 0;
            for (int list = width - 1; list >= 0; list--)
            {
                if (!检测列是否透明(pixelData, width, height, list))
                {
                    right = list;
                    break;
                }
            }
            // 检测上边缘
            int top = 0;
            for (int line = 0; line < height; line++)
            {
                if (!检测行是否透明(pixelData, width, line))
                {
                    top = line;
                    break;
                }
            }
            // 检测下边缘
            int bottom = 0;
            for (int line = height - 1; line >= 0; line--)
            {
                if (!检测行是否透明(pixelData, width, line))
                {
                    bottom = line;
                    break;
                }
            }
            return new Rect(left, top, right - left + 1, bottom - top + 1);
        }

        private bool 检测列是否透明(byte[] pixelData, int width, int height, int listIndex)
        {
            for (int lineIndex = 0; lineIndex < height; lineIndex++)
            {
                int pixelIndex = lineIndex * width + listIndex;
                if (pixelData[pixelIndex * 4 + 3] > 0) return false;
            }
            return true;
        }

        private bool 检测行是否透明(byte[] pixelData, int width, int lineIndex)
        {
            for (int listIndex = 0; listIndex < width; listIndex++)
            {
                int pixelIndex = lineIndex * width + listIndex;
                if (pixelData[pixelIndex * 4 + 3] > 0) return false;
            }
            return true;
        }

        private readonly GlyphNode _baseNode = new GlyphNode { Name = "Root" };
    }
}