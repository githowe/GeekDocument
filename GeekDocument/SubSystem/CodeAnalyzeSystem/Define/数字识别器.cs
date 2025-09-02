namespace GeekDocument.SubSystem.CodeAnalyzeSystem.Define
{
    public class 数字识别器 : 识别器
    {
        public override 词法单元? 识别(string 语句, int startIndex)
        {
            if (!char.IsDigit(语句[startIndex])) return null;

            词法单元 result = new 词法单元 { 类型 = "数字" };
            int index = startIndex;
            int state = 0;
            bool accepted = false;

            while (true)
            {
                if (accepted || index == 语句.Length) break;
                switch (state)
                {
                    case 0:
                        // 直接进入读取整数部分状态
                        state = 1;
                        break;
                    case 1:
                        // 遇到数字，继续读取整数部分
                        if (char.IsDigit(语句[index]))
                        {
                            result.值 += 语句[index];
                            index++;
                        }
                        // 遇到小数点，进入读取小数部分状态
                        else if (语句[index] == '.')
                        {
                            result.值 += 语句[index];
                            state = 2;
                            index++;
                            // 如果读取至末尾，直接接受
                            if (index == 语句.Length) { accepted = true; }
                        }
                        // 遇到其他字符，接受整数部分
                        else state = 3;
                        break;
                    case 2:
                        if (char.IsDigit(语句[index]))
                        {
                            result.值 += 语句[index];
                            index++;
                        }
                        else state = 3;
                        break;
                    case 3:
                        accepted = true;
                        break;
                }
            }

            return result;
        }
    }
}