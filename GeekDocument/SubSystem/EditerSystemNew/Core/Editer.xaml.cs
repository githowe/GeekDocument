using GeekDocument.SubSystem.EditerSystemNew.Define;
using GeekDocument.SubSystem.ImageSystem;
using GeekDocument.SubSystem.LayoutEngine;
using GeekDocument.SubSystem.LayoutEngine.Element;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Page = GeekDocument.SubSystem.LayoutEngine.Page;

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

        }

        /// <summary>
        /// 处理按键按下
        /// </summary>
        public void HandleKeyDown(KeyEventArgs e)
        {

        }

        /// <summary>
        /// 处理按键松开
        /// </summary>
        public void HandleKeyUp(KeyEventArgs e)
        {

        }

        /// <summary>
        /// 处理文本输入
        /// </summary>
        public void HandleTextInput(string text)
        {

        }

        #endregion

        #region 核心接口

        public void LoadDocument(Document document)
        {
            AddTestData(document);
            // 创建页面控件并加载文档中的页面属性
            _page = new Page();
            MainGrid.Children.Add(_page);
            _page.Width = document.PageWidth;
            OptionSystem.PageThickness padding = document.Padding;
            _page.PagePadding = new Thickness(padding.Left, padding.Top, padding.Right, padding.Bottom);
            _page.BlockInterval = document.BlockInterval;
            _page.FirstLineIndent = document.FirstLineIndent;
            _page.TextFont = document.TextFont;
            _page.TextSize = document.TextSize;
            // 加载块
            foreach (var item in document.块列表)
            {
                if (item.根元素 is null) continue;
                块元素 element = new 块元素
                {
                    BlockMargin = new Thickness(item.Margin[0], item.Margin[1], item.Margin[2], item.Margin[3]),
                    根元素 = item.根元素
                };
                _page.BlockList.Add(element);
            }
            // 初始化页面
            _page.Init();
            // 更新页面
            _page.更新页面();
        }

        #endregion

        #region 私有方法

        private void AddTestData(Document document)
        {
            段落 段落元素 = new 段落
            {
                Text = File.ReadAllText("D:/示例文档3.txt"),
            };

            块 段落块 = new 块
            {
                类型 = 块类型.段落,
                根元素 = 段落元素,
            };
            document.块列表.Add(段落块);

            图片 图片元素 = 创建图片元素("C:\\Users\\12460\\Desktop\\一一五\\像素艺术\\01f0d15b7847b1a801218d3218024a.gif", 320, true);
            图片元素.Caption = null;
            段落元素.内嵌元素列表.Add(new 段落内嵌元素
            {
                LineIndex = 2,
                CharIndex = 20,
                ElementList = new List<布局元素> { 图片元素 }
            });

            图片元素 = 创建图片元素("C:\\Users\\12460\\Desktop\\一一五\\像素艺术\\235855420_org.v1461646057.gif", pixelArt: true);
            // 图片元素.Caption = "臭臭泥";
            图片元素.Caption = null;
            段落元素.内嵌元素列表.Add(new 段落内嵌元素
            {
                LineIndex = 5,
                CharIndex = 16,
                ElementList = new List<布局元素> { 图片元素 }
            });

            图片元素 = 创建图片元素("C:\\Users\\12460\\Desktop\\一一五\\像素艺术\\0114005b7847b1a80120d8c0319ff7.gif", 180, true);
            图片元素.CaptionMaxWidthType = 图注最大宽度.指定宽度;
            图片元素.CaptionMaxWidth = 0;
            块 图片块 = new 块
            {
                类型 = 块类型.图片,
                根元素 = 图片元素,
            };
            // document.块列表.Add(图片块);

            表格 表格元素 = new 表格
            {
                行数 = 5,
                列数 = 5,
                水平对齐 = 水平对齐方式.Center
            };
            表格元素.Init();
            块 表格块 = new 块
            {
                类型 = 块类型.表格,
                根元素 = 表格元素,
            };
            document.块列表.Add(表格块);
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

            /*段落元素.内嵌元素列表.Add(new 段落内嵌元素
            {
                LineIndex = 4,
                CharIndex = 2,
                ElementList = new List<布局元素> { 表格元素 }
            });*/
            表格元素.设置行高(3, 24);
            表格元素.设置列宽(0, 72);
            表格元素.设置列宽(3, 72);
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

        #endregion
    }
}