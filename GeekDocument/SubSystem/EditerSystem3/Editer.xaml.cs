using GeekDocument.SubSystem.ArchiveSystem2;
using GeekDocument.SubSystem.FileSystem;
using GeekDocument.SubSystem.ImageSystem;
using GeekDocument.SubSystem.LayoutEngine;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XLogic.Base.Ex;

namespace GeekDocument.SubSystem.EditerSystem3
{
    public partial class Editer : UserControl
    {
        #region 构造方法

        public Editer() => InitializeComponent();

        #endregion

        #region 属性、事件

        /// <summary>文档名称</summary>
        public string DocumentName { get; set; } = "未命名文档";

        /// <summary>文档路径</summary>
        public string DocumentPath { get; set; } = "";

        /// <summary>已保存</summary>
        public bool Saved
        {
            get => _saved;
            set
            {
                if (_saved == value) return;
                _saved = value;
                SaveStateChanged?.Invoke(this);
            }
        }

        public event Action<Editer> SaveStateChanged;

        #endregion

        #region 公开方法

        public void Init()
        {
            Panel_LayoutTree.Init();

            Tool_OpenLeft.Click += Tool_OpenLeft_Click;
            Tool_CloseLeft.Click += Tool_CloseLeft_Click;
            Panel_LayoutTree.HoverElementChanged += Panel_LayoutTree_HoverElementChanged;
        }

        public void LoadDocument(文档 文档)
        {
            _doc = 文档;
            页面 page = _doc.页面;
            _pageView = new PageView { OwnerEditer = this };
            PaperBox.Children.Add(_pageView);
            // 页面宽度 = 内容宽度 + 左边距 + 右边距
            _pageView.Width = page.页宽 + page.内边距.Left + page.内边距.Right;
            _pageView.PagePadding = page.内边距;
            _pageView.Init(page);
            _pageView.更新页面();
            _pageView.InitEditSystem();
            // 加载元素结构
            Panel_LayoutTree.LoadLayoutTree(page);
        }

        /// <summary>
        /// 处理按键按下
        /// </summary>
        public void HandleKeyDown(KeyEventArgs e)
        {
            _pageView?.HandleKeyDown(e);
        }

        /// <summary>
        /// 处理按键松开
        /// </summary>
        public void HandleKeyUp(KeyEventArgs e) { }

        /// <summary>
        /// 处理文本输入
        /// </summary>
        public void HandleTextInput(string text)
        {
            _pageView?.HandleTextInput(text);
        }

        #endregion

        #region 控件事件

        private void Tool_OpenLeft_Click(object sender, RoutedEventArgs e)
        {
            LeftArea.Width = new GridLength(320);
            Tool_OpenLeft.Visibility = Visibility.Collapsed;
            Tool_CloseLeft.Visibility = Visibility.Visible;
        }

        private void Tool_CloseLeft_Click(object sender, RoutedEventArgs e)
        {
            LeftArea.Width = new GridLength(0);
            Tool_OpenLeft.Visibility = Visibility.Visible;
            Tool_CloseLeft.Visibility = Visibility.Collapsed;
        }

        private void Panel_LayoutTree_HoverElementChanged(布局元素? 元素)
        {
            _pageView.UpdateHoveredElement(元素);
        }

        private void Tool_Save_Click(object sender, RoutedEventArgs e)
        {
            // 先备份文件
            string backPath = BackupFile(DocumentPath);
            try
            {
                byte[] byteData = 存档管理器.Instance.生成存档数据(_doc);
                // 打开文件
                FileStream fileStream = new FileStream(DocumentPath, FileMode.Create);
                // 写入存档数据并关闭
                fileStream.Write(byteData, 0, byteData.Length);
                fileStream.Close();
            }
            catch (Exception)
            {
                // 出现异常时，恢复文件
                File.Copy(backPath, DocumentPath, true);
            }
        }

        private void Tool_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            // string jsonData = File.ReadAllText("D:/示例存档.json");
            // 文档数据 文档 = JsonConvert.DeserializeObject<文档数据>(jsonData)!;
        }

        private void Tool_Image_Click(object sender, RoutedEventArgs e)
        {
            // 选择图片
            List<string> pathList = FM.Instance.OpenReadImageDialog("插入图片");
            if (pathList.Count == 0) return;
            // 遍历选择的图片列表
            List<图片> 图片列表 = new List<图片>();
            foreach (var imagePath in pathList)
            {
                // 获取图片文件数据
                ImageFileData fileData = ImageManager.Instance.GetImageFileData(imagePath);
                // 加载图片
                string hash = LoadImage(fileData);
                // 创建图片元素
                图片 图片 = new 图片 { SourceHash = hash };
                图片.Init();
                图片列表.Add(图片);
            }
            // 插入图片
            _pageView.插入图片(图片列表);
        }

        private void Tool_Table_Click(object sender, RoutedEventArgs e)
        {
            InsertTableDialog dialog = new InsertTableDialog();
            if (dialog.ShowDialog() == true)
            {
                表格 表格 = new 表格
                {
                    行数 = dialog.行数,
                    列数 = dialog.列数,
                    单元格宽度 = dialog.单元格宽度,
                    单元格高度 = dialog.单元格高度
                };
                表格.Init();
                _pageView.插入表格(表格);
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 备份文件
        /// </summary>
        private string BackupFile(string filePath)
        {
            // 获取系统临时文件夹路径
            string tempPath = Path.GetTempPath();
            // 随机一个文件名
            string guid = Guid.NewGuid().ToString();
            // 备份路径
            string backupPath = Path.Combine(tempPath, guid + ".gdocbak");
            // 复制文件
            File.Copy(filePath, backupPath, true);
            // 返回备份路径
            return backupPath;
        }

        /// <summary>
        /// 加载图片
        /// </summary>
        private string LoadImage(ImageFileData fileData)
        {
            // 获取图片信息，获取成功则直接返回
            ImageInfo? imageInfo = ImageManager.Instance.FindImageInfo(fileData.Hash);
            if (imageInfo != null) return fileData.Hash;
            // 加载图片
            imageInfo = ImageLoader.Instance.LoadImageFile(fileData.Data, fileData.Type);
            // 加载失败时返回空
            if (imageInfo == null) return "";
            // 缓存文件数据和解码结果
            ImageManager.Instance.AddFileData(fileData);
            ImageManager.Instance.AddImageInfo(fileData.Hash, imageInfo);
            // 返回图片哈希
            return fileData.Hash;
        }

        #endregion

        #region 字段

        private 文档 _doc = null!;
        private PageView _pageView = null!;
        private bool _saved = true;

        #endregion
    }
}