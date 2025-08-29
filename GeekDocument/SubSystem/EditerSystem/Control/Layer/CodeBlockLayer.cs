using GeekDocument.SubSystem.EditerSystem.Define.BlockDerive;
using GeekDocument.SubSystem.LayoutSystem;
using System.Windows;
using System.Windows.Media;
using XLogic.Base.Ex;

namespace GeekDocument.SubSystem.EditerSystem.Control.Layer
{
    public class CodeBlockLayer : BlockLayer
    {
        #region 属性

        public BlockCode Block { get; set; }

        public override int BlockHeight => _blockHeight;

        public override int CharIndex => 0;

        public override int CharIndexMax => 0;

        #endregion

        public override void Init()
        {
            _代码背景.Freeze();
            _行号背景.Freeze();
        }

        protected override void OnUpdate()
        {
            // 代码块高度 = 代码区高度 + 上下边距
            _blockHeight = Block.GetViewHeight() + _padding * 2;
            // 行号区宽度 = 最长行号宽度 + 双倍边距
            int numberAreaWidth = Block.NumberList.Last().GetWidth().RoundInt() + _padding * 2;
            // 绘制底框
            _dc.DrawRectangle(_行号背景, null, new Rect(0, 0, numberAreaWidth, _blockHeight));
            _dc.DrawRectangle(_代码背景, null, new Rect(numberAreaWidth, 0, BlockWidth - numberAreaWidth, _blockHeight));
            // 绘制行号
            int y = _padding;
            foreach (var line in Block.NumberList)
            {
                DrawLine(line, (numberAreaWidth - _padding - line.GetWidth()).RoundInt(), y, 128, 128, 128);
                y += Block.FontSize + Block.LineSpace;
            }
            // 绘制代码
            y = _padding;
            foreach (var line in Block.LineList)
            {
                DrawLine(line, numberAreaWidth + _padding, y, 255, 255, 255);
                y += Block.FontSize + Block.LineSpace;
            }
            // 语言区大小
            double languageAreaWidth = (Block.LanguageLine.GetWidth() + _padding).RoundInt();
            double languageAreaHeight = Block.FontSize + _padding;
            _dc.DrawRectangle(_行号背景, null, new Rect(BlockWidth - languageAreaWidth, 0, languageAreaWidth, languageAreaHeight));
            DrawLine(Block.LanguageLine, (BlockWidth - languageAreaWidth).RoundInt() + _padding / 2, _padding / 2, 255, 221, 103);
        }

        private void DrawLine(CodeLine line, int left, int top, byte r, byte g, byte b)
        {
            int index = 0;
            List<double> xList = line.GetXList(left);
            foreach (var item in line.GlyphImageList)
            {
                double x = xList[index];
                Point leftTop = new Point((x + item.Origin.X).Round(), top + item.Origin.Y);
                _dc.DrawImage(item.GetBitmap(r, g, b), new Rect(leftTop, new Size(item.RenderWidth, item.RenderHeight)));
                index++;
            }
        }

        private readonly Brush _代码背景 = new SolidColorBrush(Color.FromArgb(255, 24, 24, 24));
        private readonly Brush _行号背景 = new SolidColorBrush(Color.FromArgb(255, 16, 16, 16));

        private int _blockHeight = 0;
        private readonly int _padding = 16;
    }
}