namespace GeekDocument.SubSystem.CodeAnalyzeSystem.Define
{
    public class 字符识别器 : 识别器
    {
        public override 词法单元? 识别(string 语句, int startIndex)
        {
            if (语句[startIndex] != '\'') return null;
            词法单元 result = new 词法单元 { 类型 = "字符" };

            int index = startIndex;
            int state = 0;
            bool accepted = false;

            while (true)
            {
                if (accepted || index == 语句.Length) break;
                switch (state)
                {
                    case 0:
                        // 读取起始单引号
                        if (语句[index] == '\'')
                        {
                            result.值 += '\'';
                            index++;
                            state = 1;
                        }
                        break;
                    case 1:
                        // 遇到换行符，说明字符没有闭合，直接接受
                        if (语句[index] == '\n' || 语句[index] == '\r') accepted = true;
                        // 遇到非单引号，继续读取字符
                        else if (语句[index] != '\'')
                        {
                            result.值 += 语句[index];
                            index++;
                        }
                        // 遇到单引号
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
                                result.值 += '\'';
                                index++;
                            }
                            // 否则，结束字符读取
                            else
                            {
                                result.值 += '\'';
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