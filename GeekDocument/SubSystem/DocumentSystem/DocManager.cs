using GeekDocument.SubSystem.CacheSystem;
using GeekDocument.SubSystem.CacheSystem.Define;
using GeekDocument.SubSystem.EditerSystem.Define;
using GeekDocument.SubSystem.OptionSystem;
using System.IO;
using XLogic.Base;

namespace GeekDocument.SubSystem.DocumentSystem
{
    /// <summary>
    /// 文档管理器
    /// </summary>
    public class DocManager
    {
        #region 单例

        private DocManager() { }
        public static DocManager Instance { get; } = new DocManager();

        #endregion

        #region 公开方法

        public string GetRecentDocumentPath()
        {
            // 获取最近编辑过的文档
            RecentDocument? recentDoc = CacheManager.Instance.Cache.DocumentManager.GetRecentDocument();
            // 没有最近文档，则返回默认文档库路径
            if (recentDoc == null) return Options.Instance.System.DefaultPath;
            // 返回最近文档的完整路径
            string? folderPath = Path.GetDirectoryName(recentDoc.FullPath);
            return folderPath ?? Options.Instance.System.DefaultPath;
        }

        #endregion

        #region 私有方法

        #endregion

        #region 字段

        private List<Document> _documentList = new List<Document>();

        #endregion
    }
}