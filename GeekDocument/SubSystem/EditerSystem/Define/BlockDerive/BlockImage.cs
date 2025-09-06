using GeekDocument.SubSystem.ImageSystem;
using GeekDocument.SubSystem.LayoutSystem;
using GeekDocument.SubSystem.WindowSystem;
using Newtonsoft.Json;
using XLogic.Base.Ex;

namespace GeekDocument.SubSystem.EditerSystem.Define.BlockDerive
{
    public class ImageBlockData
    {
        /// <summary>源哈希值</summary>
        public string SourceHash { get; set; } = "";

        /// <summary>期望渲染宽度</summary>
        public int RenderWidth { get; set; } = 0;

        /// <summary>对齐方式</summary>
        public int Align { get; set; } = 0;

        /// <summary>是否为像素画</summary>
        public bool PixelArt { get; set; } = false;

        /// <summary>图注</summary>
        public string? Caption { get; set; } = null;

        public string FontFamily { get; set; } = "仿宋";

        public int FontSize { get; set; } = 16;

        public int MarginTop { get; set; } = 0;

        public int MarginBottom { get; set; } = 0;
    }

    /// <summary>
    /// 图片块
    /// </summary>
    public class BlockImage : Block
    {
        #region 构造方法

        public BlockImage() => Type = BlockType.Image;

        #endregion

        #region 属性

        /// <summary>源哈希值</summary>
        public string SourceHash { get; set; } = "";

        /// <summary>期望渲染宽度</summary>
        public int RenderWidth { get; set; } = 0;

        /// <summary>对齐方式</summary>
        public LineAlignType Align { get; set; } = LineAlignType.Center;

        /// <summary>是否为像素画</summary>
        public bool PixelArt { get; set; } = false;

        /// <summary>图注</summary>
        public string? Caption { get; set; } = null;

        public string FontFamily { get; set; } = "仿宋";

        public int FontSize { get; set; } = 16;

        #endregion

        #region 运行时属性

        /// <summary>帧列表</summary>
        public List<ImageFrame> FrameList { get; set; } = new List<ImageFrame>();

        /// <summary>总持续时长。单位：毫秒</summary>
        public int Duration { get; set; } = 0;

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

        /// <summary>文本行</summary>
        public TextLine? TextLine => _textLine;

        /// <summary>文本纵坐标</summary>
        public int TextY => _actualHeight + _captionTop;

        public int CaptionTop => _captionTop;

        #endregion

        #region Block 方法

        public override void LoadJson(string json)
        {
            ImageBlockData? blockData = JsonConvert.DeserializeObject<ImageBlockData>(json);
            if (blockData == null) return;

            SourceHash = blockData.SourceHash;
            RenderWidth = blockData.RenderWidth;
            Align = (LineAlignType)blockData.Align;
            PixelArt = blockData.PixelArt;
            Caption = blockData.Caption;
            FontFamily = blockData.FontFamily;
            FontSize = blockData.FontSize;
            MarginTop = blockData.MarginTop;
            MarginBottom = blockData.MarginBottom;

            LoadImage();
        }

        public override string ToJson()
        {
            ImageBlockData blockData = new ImageBlockData
            {
                SourceHash = SourceHash,
                RenderWidth = RenderWidth,
                Align = (int)Align,
                PixelArt = PixelArt,
                Caption = Caption,
                FontFamily = FontFamily,
                FontSize = FontSize,
                MarginTop = MarginTop,
                MarginBottom = MarginBottom,
            };
            return JsonConvert.SerializeObject(blockData);
        }

        public override int GetViewHeight()
        {
            // 有图注时，高度 = 图片高度 + 图注上边距 + 图注高度
            if (Caption != null)
                return _actualHeight + _captionTop + _textLineHeight;
            // 无图注时，高度 = 图片高度
            return _actualHeight;
        }

        public override void UpdateViewData(int blockWidth)
        {
            CalculateActualSize(blockWidth);
            CalculateImageX(blockWidth);
            UpdateTextLine(blockWidth);
        }

        #endregion

        #region 私有方法

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

        /// <summary>
        /// 更新文本行
        /// </summary>
        private void UpdateTextLine(int blockWidth)
        {
            _textLine = null;
            _textLineHeight = 0;
            if (Caption == null) return;
            _textLineHeight = FontSize;
            if (Caption == "") return;

            // 生成字列表
            List<Word> wordList = WordSpliter.Instance.ToWordList(Caption);
            // 更新字的字形图片、字号、字间距
            foreach (var word in wordList) word.UpdateGlyphImage(FontFamily, FontSize, false, false);
            foreach (var word in wordList) word.Size = FontSize;
            for (int index = 0; index < wordList.Count - 1; index++)
            {
                Word first = wordList[index];
                Word second = wordList[index + 1];
                // 如果是中文和英文之间，设置间距
                if (first.WordType == WordType.Chinese && second.WordType == WordType.English
                    || first.WordType == WordType.English && second.WordType == WordType.Chinese)
                {
                    first.Interval = first.Size * 0.25;
                }
            }
            // 使用左对齐生成单文本行
            List<TextLine> lineList = new List<TextLine>();
            TextWrapTool.Instance.WrapText(wordList, lineList, blockWidth, LineAlignType.Left, true);
            _textLine = lineList[0];
            // 文本行居中对齐至图片
            {
                // 计算图片中心
                double imageCenter = ImageX + _actualWidth / 2.0;
                // 计算文本行横坐标
                double x = imageCenter - _textLine.CurrentWidth / 2.0;
                // 防止超出左边界
                if (x < 0) x = 0;
                // 移动至横坐标
                _textLine.MoveTo(x);
            }
        }

        /// <summary>
        /// 加载图片。加载块数据后调用
        /// </summary>
        private void LoadImage()
        {
            // 获取图片信息
            ImageInfo? imageInfo = ImageManager.Instance.FindImageInfo(SourceHash);
            if (imageInfo == null)
            {
                WM.ShowErrorTip($"加载图片块“{Caption}”失败：找不到图片信息");
                return;
            }
            SourceWidth = imageInfo.Width;
            SourceHeight = imageInfo.Height;
            FrameList = imageInfo.FrameList;
            Duration = imageInfo.Duration;
        }

        #endregion

        #region 字段

        /// <summary>实际渲染宽度</summary>
        private int _actualWidth = 0;
        /// <summary>实际渲染高度</summary>
        private int _actualHeight = 0;

        /// <summary>图注上边距mmary>
        private readonly int _captionTop = 4;

        /// <summary>文本行。存放图注</summary>
        private TextLine? _textLine = null;
        /// <summary>文本行高度</summary>
        private int _textLineHeight = 0;

        #endregion
    }
}