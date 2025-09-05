namespace GeekDocument.SubSystem.DirectWriteSystem
{
    public class DWriteTool
    {
        private DWriteTool() { }
        public static DWriteTool Instance { get; } = new DWriteTool();

        public void Init()
        {
            _tool = Interop.CreateDWriteTool();
        }

        public FontMetrics GetFontMetrics(string fontFamilyName, bool bold, bool italic)
        {
            return Interop.GetFontMetrics(_tool, fontFamilyName, bold, italic);
        }

        public void Clear()
        {
            Interop.ReleaseDWriteTool(_tool);
        }

        private nint _tool = nint.Zero;
    }
}