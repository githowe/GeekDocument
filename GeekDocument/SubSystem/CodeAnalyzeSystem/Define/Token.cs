namespace GeekDocument.SubSystem.CodeAnalyzeSystem.Define
{
    public class Token
    {
        public string Type { get; set; } = "";

        public string Value { get; set; } = "";

        public int StartIndex { get; set; } = -1;

        public override string ToString() => $"{Type}: {Value}";
    }
}