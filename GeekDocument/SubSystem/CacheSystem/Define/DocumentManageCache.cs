using Newtonsoft.Json;
using System.Globalization;

namespace GeekDocument.SubSystem.CacheSystem.Define
{
    /// <summary>
    /// 最近文档
    /// </summary>
    public class RecentDocument : IComparable<RecentDocument>
    {
        /// <summary>完整路径</summary>
        public string FullPath { get; set; } = "";

        /// <summary>编辑时间</summary>
        public string EditTime { get; set; } = "";

        /// <summary>已标记</summary>
        public bool Marked { get; set; } = false;

        public int CompareTo(RecentDocument? other)
        {
            if (other == null) return 0;
            DateTime selfTime = DateTime.ParseExact(EditTime, DocumentManageCache.TimeFormat, CultureInfo.InvariantCulture);
            DateTime otherTime = DateTime.ParseExact(other.EditTime, DocumentManageCache.TimeFormat, CultureInfo.InvariantCulture);
            return selfTime.CompareTo(otherTime);
        }
    }

    /// <summary>
    /// 文档管理缓存
    /// </summary>
    public class DocumentManageCache
    {
        /// <summary>最近文档列表</summary>
        public List<RecentDocument> RecentDocumentList { get; set; } = new List<RecentDocument>();

        /// <summary>最大最近文档数量</summary>
        [JsonIgnore]
        public int RecentDocumentMax { get; set; } = 50;

        /// <summary>时间格式</summary>
        [JsonIgnore]
        public static string TimeFormat => "yyyy-MM-dd HH:mm";

        /// <summary>
        /// 获取最近文档
        /// </summary>
        public RecentDocument? GetRecentDocument()
        {
            // 因为最近打开的会放在列表的最后，所以返回最后一个
            return RecentDocumentList.Count > 0 ? RecentDocumentList.Last() : null;
        }

        /// <summary>
        /// 获取排序后的最近文档列表
        /// </summary>
        public List<RecentDocument> GetSortedList()
        {
            // 已标记的列表
            List<RecentDocument> markedList = new List<RecentDocument>();
            // 未标记的列表
            List<RecentDocument> unmarkList = new List<RecentDocument>();
            // 分类
            foreach (var item in RecentDocumentList)
            {
                if (item.Marked) markedList.Add(item);
                else unmarkList.Add(item);
            }
            // 排序：按时间顺序
            markedList.Sort();
            unmarkList.Sort();
            // 反转顺序：确保时间最近的在前
            markedList.Reverse();
            unmarkList.Reverse();
            // 合并两个列表：标记列表在前
            markedList.AddRange(unmarkList);
            // 返回合并后的结果
            return markedList;
        }

        public void AddRecentDocument(string path)
        {
            // 当前时间
            string time = DateTime.Now.ToString(TimeFormat);
            // 查找最近文档
            RecentDocument? recnet = FindRecentDocument(path);
            // 已存在此文档
            if (recnet != null)
            {
                // 从列表中移除
                RecentDocumentList.Remove(recnet);
                // 更新时间
                recnet.EditTime = time;
                // 插入至最后
                RecentDocumentList.Add(recnet);
            }
            // 不存在此文档
            else
            {
                // 新建最近文档
                recnet = new RecentDocument
                {
                    FullPath = path,
                    EditTime = time,
                };
                // 如果列表已满，则删除第一个
                if (RecentDocumentList.Count >= RecentDocumentMax) RecentDocumentList.RemoveAt(0);
                // 添加文档
                RecentDocumentList.Add(recnet);
            }
        }

        /// <summary>
        /// 移除最近文档
        /// </summary>
        public void RemoveRecentDocument(string path)
        {
            // 查找最近文档
            RecentDocument? recnet = FindRecentDocument(path);
            // 移除
            if (recnet != null) RecentDocumentList.Remove(recnet);
        }

        /// <summary>
        /// 查找最近文档
        /// </summary>
        private RecentDocument? FindRecentDocument(string path)
        {
            foreach (var item in RecentDocumentList)
                if (item.FullPath == path) return item;
            return null;
        }
    }
}