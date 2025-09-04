namespace GeekDocument.SubSystem.CodeAnalyzeSystem.Define
{
    public abstract class 识别器
    {
        public abstract Token? 识别(string 语句, int startIndex);
    }
}