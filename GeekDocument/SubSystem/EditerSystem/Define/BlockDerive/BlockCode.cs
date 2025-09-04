using GeekDocument.SubSystem.EditerSystem.Tool;
using GeekDocument.SubSystem.LayoutSystem;
using Newtonsoft.Json;
using XLogic.Base.Ex;

namespace GeekDocument.SubSystem.EditerSystem.Define.BlockDerive
{
    public class CodeBlockData
    {
        /// <summary>语言</summary>
        public string Language { get; set; } = "C#";

        /// <summary>源代码</summary>
        public string SourceCode { get; set; } = "";

        /// <summary>字体</summary>
        public string FontFamily { get; set; } = "仿宋";

        /// <summary>字号</summary>
        public int FontSize { get; set; } = 16;

        /// <summary>行间距</summary>
        public int LineSpace { get; set; } = 2;
    }

    /// <summary>
    /// 代码块
    /// </summary>
    public class BlockCode : Block
    {
        public BlockCode() => Type = BlockType.Code;

        #region 属性

        /// <summary>语言</summary>
        public string Language { get; set; } = "C#";

        /// <summary>源代码</summary>
        public string SourceCode { get; set; } = "public class Program\r\n{\r\n    static void Main()\r\n    {\r\n        Console.WriteLine(\"Hello World!\");\r\n    }\r\n}";

        /// <summary>字体</summary>
        public string FontFamily { get; set; } = "仿宋";

        /// <summary>字号</summary>
        public int FontSize { get; set; } = 16;

        /// <summary>行间距</summary>
        public int LineSpace { get; set; } = 2;

        #endregion

        #region 运行时属性

        public List<string> SourceLineList { get; set; } = new List<string>();

        public CodeLine LanguageLine => _languageLine;

        public List<CodeLine> LineList => _lineList;

        public List<CodeLine> NumberList => _numberList;

        public HighlightResult HighlightResult => _highlightResult;

        #endregion

        #region Block 方法

        public override void Init()
        {
            // 初始化语言行
            _languageLine.Text = Language;
            _languageLine.FontFamily = "新宋体";
            _languageLine.Size = FontSize;
            _languageLine.UpdateGlyphImage();
            // 没有代码
            if (SourceCode == "")
            {
                // 创建空行
                SourceLineList.Add("");
                return;
            }
            // 制表符替换为空格
            string code = SourceCode.Replace("\t", "    ");
            // 统一换行符
            code = code.Replace("\r\n", "\n");
            // 分割行
            SourceLineList = code.Split('\n').ToList();
            // 更新源代码
            SourceCode = code;
        }

        public override void UpdateViewData(int blockWidth)
        {
            UpdateViewData();
        }

        public override int GetViewHeight() => _viewHeight;

        public override void LoadJson(string json)
        {
            CodeBlockData? blockData = JsonConvert.DeserializeObject<CodeBlockData>(json);
            if (blockData == null) return;

            Language = blockData.Language;
            SourceCode = blockData.SourceCode;
            FontFamily = blockData.FontFamily;
            FontSize = blockData.FontSize;
            LineSpace = blockData.LineSpace;
        }

        public override string ToJson()
        {
            SourceCode = SourceLineList.ToListString("\n");
            CodeBlockData blockData = new CodeBlockData
            {
                Language = Language,
                SourceCode = SourceCode,
                FontFamily = FontFamily,
                FontSize = FontSize,
                LineSpace = LineSpace,
            };
            return JsonConvert.SerializeObject(blockData);
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 移除行
        /// </summary>
        public void RemoveLine(CodeLine line)
        {
            // 删除源代码行
            int index = _lineList.IndexOf(line);
            SourceLineList.RemoveAt(index);
            SourceCode = SourceLineList.ToListString("\n");
            // 更新
            UpdateViewData();
        }

        /// <summary>
        /// 移除字符
        /// </summary>
        public void RemoveChar(CodeLine line, int charIndex)
        {
            // 更新源代码行内容
            int index = _lineList.IndexOf(line);
            SourceLineList[index] = SourceLineList[index].Remove(charIndex, 1);
            // 更新代码行
            line.Text = SourceLineList[index];
            line.UpdateGlyphImage();
            // 更新源代码以及高亮
            SourceCode = SourceLineList.ToListString("\n");
            _highlightResult = CodeTool.Instance.Highlighting(SourceCode);
        }

        public void 合并至上一行(CodeLine line)
        {
            // 合并行至上一行
            int index = _lineList.IndexOf(line);
            SourceLineList[index - 1] = SourceLineList[index - 1] + line.Text;
            // 删除当前行
            SourceLineList.RemoveAt(index);
            SourceCode = SourceLineList.ToListString("\n");
            // 更新
            UpdateViewData();
        }

        public void 插入行(int index, string text)
        {
            SourceLineList.Insert(index, text);
            SourceCode = SourceLineList.ToListString("\n");
            UpdateViewData();
        }

        public void 分割行(int lineIndex, int splitIndex)
        {
            // 获取分割索引后的内容
            string splitText = SourceLineList[lineIndex].Substring(splitIndex);
            // 更新当前行内容
            SourceLineList[lineIndex] = SourceLineList[lineIndex].Substring(0, splitIndex);
            // 插入新行
            SourceLineList.Insert(lineIndex + 1, splitText);
            SourceCode = SourceLineList.ToListString("\n");
            UpdateViewData();
        }

        public void 插入文本(int lineIndex, int insertIndex, string text)
        {
            SourceLineList[lineIndex] = SourceLineList[lineIndex].Insert(insertIndex, text);
            SourceCode = SourceLineList.ToListString("\n");
            UpdateViewData();
        }

        public void UpdateSouceCode()
        {
            SourceCode = SourceLineList.ToListString("\n");
            UpdateViewData();
        }

        #endregion

        #region 私有方法

        private void UpdateViewData()
        {
            _lineList.Clear();
            _numberList.Clear();
            // 遍历行
            int index = 0;
            foreach (var line in SourceLineList)
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
            // 高亮代码
            _highlightResult = CodeTool.Instance.Highlighting(SourceCode);
        }

        #endregion

        #region 字段

        /// <summary>语言行</summary>
        private readonly CodeLine _languageLine = new CodeLine();
        /// <summary>代码行列表</summary>
        private readonly List<CodeLine> _lineList = new List<CodeLine>();
        /// <summary>行号列表</summary>
        private readonly List<CodeLine> _numberList = new List<CodeLine>();

        private int _viewHeight = 0;

        private HighlightResult _highlightResult;

        #endregion
    }
}