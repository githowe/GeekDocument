namespace GeekDocument.SubSystem.CacheSystem.Define
{
    public class FileManagerCache
    {
        public string RecentImagePath { get; set; } = "";

        public int RecentImageType { get; set; } = 0;

        public string RecentExportPath { get; set; } = "";

        public int RecentExportType { get; set; } = 0;
    }
}