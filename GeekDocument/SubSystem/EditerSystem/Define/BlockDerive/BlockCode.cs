using GeekDocument.SubSystem.LayoutSystem;

namespace GeekDocument.SubSystem.EditerSystem.Define.BlockDerive
{
    /// <summary>
    /// 代码块
    /// </summary>
    public class BlockCode : Block
    {
        public BlockCode() => Type = BlockType.Code;

        #region 属性

        /// <summary>源代码</summary>
        public string SourceCode { get; set; } = "public class Program\r\n{\r\n    static void Main()\r\n    {\r\n        Console.WriteLine(\"Hello World!\");\r\n    }\r\n}";

        /// <summary>字体</summary>
        public string FontFamily { get; set; } = "仿宋";

        /// <summary>字号</summary>
        public int FontSize { get; set; } = 16;

        /// <summary>颜色</summary>
        public string Color { get; set; } = "FFFFFF";

        /// <summary>行间距</summary>
        public int LineSpace { get; set; } = 2;

        #endregion

        #region 运行时属性

        public List<CodeLine> LineList => _lineList;

        public List<CodeLine> NumberList => _numberList;

        #endregion

        #region Block 方法

        public override void UpdateViewData(int blockWidth)
        {
            _lineList.Clear();
            _numberList.Clear();
            // 没有代码
            if (SourceCode == "")
            {
                CodeLine numberLine = new CodeLine()
                {
                    Text = "1",
                    FontFamily = FontFamily,
                    Size = FontSize
                };
                numberLine.UpdateGlyphImage();
                _lineList.Add(numberLine);
                _viewHeight = FontSize;
                return;
            }
            // 分割行
            string code = SourceCode.Replace("\r\n", "\n");
            List<string> lineList = code.Split('\n').ToList();
            // 遍历行
            int index = 0;
            foreach (var line in lineList)
            {
                // 创建并添加代码行
                CodeLine codeLine = new CodeLine()
                {
                    Text = line,
                    FontFamily = FontFamily,
                    Size = FontSize
                };
                codeLine.UpdateGlyphImage();
                _lineList.Add(codeLine);
                // 创建并添加行号行
                CodeLine numberLine = new CodeLine()
                {
                    Text = (index + 1).ToString(),
                    FontFamily = FontFamily,
                    Size = FontSize
                };
                numberLine.UpdateGlyphImage();
                _numberList.Add(numberLine);
                index++;
            }
            // 更新视图高度
            _viewHeight = _lineList.Count * FontSize + (_lineList.Count - 1) * LineSpace;
        }

        public override int GetViewHeight() => _viewHeight;

        public override void LoadJson(string json)
        {

        }

        public override string ToJson()
        {
            return "";
        }

        #endregion

        #region 字段

        /// <summary>代码行列表</summary>
        protected List<CodeLine> _lineList = new List<CodeLine>();
        /// <summary>行号列表</summary>
        protected List<CodeLine> _numberList = new List<CodeLine>();
        private int _viewHeight = 0;

        #endregion
    }
}