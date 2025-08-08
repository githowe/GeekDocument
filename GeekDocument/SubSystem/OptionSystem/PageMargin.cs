namespace GeekDocument.SubSystem.OptionSystem
{
    /// <summary>
    /// 页边距
    /// </summary>
    public class PageMargin
    {
        public PageMargin() { }

        public PageMargin(int margin)
        {
            Left = margin;
            Right = margin;
            Top = margin;
            Bottom = margin;
        }

        public PageMargin(string margin)
        {
            string[] array = margin.Split(',');
            if (array.Length != 4) return;
            Left = int.Parse(array[0]);
            Right = int.Parse(array[1]);
            Top = int.Parse(array[2]);
            Bottom = int.Parse(array[3]);
        }

        public int Left { get; set; } = 20;

        public int Right { get; set; } = 20;

        public int Top { get; set; } = 20;

        public int Bottom { get; set; } = 20;

        public override string ToString() => $"{Left},{Right},{Top},{Bottom}";
    }
}