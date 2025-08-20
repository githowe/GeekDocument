using GeekDocument.SubSystem.CacheSystem.Define;
using Newtonsoft.Json;
using System.IO;

namespace GeekDocument.SubSystem.CacheSystem
{
    /// <summary>
    /// 缓存数据
    /// </summary>
    public class CacheData
    {
        public ApplicationCache Application { get; set; } = new ApplicationCache();

        public MainWindowCache MainWindow { get; set; } = new MainWindowCache();

        public DocumentManageCache DocumentManager { get; set; } = new DocumentManageCache();

        public DocumentLibCache DocumentLib { get; set; } = new DocumentLibCache();
    }

    public class CacheManager
    {
        #region 单例

        private CacheManager() { }
        public static CacheManager Instance { get; } = new CacheManager();

        #endregion

        public CacheData Cache { get; set; } = new CacheData();

        public void Init()
        {
            _cacheFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\GeekDocument\\Cache.json";
            if (!File.Exists(_cacheFilePath)) SaveCache();
            LoadCacheFile();
        }

        public void SaveCache()
        {

            File.WriteAllText(_cacheFilePath, JsonConvert.SerializeObject(Cache, Formatting.Indented));
        }

        private void LoadCacheFile()
        {
            string jsonData = File.ReadAllText(_cacheFilePath);
            CacheData? cacheData = JsonConvert.DeserializeObject<CacheData>(jsonData);
            if (cacheData != null) Cache = cacheData;
        }

        /// <summary>缓存文件名</summary>
        private string _cacheFilePath = "";
    }
}