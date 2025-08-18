namespace GeekDocument.SubSystem.OptionSystem
{
    /// <summary>
    /// 页边线
    /// </summary>
    public class PageThickness
    {
        public PageThickness() { }

        public PageThickness(int margin)
        {
            Left = margin;
            Right = margin;
            Top = margin;
            Bottom = margin;
        }

        public PageThickness(string margin)
        {
            string[] array = margin.Split(',');
            if (array.Length != 4) return;
            Left = int.Parse(array[0]);
            Right = int.Parse(array[1]);
            Top = int.Parse(array[2]);
            Bottom = int.Parse(array[3]);
        }

        public int Left { get; set; } = 32;

        public int Right { get; set; } = 32;

        public int Top { get; set; } = 32;

        public int Bottom { get; set; } = 32;

        /// <summary>横向线宽</summary>
        public int Horizontal => Left + Right;

        /// <summary>纵向线宽</summary>
        public int Vertical => Top + Bottom;

        public override string ToString() => $"{Left},{Right},{Top},{Bottom}";
    }
}