using GeekDocument.SubSystem.ImageSystem;
using Newtonsoft.Json;
using WpfMath;
using WpfMath.Parsers;
using XLogic.Base.Ex;

namespace GeekDocument.SubSystem.EditerSystem.Define.BlockDerive
{
    public class FormulaBlockData
    {
        public string Latex { get; set; } = "";

        public int Size { get; set; } = 0;

        /// <summary>期望渲染宽度</summary>
        public int RenderWidth { get; set; } = 0;

        public int Align { get; set; } = 0;

        public int MarginTop { get; set; } = 0;

        public int MarginBottom { get; set; } = 0;
    }

    /// <summary>
    /// 公式块
    /// </summary>
    public class BlockFormula : Block
    {
        #region 构造方法

        public BlockFormula() => Type = BlockType.Formula;

        #endregion

        #region 属性

        public string Latex { get; set; } = @"\color[RGB]{255,255,255}{f(x)=\sqrt{x^2+y^2}}";

        public int Size { get; set; } = 24;

        /// <summary>期望渲染宽度</summary>
        public int RenderWidth { get; set; } = 0;

        /// <summary>对齐方式</summary>
        public LineAlignType Align { get; set; } = LineAlignType.Center;

        #endregion

        #region 运行时属性

        /// <summary>像素数据</summary>
        public byte[] PixelData { get; set; } = Array.Empty<byte>();

        /// <summary>源宽度</summary>
        public int SourceWidth { get; set; } = 0;

        /// <summary>源高度</summary>
        public int SourceHeight { get; set; } = 0;

        /// <summary>实际渲染宽度</summary>
        public int RealRenderWidth => _actualWidth;

        /// <summary>实际渲染高度</summary>
        public int RenderHeight => _actualHeight;

        /// <summary>图片横坐标</summary>
        public int ImageX { get; private set; } = 0;

        #endregion

        #region Block 方法

        public override void LoadJson(string json)
        {
            FormulaBlockData? blockData = JsonConvert.DeserializeObject<FormulaBlockData>(json);
            if (blockData == null) return;

            Latex = blockData.Latex;
            Size = blockData.Size;
            RenderWidth = blockData.RenderWidth;
            Align = (LineAlignType)blockData.Align;
            MarginTop = blockData.MarginTop;
            MarginBottom = blockData.MarginBottom;
        }

        public override string ToJson()
        {
            FormulaBlockData blockData = new FormulaBlockData
            {
                Latex = Latex,
                Size = Size,
                RenderWidth = RenderWidth,
                Align = (int)Align,
                MarginTop = MarginTop,
                MarginBottom = MarginBottom
            };
            return JsonConvert.SerializeObject(blockData);
        }

        public override int GetViewHeight() => _actualHeight;

        public override void UpdateViewData(int blockWidth)
        {
            XamlMath.TexFormulaParser 解析器 = WpfTeXFormulaParser.Instance;
            XamlMath.TexFormula 公式 = 解析器.Parse(Latex);
            byte[] sourceData = 公式.RenderToPng(Size, 0, 0, "Arial");

            ImageInfo? imageInfo = ImageLoader.Instance.LoadImageFile(sourceData, "png");
            SourceWidth = imageInfo.Width;
            SourceHeight = imageInfo.Height;
            PixelData = imageInfo.FrameList[0].PixelData;
            // UpdateColor(255, 255, 255);
            CalculateActualSize(blockWidth);
            CalculateImageX(blockWidth);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 更新颜色
        /// </summary>
        private void UpdateColor(byte r, byte g, byte b)
        {
            if (PixelData == null) return;

            int pixelCount = PixelData.Length / 4;
            for (int index = 0; index < pixelCount; index++)
            {
                int offset = index * 4;
                PixelData[offset + 0] = b;
                PixelData[offset + 1] = g;
                PixelData[offset + 2] = r;
            }
        }

        /// <summary>
        /// 计算实际渲染大小
        /// </summary>
        private void CalculateActualSize(int blockWidth)
        {
            // 设置了渲染宽度
            if (RenderWidth > 0)
            {
                // 没有超过块宽度
                if (RenderWidth <= blockWidth) _actualWidth = RenderWidth;
                // 超过块宽度，使用块宽度
                else _actualWidth = blockWidth;
            }
            // 未设置渲染宽度，使用源宽度
            else
            {
                // 没有超过块宽度
                if (SourceWidth <= blockWidth) _actualWidth = SourceWidth;
                // 超过块宽度，使用块宽度
                else _actualWidth = blockWidth;
            }

            // 计算源图比例
            double ratio = (double)SourceHeight / SourceWidth;
            // 计算实际渲染高度
            _actualHeight = (_actualWidth * ratio).RoundInt();
        }

        /// <summary>
        /// 计算图片横坐标
        /// </summary>
        private void CalculateImageX(int blockWidth)
        {
            switch (Align)
            {
                case LineAlignType.Left:
                    ImageX = 0;
                    break;
                case LineAlignType.Center:
                    ImageX = (blockWidth - _actualWidth) / 2;
                    break;
                case LineAlignType.Right:
                    ImageX = blockWidth - _actualWidth;
                    break;
                case LineAlignType.Justify:
                    ImageX = (blockWidth - _actualWidth) / 2;
                    break;
            }
        }

        #endregion

        #region 字段

        /// <summary>实际渲染宽度</summary>
        private int _actualWidth = 0;
        /// <summary>实际渲染高度</summary>
        private int _actualHeight = 0;

        #endregion
    }
}