namespace GeekDocument.SubSystem.LayoutEngine.Element
{
    public class 项信息
    {
        public 项信息() { }

        public 项信息(int deep, 段落 段落)
        {
            段落.Init();
            Deep = deep;
            this.段落 = 段落;
        }

        public int Deep { get; set; } = 0;

        public 段落 段落 { get; set; } = null!;

        public override string ToString()
        {
            string text = 段落.获取文本().Replace("\u200b", "");
            int length = Math.Min(text.Length, 20);
            return $"{Deep} - {text.Substring(0, length)}";
        }
    }
}