namespace GeekDocument.SubSystem.CodeAnalyzeSystem.Define
{
    public class 字符串识别器 : 识别器
    {
        public override Token? 识别(string 语句, int startIndex)
        {
            if (语句[startIndex] != '"') return null;
            Token result = new Token { Type = "字符串" };

            int index = startIndex;
            int state = 0;
            bool accepted = false;

            while (true)
            {
                if (accepted || index == 语句.Length) break;
                switch (state)
                {
                    case 0:
                        if (语句[index] == '"')
                        {
                            result.Value += '"';
                            index++;
                            state = 1;
                        }
                        break;
                    case 1:
                        // 遇到换行符，说明字符没有闭合，直接接受
                        if (语句[index] == '\n' || 语句[index] == '\r') accepted = true;
                        // 遇到非引号，继续读取字符串内容
                        else if (语句[index] != '"')
                        {
                            result.Value += 语句[index];
                            index++;
                        }
                        // 遇到引号
                        else
                        {
                            // 统计引号前连续反斜杠数量
                            int 斜杠数量 = 0;
                            int 回退索引 = index - 1;
                            while (回退索引 >= startIndex && 语句[回退索引] == '\\')
                            {
                                斜杠数量++;
                                回退索引--;
                            }
                            // 数量为奇数，说明引号是转义的，不作为结束符
                            if (斜杠数量 % 2 == 1)
                            {
                                result.Value += '"';
                                index++;
                            }
                            // 否则，结束字符串读取
                            else
                            {
                                result.Value += '"';
                                index++;
                                state = 2;
                            }
                        }
                        break;
                    case 2:
                        accepted = true;
                        break;
                }
            }

            return result;
        }
    }
}