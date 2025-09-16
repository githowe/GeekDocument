using GeekDocument.SubSystem.EditerSystemNew.Define;
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
            段落元素.Init();

            块 段落块 = new 块
            {
                类型 = 块类型.段落,
                根元素 = 段落元素,
            };
            document.块列表.Add(段落块);

            图片 图片元素 = new 图片
            {
                水平对齐 = 水平对齐方式.Center,
            };
            图片元素.Init();
            块 图片块 = new 块
            {
                类型 = 块类型.图片,
                根元素 = 图片元素,
            };
            document.块列表.Add(图片块);
        }

        #endregion

        #region 字段

        private Page? _page = null;

        private bool _saved = true;

        #endregion
    }
}