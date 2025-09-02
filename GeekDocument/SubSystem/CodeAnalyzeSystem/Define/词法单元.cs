namespace GeekDocument.SubSystem.CodeAnalyzeSystem.Define
{
    public class 词法单元
    {
        public string 类型 { get; set; } = "";

        public string 值 { get; set; } = "";

        public int StartIndex { get; set; } = -1;

        public override string ToString() => $"{类型}: {值}";
    }
}