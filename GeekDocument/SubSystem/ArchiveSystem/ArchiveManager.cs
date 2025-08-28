using GeekDocument.SubSystem.ArchiveSystem.Define;
using GeekDocument.SubSystem.EditerSystem.Define;
using GeekDocument.SubSystem.EditerSystem.Define.BlockDerive;
using GeekDocument.SubSystem.ImageSystem;
using GeekDocument.SubSystem.WindowSystem;
using Newtonsoft.Json;

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
            {
                BlockInfo blockInfo = new BlockInfo
                {
                    Type = block.Type.ToString(),
                    SourceData = block.ToJson()
                };
                archiveFile.BlockData.DataList.Add(JsonConvert.SerializeObject(blockInfo));
            }
            // 设置页面信息
            archiveFile.PageData.PageWidth = document.PageWidth.ToString();
            archiveFile.PageData.Padding = document.Padding.ToString();
            archiveFile.PageData.FirstLineIndent = document.FirstLineIndent;
            archiveFile.PageData.ParagraphInterval = document.ParagraphInterval;
            // 设置资源数据
            {
                // 资源大小
                int resourceSize = 0;
                // 收集文档中的资源
                List<DocumentRes> 资源列表 = 收集文档中的资源(document);
                // 设置资源列表
                foreach (var 资源 in 资源列表)
                {
                    ResInfo resInfo = new ResInfo
                    {
                        Hash = 资源.Hash,
                        ResType = 资源.ResType,
                        ResSize = 资源.SourceData.Length
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
                    byte[] resData = 查找源数据(资源列表, resInfo.Hash);
                    Array.Copy(resData, 0, archiveFile.ResData, offset, resInfo.ResSize);
                    offset += resInfo.ResSize;
                }
            }
            return archiveFile.ToByteData();
        }

        #endregion

        #region 私有方法

        private static List<DocumentRes> 收集文档中的资源(Document document)
        {
            // 图片块中引用了图片的哈希值
            // 通过此哈希值查找图片的源数据

            List<DocumentRes> 资源列表 = new List<DocumentRes>();
            foreach (var block in document.BlockList)
            {
                // 图片块
                if (block.Type == BlockType.Image)
                {
                    BlockImage blockImage = (BlockImage)block;
                    // 查找图片块对应的图片文件数据
                    ImageFileData? fileData = ImageManager.Instance.FindFileData(blockImage.SourceHash);
                    if (fileData == null)
                    {
                        WM.ShowErrorTip($"查找文件数据失败。哈希值：{blockImage.SourceHash}");
                        continue;
                    }
                    // 创建资源并添加
                    DocumentRes res = new DocumentRes
                    {
                        Hash = blockImage.SourceHash,
                        ResType = fileData.Type,
                        SourceData = fileData.Data,
                    };
                    资源列表.Add(res);
                }
            }
            return 资源列表;
        }

        private static byte[] 查找源数据(List<DocumentRes> resList, string hash)
        {
            foreach (var res in resList)
                if (res.Hash == hash) return res.SourceData;
            throw new Exception($"查找源数据失败。哈希值：{hash}");
        }

        #endregion
    }
}