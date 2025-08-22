using Newtonsoft.Json;
using System.IO;

namespace GeekDocument.SubSystem.OptionSystem
{
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

        public ParagraphOption Paragraph { get; set; } = new ParagraphOption();

        public ViewOption View { get; set; } = new ViewOption();

        public void Init()
        {
            // 选项文件路径
            _optionFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\GeekDocument\\Options.json";
            // 不存在则新建选项
            if (!File.Exists(_optionFilePath)) NewOption();
            // 加载选项
            LoadOption();
        }

        public void Save()
        {
            OptionFile file = new OptionFile
            {
                LibPathList = System.LibList,
                DefaultPathIndex = System.DefaultPathIndex,
                PageWidth = Page.PageWidth,
                PagePadding = Page.PagePadding.ToString(),
                FirstLineIndent = Paragraph.FirstLineIndent,
                ParagraphInterval = Paragraph.ParagraphInterval,
                ShowPaddingMark = View.ShowPaddingMark,
                ShowRowLine = View.ShowRowLine
            };
            File.WriteAllText(_optionFilePath, JsonConvert.SerializeObject(file, Formatting.Indented));
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
            {
                // 文档库
                System.LibList.AddRange(optionFile.LibPathList);
                System.DefaultPathIndex = optionFile.DefaultPathIndex;
                // 页面宽度
                Page.PageWidth = optionFile.PageWidth;
                string[] margin = optionFile.PagePadding.Split(',');
                // 页边距
                Page.PagePadding = new PageThickness
                {
                    Left = int.Parse(margin[0]),
                    Top = int.Parse(margin[1]),
                    Right = int.Parse(margin[2]),
                    Bottom = int.Parse(margin[3])
                };
                // 首行缩进、段间距
                Paragraph.FirstLineIndent = optionFile.FirstLineIndent;
                Paragraph.ParagraphInterval = optionFile.ParagraphInterval;
                // 边距标记、行线
                View.ShowPaddingMark = optionFile.ShowPaddingMark;
                View.ShowRowLine = optionFile.ShowRowLine;
            }

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