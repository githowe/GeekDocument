using GeekDocument.SubSystem.EditerSystem.Define;
using GeekDocument.SubSystem.OptionSystem;
using System.IO;
using XLogic.Base.UI;

namespace GeekDocument.SubSystem.EditerSystem.Core.Component
{
    /// <summary>
    /// 文档组件
    /// </summary>
    public class DocumentComponent : Component<Editer>
    {
        #region 属性

        /// <summary>文档实例</summary>
        public Document Document { get; private set; }

        #endregion

        #region 生命周期

        protected override void Init()
        {

        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 加载示例文档
        /// </summary>
        public void LoadDemoDocument()
        {
            // 获取系统选项
            PageOption pageOption = Options.Instance.Page;
            // 字体列表
            List<string> fontList = new List<string>
            {
                "仿宋",// 0
                "楷体",// 1
                "新宋体",// 2
                "微软雅黑",// 3
                "霞鹜文楷",// 4
            };
            // 块列表
            List<Block> blockList = new List<Block>();
            // 创建文档实例
            Document document = new Document
            {
                BlockList = blockList,
                PageWidth = pageOption.PageWidth,
                Padding = pageOption.PagePadding,
            };
            // 读取文本文件
            foreach (var line in File.ReadAllLines("D:/示例文档3.txt"))
            {
                // 每行创建一个文本块
                BlockText blockText = new BlockText
                {
                    Content = line,
                    LineSpace = 4,
                    // FontFamily = fontList.RandomElement(),
                    // FontFamily = fontList[4],
                    FontSize = 24
                };
                blockText.UpdateViewData(document.PageWidth);
                blockList.Add(blockText);
            }
            // 加载文档
            LoadDocument(document);
        }

        /// <summary>
        /// 加载文档
        /// </summary>
        public void LoadDocument(Document document)
        {
            // 设置文档实例
            Document = document;
            // 设置文档区域宽度
            _host.DocArea.Width = document.PageWidth + document.Padding.Left + document.Padding.Right;
            // 更新布局以确保区域内控件大小的正确性
            _host.DocArea.UpdateLayout();
            // 设置页宽（包含内边距）、内边距、首行缩进、段间距
            PageComponent pageComponent = GetComponent<PageComponent>();
            pageComponent.PageWidth = (int)_host.DocArea.Width;
            pageComponent.Padding = document.Padding;
            pageComponent.FirstLineIndent = document.FirstLineIndent;
            pageComponent.ParagraphInterval = document.ParagraphInterval;
            // 加载块
            pageComponent.LoadBlock(document.BlockList);
        }

        /// <summary>
        /// 保存文档
        /// </summary>
        public void SaveDocument()
        {

        }

        /// <summary>
        /// 卸载文档
        /// </summary>
        public void UnloadDocument()
        {

        }

        #endregion
    }
}