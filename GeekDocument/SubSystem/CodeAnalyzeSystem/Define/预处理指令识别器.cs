namespace GeekDocument.SubSystem.CodeAnalyzeSystem.Define
{
    public class 预处理指令识别器 : 识别器
    {
        public override 词法单元? 识别(string 语句, int startIndex)
        {
            // 如果第一个字符不是“#”，说明不是指令
            if (语句[startIndex] != '#') return null;

            词法单元 result = new 词法单元 { 类型 = "预处理指令" };
            int index = startIndex;
            int state = 0;
            bool accepted = false;

            while (true)
            {
                if (accepted || index == 语句.Length) break;
                switch (state)
                {
                    case 0:
                        // 添加第一个字符“#”，进入状态“1”，读取指令名称
                        result.值 += 语句[index];
                        index++;
                        state = 1;
                        break;
                    case 1:
                        // 遇到空格，开始读取参数
                        if (char.IsWhiteSpace(语句[index]))
                        {
                            result.值 += 语句[index];
                            index++;
                            state = 2;
                        }
                        // 遇到换行，结束读取
                        else if (语句[index] == '\r' || 语句[index] == '\n')
                        {
                            accepted = true;
                        }
                        // 其他字符，视为名称的一部分
                        else
                        {
                            result.值 += 语句[index];
                            index++;
                        }
                        break;
                    case 2:
                        // 遇到换行，结束读取
                        if (语句[index] == '\r' || 语句[index] == '\n')
                        {
                            accepted = true;
                        }
                        else
                        {
                            result.值 += 语句[index];
                            index++;
                        }
                        break;
                }
            }

            return result;
        }
    }
}