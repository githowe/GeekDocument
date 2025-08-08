namespace GeekDocument.SubSystem.CacheSystem.Define
{
    public class DocumentLibCache
    {
        public List<string> ExpandedFolderList { get; set; } = new();

        /// <summary>
        /// 判断指定的文件夹是否处于展开状态
        /// </summary>
        public bool IsExpanded(string folderPath) => ExpandedFolderList.Contains(folderPath);

        /// <summary>
        /// 折叠指定文件夹
        /// </summary>
        public void Fold(string folderPath)
        {
            if (ExpandedFolderList.Contains(folderPath))
                ExpandedFolderList.Remove(folderPath);
            CacheManager.Instance.SaveCache();
        }

        /// <summary>
        /// 展开指定文件夹
        /// </summary>
        public void Expand(string folderPath)
        {
            if (!ExpandedFolderList.Contains(folderPath))
                ExpandedFolderList.Add(folderPath);
            CacheManager.Instance.SaveCache();
        }
    }
}