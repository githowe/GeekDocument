namespace GeekDocument.SubSystem.LayoutSystem
{
    /// <summary>
    /// 字符串片段
    /// </summary>
    public class StringSegment
    {
        public bool Chinese { get; set; } = false;

        public string Text { get; set; } = "";

        /// <summary>第一个字符的索引</summary>
        public int FirstCharIndex { get; set; } = -1;

        public override string ToString() => $"{FirstCharIndex:000}.{Text}";
    }

    /// <summary>
    /// 字符串分割器。将字符串中的中英文分割为片段
    /// </summary>
    public class StringSpliter
    {
        private StringSpliter() { }
        public static StringSpliter Instance { get; } = new StringSpliter();

        /// <summary>
        /// 转换为字符串列表
        /// </summary>
        public List<StringSegment> Split(string value)
        {
            _sourceString = value;
            _offset = 0;

            List<StringSegment> result = new List<StringSegment>();

            while (_offset < _sourceString.Length)
            {
                // 尝试获取中文字符串
                string str = GetChineseString();
                // 获取成功
                if (str != "")
                {
                    StringSegment segment = new StringSegment
                    {
                        Chinese = true,
                        Text = str,
                        FirstCharIndex = _offset - str.Length
                    };
                    result.Add(segment);
                }
                else
                {
                    str = GetEnglishString();
                    if (str != "")
                    {
                        StringSegment segment = new StringSegment
                        {
                            Text = str,
                            FirstCharIndex = _offset - str.Length
                        };
                        result.Add(segment);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 获取中文字符串 - 直到遇到英文字符
        /// </summary>
        private string GetChineseString()
        {
            string strChinese = "";
            for (int index = _offset; index < _sourceString.Length; index++)
            {
                if (char.IsControl(_sourceString[index])) continue;
                if (_sourceString[index] > 255)
                {
                    strChinese += _sourceString[index];
                    _offset++;
                }
                else break;
            }
            return strChinese;
        }

        /// <summary>
        /// 获取英文字符串 - 直到遇到中文字符
        /// </summary>
        private string GetEnglishString()
        {
            string strEnglish = "";
            for (int index = _offset; index < _sourceString.Length; index++)
            {
                if (char.IsControl(_sourceString[index])) continue;
                if (_sourceString[index] <= 255)
                {
                    strEnglish += _sourceString[index];
                    _offset++;
                }
                else break;
            }
            return strEnglish;
        }

        private string _sourceString = "";
        private int _offset = 0;
    }
}