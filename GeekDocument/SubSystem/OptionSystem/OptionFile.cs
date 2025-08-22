using System.IO;

namespace GeekDocument.SubSystem.OptionSystem
{
    /// <summary>
    /// 选项文件
    /// </summary>
    public class OptionFile
    {
        /// <summary>文档库列表</summary>
        public List<DocumentLib> LibPathList { get; set; } = new List<DocumentLib>();

        /// <summary>默认路径索引</summary>
        public int DefaultPathIndex { get; set; } = 0;

        /// <summary>页面宽度</summary>
        public int PageWidth { get; set; } = 800;

        /// <summary>页边距</summary>
        public string PagePadding { get; set; } = "64,64,64,64";

        /// <summary>首行缩进。单位：像素</summary>
        public int FirstLineIndent { get; set; } = 32;

        /// <summary>段间距</summary>
        public int ParagraphInterval { get; set; } = 16;

        /// <summary>显示边距标记</summary>
        public bool ShowPaddingMark { get; set; } = true;

        /// <summary>显示行线</summary>
        public bool ShowRowLine { get; set; } = false;

        public void Init()
        {
            // 创建默认文档库
            string defaultLibPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\GeekDocument\\DocLib\\";
            if (!Directory.Exists(defaultLibPath)) Directory.CreateDirectory(defaultLibPath);
            // 添加默认文档库路径
            DocumentLib documentLib = new DocumentLib
            {
                Name = "极客文档",
                Path = defaultLibPath
            };
            LibPathList.Add(documentLib);
        }
    }
}