using GeekDocument.SubSystem.CodeAnalyzeSystem.Define;

namespace GeekDocument.SubSystem.CodeAnalyzeSystem
{
    public class 词法识别器
    {
        public List<词法单元> 识别语句(string 语句)
        {
            int index = 0;
            List<词法单元> 结果 = new List<词法单元>();
            while (index < 语句.Length)
            {
                // 跳过空白字符
                if (char.IsWhiteSpace(语句[index]))
                {
                    index++;
                    continue;
                }
                词法单元 token = null;
                foreach (var 识别器 in 识别器列表)
                {
                    token = 识别器.识别(语句, index);
                    if (token != null)
                    {
                        token.StartIndex = index;
                        结果.Add(token);
                        index += token.值.Length;
                        break;
                    }
                }
                // 无识别器能识别当前字符，跳过当前字符
                if (token == null) index++;
            }

            return 结果;
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