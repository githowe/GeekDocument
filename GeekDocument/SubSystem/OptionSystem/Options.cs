using Newtonsoft.Json;
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
        public string PageMargin { get; set; } = "32,32,32,32";

        /// <summary>块间距</summary>
        public int BlockInterval { get; set; } = 16;

        /// <summary>显示行线</summary>
        public bool ShowRowLine { get; set; } = false;

        /// <summary>显示段落标记</summary>
        public bool ShowParagraphMark { get; set; } = false;

        /// <summary>首行缩进。单位：像素</summary>
        public int FirstLineIndent { get; set; } = 32;

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

    /// <summary>
    /// 选项管理器
    /// </summary>
    public class Options
    {
        #region 单例

        private Options() { }
        public static Options Instance { get; } = new Options();

        #endregion

        public SystemOption System { get; set; } = new SystemOption();

        public PageOption Page { get; set; } = new PageOption();

        public ViewOption View { get; set; } = new ViewOption();

        public ParagraphOption Paragraph { get; set; } = new ParagraphOption();

        public void Init()
        {
            // 选项文件路径
            _optionFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\GeekDocument\\Options.json";
            // 不存在则新建选项
            if (!File.Exists(_optionFilePath)) NewOption();
            // 加载选项
            LoadOption();
        }

        /// <summary>
        /// 新建选项
        /// </summary>
        private void NewOption()
        {
            OptionFile file = new OptionFile();
            file.Init();
            File.WriteAllText(_optionFilePath, JsonConvert.SerializeObject(file, Formatting.Indented));
        }

        /// <summary>
        /// 加载选项
        /// </summary>
        private void LoadOption()
        {
            // 读取选项文件并反序列化
            string jsonData = File.ReadAllText(_optionFilePath);
            OptionFile? optionFile = JsonConvert.DeserializeObject<OptionFile>(jsonData);
            if (optionFile == null) throw new Exception("加载选项文件失败");

            // 加载选项
            System.LibList.AddRange(optionFile.LibPathList);
            System.DefaultPathIndex = optionFile.DefaultPathIndex;
            Page.PageWidth = optionFile.PageWidth;
            string[] margin = optionFile.PageMargin.Split(',');
            Page.PageMargin = new PageMargin
            {
                Left = int.Parse(margin[0]),
                Top = int.Parse(margin[1]),
                Right = int.Parse(margin[2]),
                Bottom = int.Parse(margin[3])
            };
            Page.BlockInterval = optionFile.BlockInterval;
            View.ShowRowLine = optionFile.ShowRowLine;
            View.ShowParagraphMark = optionFile.ShowParagraphMark;
            Paragraph.FirstLineIndent = optionFile.FirstLineIndent;

            // 移除无效路径
            for (int index = System.LibList.Count - 1; index >= 0; index--)
            {
                if (!Directory.Exists(System.LibList[index].Path))
                    System.LibList.RemoveAt(index);
            }
            // 文档库路径为空时，添加默认路径
            if (System.LibList.Count == 0)
            {
                string defaultLibPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\GeekDocument\\DocLib\\";
                if (!Directory.Exists(defaultLibPath)) Directory.CreateDirectory(defaultLibPath);
                DocumentLib documentLib = new DocumentLib
                {
                    Name = "极客文档",
                    Path = defaultLibPath
                };
                System.LibList.Add(documentLib);
            }
            // 确保默认路径索引在有效范围内
            if (System.DefaultPathIndex < 0 || System.DefaultPathIndex > System.LibList.Count - 1)
                System.DefaultPathIndex = 0;
        }

        private string _optionFilePath = "";
    }
}