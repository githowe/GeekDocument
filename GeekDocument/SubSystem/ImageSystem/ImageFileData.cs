namespace GeekDocument.SubSystem.ImageSystem
{
    public class ImageFileData
    {
        public string Hash { get; set; } = "";

        public string Type { get; set; } = "";

        public byte[] Data { get; set; } = Array.Empty<byte>();
    }
}