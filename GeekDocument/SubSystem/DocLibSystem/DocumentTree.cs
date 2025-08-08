using GeekDocument.SubSystem.OptionSystem;
using GeekDocument.SubSystem.ResourceSystem;
using System.Collections;
using System.IO;
using XLogic.Base;
using XLogic.WpfControl;

namespace GeekDocument.SubSystem.DocLibSystem
{
    /// <summary>
    /// 文件信息比较器
    /// </summary>
    public class FileInfoComparer : IComparer<FileInfo>
    {
        public int Compare(FileInfo? x, FileInfo? y)
        {
            if (x == null || y == null) return 0;
            string xName = Path.GetFileNameWithoutExtension(x.FullName);
            string yName = Path.GetFileNameWithoutExtension(y.FullName);
            return _comparer.Compare(xName, yName);
        }

        /// <summary>自然比较器</summary>
        private readonly NaturalComparer _comparer = new NaturalComparer();
    }

    /// <summary>
    /// 文档树。用于加载文档库
    /// </summary>
    public class DocumentTree
    {
        #region 单例

        private DocumentTree() { }
        public static DocumentTree Instance { get; } = new DocumentTree();

        #endregion

        #region 属性

        public TreeItem Root { get; set; } = new TreeItem { Text = "文档库" };

        #endregion

        #region 公开方法

        public void Init()
        {
            LoadDocumentLib();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Rrfresh()
        {
            Root.ItemList.Clear();
            LoadDocumentLib();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 加载文档库
        /// </summary>
        private void LoadDocumentLib()
        {
            // 遍历文档库
            foreach (var lib in Options.Instance.System.LibList)
            {
                TreeItem libRoot = new TreeItem
                {
                    Parent = Root,
                    Icon = ImageResManager.Instance.GetIcon15("Lib.png"),
                    Text = lib.Name,
                    CanExpand = true
                };
                Root.ItemList.Add(libRoot);
                LoadSubFolder(libRoot, lib.Path);
            }
        }

        private void LoadSubFolder(TreeItem parent, string folderPath)
        {
            // 遍历子文件夹
            foreach (DirectoryInfo childFolder in GetFolderList(folderPath))
            {
                // 创建文件夹项
                TreeItem folderItem = new TreeItem
                {
                    Parent = parent,
                    Icon = ImageResManager.Instance.GetIcon15("Folder.png"),
                    Text = childFolder.Name,
                    CanExpand = true
                };
                parent.ItemList.Add(folderItem);
                // 递归加载子文件夹
                LoadSubFolder(folderItem, childFolder.FullName);
                if (folderItem.ItemList.Count == 0)
                    folderItem.Icon = ImageResManager.Instance.GetIcon15("EmptyFolder.png");
            }
            // 遍历文件
            foreach (FileInfo file in GetFileList(folderPath))
            {
                if (Path.GetExtension(file.FullName) != ".gdoc") continue;
                // 创建文件项
                TreeItem fileItem = new TreeItem
                {
                    Parent = parent,
                    Icon = ImageResManager.Instance.GetIcon15("Document.png"),
                    Text = Path.GetFileNameWithoutExtension(file.FullName),
                    CanExpand = false
                };
                parent.ItemList.Add(fileItem);
            }
        }

        private ArrayList GetFolderList(string folderPath)
        {
            ArrayList folderList = new ArrayList(new DirectoryInfo(folderPath).GetDirectories());
            folderList.Sort(_comparer);
            return folderList;
        }

        private List<FileInfo> GetFileList(string folderPath)
        {
            List<FileInfo> fileList = new DirectoryInfo(folderPath).GetFiles().ToList();
            fileList.Sort(_fileInfoComparer);
            return fileList;
        }

        #endregion

        /// <summary>自然比较器。用于比较文件夹名</summary>
        private readonly NaturalComparer _comparer = new NaturalComparer();
        /// <summary>文件名比较器。因为文件有扩展名，直接用自然比较器会将扩展名当成名称的一部分</summary>
        private readonly FileInfoComparer _fileInfoComparer = new FileInfoComparer();
    }
}