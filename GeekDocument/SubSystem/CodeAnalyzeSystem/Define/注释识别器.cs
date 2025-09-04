namespace GeekDocument.SubSystem.CodeAnalyzeSystem.Define
{
    public class 注释识别器 : 识别器
    {
        public override Token? 识别(string 语句, int startIndex)
        {
            // 如果第一个字符不是 '/'，说明不是注释
            if (语句[startIndex] != '/') return null;

            Token result = new Token { Type = "注释" };
            int index = startIndex;
            int state = 0;
            bool accepted = false;

            while (true)
            {
                if (accepted || index == 语句.Length) break;
                switch (state)
                {
                    case 0:
                        // 移动至下一个字符
                        index++;
                        // 如果没有下一个字符，表示不是注释，直接返回空
                        if (index == 语句.Length) return null;

                        // 下一个字符是 '/'，进入单行注释状态
                        if (语句[index] == '/')
                        {
                            result.Value = "//";
                            state = 1;
                            index++;
                        }
                        // 下一个字符是 '*'，进入多行注释状态
                        else if (语句[index] == '*')
                        {
                            result.Value = "/*";
                            state = 2;
                            index++;
                        }
                        // 否则不是注释，返回空
                        else return null;
                        break;
                    case 1:
                        // 单行注释，读取直到行尾
                        if (index < 语句.Length && 语句[index] != '\n' && 语句[index] != '\r')
                        {
                            result.Value += 语句[index];
                            index++;
                        }
                        else accepted = true;
                        break;
                    case 2:
                        // 多行注释，读取直到遇到 '*/'
                        if (index < 语句.Length)
                        {
                            // 遇到 '*'，可能是注释结束符
                            if (语句[index] == '*')
                            {
                                // 如果下一个字符是 '/'，注释结束
                                if (index + 1 < 语句.Length && 语句[index + 1] == '/')
                                {
                                    result.Value += "*/";
                                    index += 2;
                                    accepted = true;
                                }
                                else
                                {
                                    result.Value += 语句[index];
                                    index++;
                                }
                            }
                            else
                            {
                                result.Value += 语句[index];
                                index++;
                            }
                        }
                        else accepted = true;
                        break;
                }
            }

            return result;
        }
    }
}