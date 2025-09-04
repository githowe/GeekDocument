using GeekDocument.SubSystem.CodeAnalyzeSystem;
using GeekDocument.SubSystem.CodeAnalyzeSystem.Define;
using System.Windows.Media;

namespace GeekDocument.SubSystem.EditerSystem.Tool
{
    public class HighlightInfo
    {
        public int StartIndex { get; set; } = 0;

        public int EndIndex { get; set; } = 0;

        public Color Color { get; set; } = new Color();
    }

    public class HighlightResult
    {
        public List<HighlightInfo> InfoList { get; set; } = new List<HighlightInfo>();

        public Color GetColor(int index)
        {
            foreach (var item in InfoList)
            {
                if (item.StartIndex <= index && index <= item.EndIndex) return item.Color;
            }
            return Colors.White;
        }
    }

    public class CodeTool
    {
        #region 单例

        private CodeTool() { }
        public static CodeTool Instance { get; } = new CodeTool();

        #endregion

        public void Init()
        {
            // 填充颜色字典
            _colorDict.Add("注释", new Color { R = 170, G = 200, B = 40 });
            _colorDict.Add("预处理指令", new Color { R = 180, G = 180, B = 180 });
            _colorDict.Add("预处理指令参数", new Color { R = 255, G = 255, B = 255 });
            _colorDict.Add("标识符", new Color { R = 156, G = 220, B = 254 });
            _colorDict.Add("关键字", new Color { R = 86, G = 156, B = 214 });
            _colorDict.Add("类型关键字", new Color { R = 200, G = 188, B = 86 });
            _colorDict.Add("控制流关键字", new Color { R = 200, G = 138, B = 214 });
            _colorDict.Add("运算符", new Color { R = 255, G = 255, B = 255 });
            _colorDict.Add("数字", new Color { R = 255, G = 128, B = 0 });
            _colorDict.Add("分隔符", new Color { R = 255, G = 255, B = 255 });
            _colorDict.Add("字符串", new Color { R = 255, G = 128, B = 0 });
            _colorDict.Add("字符", new Color { R = 255, G = 128, B = 0 });
        }

        /// <summary>
        /// 高亮代码
        /// </summary>
        public HighlightResult Highlighting(string code, string language = "C#")
        {
            HighlightResult result = new HighlightResult();
            词法识别器 lexer = new 词法识别器();
            List<Token> 结果 = lexer.GetTokenList(code);
            foreach (var 词法单元 in 结果)
            {
                HighlightInfo highlightInfo = new HighlightInfo
                {
                    StartIndex = 词法单元.StartIndex,
                    EndIndex = 词法单元.StartIndex + 词法单元.Value.Length - 1,
                    Color = _colorDict[词法单元.Type],
                };
                result.InfoList.Add(highlightInfo);
            }
            return result;
        }

        private readonly Dictionary<string, Color> _colorDict = new Dictionary<string, Color>();
    }
}