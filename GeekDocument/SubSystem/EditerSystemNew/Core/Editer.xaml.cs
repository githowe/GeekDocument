using GeekDocument.SubSystem.EditerSystemNew.Define;
using GeekDocument.SubSystem.ImageSystem;
using GeekDocument.SubSystem.LayoutEngine;
using GeekDocument.SubSystem.LayoutEngine.Element;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Page = GeekDocument.SubSystem.EditerSystemNew.Control.Page;

namespace GeekDocument.SubSystem.EditerSystemNew.Core
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
            _split.Freeze();

            Panel_LayoutTree.Init();

            Tool_OpenLeft.Click += Tool_OpenLeft_Click;
            Tool_CloseLeft.Click += Tool_CloseLeft_Click;

            Block_ElementPath.Inlines.Clear();
            List<string> path = new List<string> { "段落", "段落", "段落", "段落", "段落", "段落", "段落", "段落", "段落", "段落" };
            for (int index = 0; index < path.Count; index++)
            {
                Block_ElementPath.Inlines.Add(new Run(path[index]));
                if (index < path.Count - 1)
                    Block_ElementPath.Inlines.Add(new Run(" > ") { Foreground = _split });
            }
        }

        /// <summary>
        /// 处理按键按下
        /// </summary>
        public void HandleKeyDown(KeyEventArgs e) => _page.HandleKeyDown(e);

        /// <summary>
        /// 处理按键松开
        /// </summary>
        public void HandleKeyUp(KeyEventArgs e) => _page.HandleKeyUp(e);

        /// <summary>
        /// 处理文本输入
        /// </summary>
        public void HandleTextInput(string text) => _page.HandleTextInput(text);

        #endregion

        #region 核心接口

        public void LoadDocument(Document document)
        {
            // InitDocumentData(document);
            AddTestData(document);
            // 创建页面控件并加载文档中的页面属性
            _page = new Page();
            PageBox.Children.Add(_page);
            _page.Width = document.PageWidth;
            OptionSystem.PageThickness padding = document.Padding;
            _page.PagePadding = new Thickness(padding.Left, padding.Top, padding.Right, padding.Bottom);
            _page.BlockInterval = document.BlockInterval;
            _page.FirstLineIndent = document.FirstLineIndent;
            _page.TextFont = document.TextFont;
            _page.TextSize = document.TextSize;
            // 加载段落
            foreach (var item in document.段落列表)
            {
                段落块 block = new 段落块(item);
                _page.BlockList.Add(block);
            }
            // 初始化页面
            _page.Init();
            // 更新页面
            _page.更新页面();

            InitEditSystem();
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

        #endregion

        #region 私有方法

        /// <summary>
        /// 初始化编辑系统
        /// </summary>
        private void InitEditSystem()
        {
            // 加载布局树
            Panel_LayoutTree.LoadLayoutTree(_page);
            // 监听事件
            Panel_LayoutTree.HoverElementChanged += Panel_LayoutTree_HoverElementChanged;

            _page.InitCaret();
        }

        private void Panel_LayoutTree_HoverElementChanged(IDocumentElement? element) => _page.UpdateHoverElementView(element);

        private void InitDocumentData(Document document)
        {
            段落 段落 = new 段落 { Text = "这是一个测试段落，用于测试编辑器的基本功能。", 水平对齐 = 水平对齐方式.Left };
            document.段落列表.Add(段落);
        }

        private void AddTestData(Document document)
        {
            // 创建第一个段落：文本、图片、表格
            段落 段落元素 = new 段落 { Text = File.ReadAllText("D:/示例文档3.txt"), 水平对齐 = 水平对齐方式.Left };
            // 嵌入图片至段落
            图片 图片元素 = 创建图片元素("C:\\Users\\12460\\Desktop\\一一五\\像素艺术\\01f0d15b7847b1a801218d3218024a.gif", 320, true);
            图片元素.Caption = null;
            段落元素.InsertLayoutElement(图片元素, 2, 20);
            图片元素 = 创建图片元素("C:\\Users\\12460\\Desktop\\一一五\\像素艺术\\01bd8c5e53ec3da801216518ea512d.png", 400, true);
            图片元素.Caption = null;
            段落元素.InsertLayoutElement(图片元素, 5, 8);
            表格 表格元素 = new 表格
            {
                行数 = 2,
                列数 = 2,
                单元格高度 = 24,
                单元格宽度 = 72,
            };
            表格元素.Init();
            表格元素.设置行高(1, 44);
            表格元素.设置单元格内容(0, 0, new 段落("一，一", 0));
            表格元素.设置单元格内容(1, 1, new 段落("二，\n二", 0));
            段落元素.InsertLayoutElement(表格元素, 6, 20);
            // 添加段落
            document.段落列表.Add(段落元素);

            // 创建第二个段落：仅图片
            段落元素 = new 段落() { 首行缩进 = 0, 水平对齐 = 水平对齐方式.Center };
            图片元素 = 创建图片元素("C:\\Users\\12460\\Desktop\\一一五\\像素艺术\\1_FuyupvvnXjN4ilD-e0BDCQ.gif", 278, pixelArt: true);
            图片元素.FontSize = 12;
            段落元素.AddLayoutElement(图片元素);
            图片元素 = 创建图片元素("C:\\Users\\12460\\Desktop\\一一五\\像素艺术\\tumblr_nw6jd8UKCY1rznluto2_250.gif", pixelArt: true);
            图片元素.FontSize = 12;
            图片元素.RightMargin = 10;
            段落元素.AddLayoutElement(图片元素);
            // document.段落列表.Add(段落元素);

            // 创建第三个段落：仅表格
            段落元素 = new 段落() { 首行缩进 = 0, 水平对齐 = 水平对齐方式.Center };
            document.段落列表.Add(段落元素);
            表格元素 = new 表格
            {
                行数 = 5,
                列数 = 5,
            };
            表格元素.Init();
            表格元素.设置行高(3, 24);
            表格元素.设置列宽(0, 72);
            表格元素.设置列宽(3, 72);
            段落元素.AddLayoutElement(表格元素);

            表格元素.设置单元格内容(1, 2, new 段落 { Text = File.ReadAllText("D:/示例文档4.txt") });
            表格元素.设置单元格内容(0, 1, new 段落 { Text = "一行，二列", 首行缩进 = 0 });
            表格元素.设置单元格内容(2, 1, new 段落 { Text = "三行，二列", 首行缩进 = 0 });
            表格元素.设置单元格内容(3, 4, new 段落 { Text = "四行，五列", 首行缩进 = 0 });

            图片 图标 = 创建图片元素("J:\\产品库\\设计库\\图标设计\\16\\企业.png");
            图标.Caption = null;
            表格元素.设置单元格内容(2, 3, 图标);

            图标 = 创建图片元素("J:\\产品库\\设计库\\图标设计\\16\\三维坐标彩色.png");
            图标.Caption = null;
            表格元素.设置单元格内容(4, 3, 图标);

            图标 = 创建图片元素("J:\\产品库\\设计库\\图标设计\\16\\工厂.png");
            图标.Caption = null;
            表格元素.设置单元格内容(3, 2, 图标);

            图标 = 创建图片元素("C:\\Users\\12460\\Desktop\\一一五\\像素艺术\\235855447_org.v1461646143.gif");
            图标.Caption = null;
            图标.MaxWidth = 128;
            图标.MaxHeight = 36;
            表格元素.设置单元格内容(4, 1, 图标);

            图标 = 创建图片元素("C:\\Users\\12460\\Desktop\\一一五\\像素艺术\\235855010_org.v1461644900.gif");
            图标.Caption = null;
            图标.MaxWidth = 128;
            图标.MaxHeight = 36;
            段落? 单元格段落 = 表格元素.获取单元格内容<段落>(2, 1);
            if (单元格段落 != null)
            {
                单元格段落.垂直对齐 = 垂直对齐方式.Top;
                单元格段落.内嵌元素列表.Add(new 段落内嵌元素
                {
                    CharIndex = 5,
                    ElementList = new List<布局元素> { 图标 }
                });
            }

            图标 = 创建图片元素("C:\\Users\\12460\\Desktop\\一一五\\像素艺术\\235854958_org.v1461644802.gif");
            图标.Caption = null;
            表格元素.设置单元格内容(0, 0, 图标);

            图标 = 创建图片元素("C:\\Users\\12460\\Desktop\\一一五\\像素艺术\\235855000_org.v1461644878.gif");
            图标.Caption = null;
            表格元素.设置单元格内容(1, 1, 图标);

            图标 = 创建图片元素("C:\\Users\\12460\\Desktop\\一一五\\像素艺术\\235854674_org.v1461644021.gif");
            图标.Caption = null;
            表格元素.设置单元格内容(1, 4, 图标);
        }

        private 图片 创建图片元素(string path, double maxWidth = double.NaN, bool pixelArt = false)
        {
            ImageFileData fileData = ImageManager.Instance.GetImageFileData(path);
            string hash = LoadImage(fileData);
            图片 element = new 图片
            {
                SourceHash = hash,
                水平对齐 = 水平对齐方式.Center,
                MaxWidth = maxWidth,
                PixelArt = pixelArt,
                Caption = Path.GetFileName(path),
            };
            element.Init();
            return element;
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

        private Page? _page = null;

        private bool _saved = true;

        private readonly Brush _split = new SolidColorBrush(Color.FromArgb(255, 249, 202, 124));

        #endregion
    }
}