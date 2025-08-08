using GeekDocument.SubSystem.FileSystem;
using GeekDocument.SubSystem.WindowSystem;
using System.IO;
using System.Windows;
using XLogic.Base;
using XLogic.Wpf.Window;

namespace GeekDocument.SubSystem.DocumentSystem;

public partial class NewDocumentDialog : XDialog
{
    public NewDocumentDialog()
    {
        InitializeComponent();
        Loaded += Dialog_Loaded;
    }

    #region 属性

    /// <summary>文档路径</summary>
    public string DocumentPath { get; private set; } = "";

    /// <summary>文档名称</summary>
    public string DocumentName { get; private set; } = "";

    #endregion

    private void Dialog_Loaded(object sender, RoutedEventArgs e)
    {
        // 获取最近文档路径
        string recentPath = DocManager.Instance.GetRecentDocumentPath();
        // 获取目录信息
        DirectoryInfo directoryInfo = new DirectoryInfo(recentPath);
        // 添加文档名称
        HashSet<string> nameSet = new HashSet<string>();
        foreach (var fileInfo in directoryInfo.EnumerateFiles())
        {
            if (Path.GetExtension(fileInfo.FullName) != ".gdoc") continue;
            nameSet.Add(Path.GetFileNameWithoutExtension(fileInfo.Name));
        }
        // 创建名称生成器
        NameGenerator generator = new NameGenerator
        {
            NameSet = nameSet,
            NameBuilder = (id) => $"新建文档_{id:00}"
        };

        // 新建文档名称
        Input_Name.Text = generator.GenerateName();
        // 新建文档路径
        Input_Path.Text = recentPath;
    }

    private void Tool_Explorer_Click(object sender, RoutedEventArgs e)
    {
        string newPath = FM.Instance.OpenFolderExplorerDialog(Input_Path.Text);
        if (newPath != "") Input_Path.Text = newPath;
    }

    private void OK_Click(object sender, RoutedEventArgs e)
    {
        DocumentPath = Input_Path.Text;
        DocumentName = Input_Name.Text;
        if (DocumentPath.EndsWith("\\")) DocumentPath = DocumentPath[..^1];
        // 检查文档是否已存在
        if (File.Exists($"{DocumentPath}\\{DocumentName}.gdoc"))
        {
            WM.ShowErrorTip($"“{Input_Path.Text}”下已存在文档“{Input_Name.Text}”");
            return;
        }
        DialogResult = true;
    }
}