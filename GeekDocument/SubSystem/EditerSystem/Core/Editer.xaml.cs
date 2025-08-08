using GeekDocument.SubSystem.EditerSystem.Define;
using GeekDocument.SubSystem.OptionSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XLogic.Base.Ex;
using Page = GeekDocument.SubSystem.EditerSystem.Control.Page;

namespace GeekDocument.SubSystem.EditerSystem.Core
{
    public partial class Editer : UserControl
    {
        public Editer() => InitializeComponent();

        public Document Document { get; private set; }

        #region 公开方法

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            InitPage();
        }

        /// <summary>
        /// 加载文档
        /// </summary>
        public void LoadDocument()
        {
            List<string> fontList = new List<string>
            {
                "仿宋",
                "楷体",
                "新宋体",
                "微软雅黑",
                "霞鹜文楷",
            };
            List<Block> blockList = new List<Block>();
            // 读取文本文件
            foreach (var item in File.ReadAllLines("D:/示例文档2.txt"))
            {
                BlockText blockText = new BlockText
                {
                    Content = item,
                    // FontFamily = fontList.RandomElement(),
                };
                blockText.UpdateViewData();
                blockList.Add(blockText);
            }
            _page.LoadBlock(blockList);
        }

        /// <summary>
        /// 加载文档
        /// </summary>
        public void LoadDocument(Document document)
        {
            Document = document;
            _page.LoadBlock(document.BlockList);
        }

        #endregion

        #region 私有方法

        private void InitPage()
        {
            // 设置文档宽度
            var pageOptoin = Options.Instance.Page;
            int docWidth = pageOptoin.PageWidth + pageOptoin.PageMargin.Left + pageOptoin.PageMargin.Right;
            DocArea.Width = docWidth;
            // 新建页面添加至文档区域
            _page = new Page();
            PageBox.Children.Add(_page);
            _page.UpdateLayout();
            _page.Init();
        }

        #endregion

        #region 字段

        private Page _page;

        #endregion
    }
}