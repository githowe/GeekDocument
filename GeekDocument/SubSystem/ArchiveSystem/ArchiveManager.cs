using GeekDocument.SubSystem.ArchiveSystem.Define;
using GeekDocument.SubSystem.EditerSystem.Define;

namespace GeekDocument.SubSystem.ArchiveSystem
{
    public class ArchiveManager
    {
        #region 单例

        private ArchiveManager() { }
        public static ArchiveManager Instance { get; } = new ArchiveManager();

        #endregion

        #region 属性

        #endregion

        #region 公开方法

        /// <summary>
        /// 生成存档数据
        /// </summary>
        public byte[] GenerateArchiveData(Document document)
        {
            ArchiveFile archiveFile = new ArchiveFile();
            // 设置元数据
            archiveFile.MetaData.Author = document.Author;
            archiveFile.MetaData.Summary = document.Summary;
            archiveFile.MetaData.Create = document.Create.ToString("yyyy.MM.dd");
            archiveFile.MetaData.Note = document.Note;
            archiveFile.MetaData.Tag = string.Join(",", document.TagList);
            // 设置块数据
            foreach (var block in document.BlockList)
                archiveFile.BlockData.DataList.Add(block.ToJson());
            // 设置页面信息
            archiveFile.PageData.PageWidth = document.PageWidth.ToString();
            archiveFile.PageData.PageMargin = document.PageMargin.ToString();
            // 设置资源数据
            {
                // 资源大小
                int resourceSize = 0;
                // 设置资源列表
                foreach (var pair in document.ResourceDict)
                {
                    ResInfo resInfo = new ResInfo
                    {
                        ResID = pair.Key,
                        ResType = pair.Value.ResType,
                        ResSize = pair.Value.SourceData.Length
                    };
                    archiveFile.ResList.List.Add(resInfo);
                    // 更新资源大小
                    resourceSize += resInfo.ResSize;
                }
                // 设置资源数据
                archiveFile.ResData = new byte[resourceSize];
                int offset = 0;
                foreach (var resInfo in archiveFile.ResList.List)
                {
                    byte[] resData = document.ResourceDict[resInfo.ResID].SourceData;
                    Array.Copy(resData, 0, archiveFile.ResData, offset, resInfo.ResSize);
                    offset += resInfo.ResSize;
                }
            }
            return archiveFile.ToByteData();
        }

        #endregion

        #region 私有方法

        #endregion
    }
}