namespace GeekDocument.SubSystem.CodeAnalyzeSystem.Define
{
    public class 运算符识别器 : 识别器
    {
        public override 词法单元? 识别(string 语句, int startIndex)
        {
            if (!运算符起始字符.Contains(语句[startIndex])) return null;

            词法单元 result = new 词法单元 { 类型 = "运算符" };
            int index = startIndex;
            int state = 0;
            bool accepted = false;

            while (true)
            {
                if (accepted || index == 语句.Length) break;
                switch (state)
                {
                    case 0:
                        if (语句[index] == '+')
                        {
                            result.值 = "+";
                            state = 1;
                            index++;
                        }
                        else if (语句[index] == '-')
                        {
                            result.值 = "-";
                            state = 2;
                            index++;
                        }
                        else if (语句[index] == '*')
                        {
                            result.值 = "*";
                            state = 3;
                            index++;
                        }
                        else if (语句[index] == '/')
                        {
                            result.值 = "/";
                            state = 4;
                            index++;
                        }
                        else if (语句[index] == '%')
                        {
                            result.值 = "%";
                            state = 5;
                            index++;
                        }
                        else if (语句[index] == '=')
                        {
                            result.值 = "=";
                            state = 6;
                            index++;
                        }
                        else if (语句[index] == '!')
                        {
                            result.值 = "!";
                            state = 7;
                            index++;
                        }
                        else if (语句[index] == '>')
                        {
                            result.值 = ">";
                            state = 8;
                            index++;
                        }
                        else if (语句[index] == '<')
                        {
                            result.值 = "<";
                            state = 9;
                            index++;
                        }
                        else if (语句[index] == '&')
                        {
                            result.值 = "&";
                            state = 10;
                            index++;
                        }
                        else if (语句[index] == '|')
                        {
                            result.值 = "|";
                            state = 11;
                            index++;
                        }
                        else if (语句[index] == '?')
                        {
                            result.值 = "?";
                            state = 12;
                            index++;
                        }
                        break;

                    case 1:
                        if (语句[index] == '+') result.值 = "++";
                        else if (语句[index] == '=') result.值 = "+=";
                        accepted = true;
                        break;
                    case 2:
                        if (语句[index] == '-') result.值 = "--";
                        else if (语句[index] == '=') result.值 = "-=";
                        accepted = true;
                        break;
                    case 3:
                        if (语句[index] == '=') result.值 = "*=";
                        accepted = true;
                        break;
                    case 4:
                        if (语句[index] == '=') result.值 = "/=";
                        // 遇到注释，直接返回空
                        else if (语句[index] == '/' || 语句[index] == '*') return null;
                        accepted = true;
                        break;
                    case 5:
                        if (语句[index] == '=') result.值 = "%=";
                        accepted = true;
                        break;
                    case 6:
                        if (语句[index] == '=') result.值 = "==";
                        else if (语句[index] == '>') result.值 = "=>";
                        accepted = true;
                        break;
                    case 7:
                        if (语句[index] == '=') result.值 = "!=";
                        accepted = true;
                        break;
                    case 8:
                        if (语句[index] == '=') result.值 = ">=";
                        else if (语句[index] == '>') result.值 = ">>";
                        accepted = true;
                        break;
                    case 9:
                        if (语句[index] == '=') result.值 = "<=";
                        else if (语句[index] == '<') result.值 = "<<";
                        accepted = true;
                        break;
                    case 10:
                        if (语句[index] == '&') result.值 = "&&";
                        else if (语句[index] == '=') result.值 = "&=";
                        accepted = true;
                        break;
                    case 11:
                        if (语句[index] == '|') result.值 = "||";
                        else if (语句[index] == '=') result.值 = "|=";
                        accepted = true;
                        break;
                    case 12:
                        if (语句[index] == '?') result.值 = "??";
                        else if (语句[index] == '.') result.值 = "?.";
                        accepted = true;
                        break;
                }
            }

            return result;
        }

        private readonly string 运算符起始字符 = "+-*/%=!><&|?";
    }
}