using GeekDocument.SubSystem.CodeAnalyzeSystem.Define;

namespace GeekDocument.SubSystem.CodeAnalyzeSystem
{
    public class 词法识别器
    {
        public List<Token> GetTokenList(string input)
        {
            List<Token> result = new List<Token>();
            int index = 0;
            while (index < input.Length)
            {
                // 跳过空白字符
                if (char.IsWhiteSpace(input[index]))
                {
                    index++;
                    continue;
                }
                Token token = null;
                foreach (var 识别器 in 识别器列表)
                {
                    token = 识别器.识别(input, index);
                    if (token != null)
                    {
                        token.StartIndex = index;
                        result.Add(token);
                        index += token.Value.Length;
                        break;
                    }
                }
                // 无识别器能识别当前字符，跳过当前字符
                if (token == null) index++;
            }
            return result;
        }

        private readonly List<识别器> 识别器列表 = new List<识别器>
        {
            new 注释识别器(),
            new 预处理指令识别器(),
            new 标识符与关键字识别器(),
            new 运算符识别器(),
            new 数字识别器(),
            new 分隔符识别器(),
            new 字符串识别器(),
            new 字符识别器()
        };
    }
}