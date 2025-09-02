namespace GeekDocument.SubSystem.CodeAnalyzeSystem.Define
{
    public class 分隔符识别器 : 识别器
    {
        public override 词法单元? 识别(string 语句, int startIndex)
        {
            if (!分隔符.Contains(语句[startIndex])) return null;
            词法单元 result = new 词法单元
            {
                类型 = "分隔符",
                值 = 语句[startIndex].ToString()
            };
            return result;
        }

        private readonly string 分隔符 = "(){}[],.;";
    }
}