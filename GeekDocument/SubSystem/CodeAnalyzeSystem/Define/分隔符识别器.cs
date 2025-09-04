namespace GeekDocument.SubSystem.CodeAnalyzeSystem.Define
{
    public class 分隔符识别器 : 识别器
    {
        public override Token? 识别(string 语句, int startIndex)
        {
            if (!分隔符.Contains(语句[startIndex])) return null;
            Token result = new Token
            {
                Type = "分隔符",
                Value = 语句[startIndex].ToString()
            };
            return result;
        }

        private readonly string 分隔符 = "(){}[],.;";
    }
}