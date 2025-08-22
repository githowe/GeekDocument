using GeekDocument.SubSystem.ArchiveSystem;
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
                FirstLineIndent = Options.Instance.Paragraph.FirstLineIndent,
                ParagraphInterval = Options.Instance.Paragraph.ParagraphInterval
            };
            // 读取文本文件
            foreach (var line in File.ReadAllLines("D:/示例文档3.txt"))
            {
                // 每行创建一个文本块
                BlockText blockText = new BlockText
                {
                    Content = line,
                    FirstLineIndent = document.FirstLineIndent,
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
            // 加载文档选项
            LoadDocumentOption();
            // 加载块
            GetComponent<PageComponent>().LoadBlock(document.BlockList);
        }

        /// <summary>
        /// 加载文档选项
        /// </summary>
        public void LoadDocumentOption()
        {
            // 设置文档区域宽度
            _host.DocArea.Width = Document.PageWidth + Document.Padding.Left + Document.Padding.Right;
            // 更新布局以确保区域内控件大小的正确性
            _host.DocArea.UpdateLayout();
            // 设置页宽（包含内边距）、内边距、首行缩进、段间距
            PageComponent pageComponent = GetComponent<PageComponent>();
            pageComponent.PageWidth = (int)_host.DocArea.Width;
            pageComponent.Padding = Document.Padding;
            pageComponent.FirstLineIndent = Document.FirstLineIndent;
            pageComponent.ParagraphInterval = Document.ParagraphInterval;
        }

        /// <summary>
        /// 保存文档
        /// </summary>
        public void SaveDocument()
        {
            // 更新块列表
            Document.BlockList = GetComponent<PageComponent>().GetBlockList();
            // 打开文件
            FileStream fileStream = File.OpenWrite(_host.DocumentPath);
            // 生成存档数据
            byte[] archiveData = ArchiveManager.Instance.GenerateArchiveData(Document);
            // 写入存档数据并关闭
            fileStream.Write(archiveData, 0, archiveData.Length);
            fileStream.Close();
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