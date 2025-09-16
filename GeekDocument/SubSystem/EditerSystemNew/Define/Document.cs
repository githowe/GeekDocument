using GeekDocument.SubSystem.ArchiveSystem.Define;
using GeekDocument.SubSystem.OptionSystem;

namespace GeekDocument.SubSystem.EditerSystemNew.Define
{
    public class Document
    {
        #region 元数据

        /// <summary>作者</summary>
        public string Author { get; set; } = "";

        /// <summary>简介</summary>
        public string Summary { get; set; } = "";

        /// <summary>创建日期</summary>
        public DateTime Create { get; set; } = DateTime.Now;

        /// <summary>备注</summary>
        public string Note { get; set; } = "";

        /// <summary>标签</summary>
        public List<string> TagList { get; set; } = new List<string>();

        #endregion

        #region 视图属性

        /// <summary>页面宽度</summary>
        public int PageWidth { get; set; } = 928;

        /// <summary>内边距</summary>
        public PageThickness Padding { get; set; } = new PageThickness();

        /// <summary>块间距</summary>
        public int BlockInterval { get; set; } = 16;

        /// <summary>首行缩进</summary>
        public int FirstLineIndent { get; set; } = 32;

        /// <summary>默认正文字体</summary>
        public string TextFont { get; set; } = "霞鹜文楷";

        /// <summary>默认正文字号。单位：像素</summary>
        public int TextSize { get; set; } = 16;

        #endregion

        public List<块> 块列表 { get; set; } = new List<块>();

        public void LoadArchive(ArchiveFile archiveFile)
        {

        }

        private void 加载块列表(List<string> blockDataList)
        {

        }
    }
}