using GeekDocument.SubSystem.EditerSystem.Define;
using GeekDocument.SubSystem.EditerSystem.Define.BlockDerive;
using GeekDocument.SubSystem.FileSystem;
using GeekDocument.SubSystem.ImageSystem;
using GeekDocument.SubSystem.OptionSystem;
using GeekDocument.SubSystem.WindowSystem;
using XLogic.Base.UI;

namespace GeekDocument.SubSystem.EditerSystem.Core.Component;

/// <summary>
/// 编辑器组件
/// </summary>
public class EditerComponent : Component<Editer>
{
    protected override void Init()
    {
        GetComponent<ToolBarComponent>().ToolClick += ToolBar_ToolClick;
    }

    private void ToolBar_ToolClick(string name)
    {
        switch (name)
        {
            case "Tool_Save":
                SaveDocument();
                break;
            case "Tool_SaveAs":
                break;
            case "Tool_Export":
                break;
            case "Tool_Undo":
                break;
            case "Tool_Redo":
                break;
            case "Tool_Option":
                OpenOptionDialog();
                break;

            case "Tool_Image":
                InsertImage();
                break;
        }
    }

    /// <summary>
    /// 保存文档
    /// </summary>
    private void SaveDocument()
    {
        GetComponent<DocumentComponent>().SaveDocument();
        _host.Saved = true;
    }

    /// <summary>
    /// 打开选项对话框
    /// </summary>
    private void OpenOptionDialog()
    {
        DocumentOptionDialog dialog = new DocumentOptionDialog { Owner = WM.Main };
        Document document = GetComponent<DocumentComponent>().Document;
        dialog.Init(document);
        if (dialog.ShowDialog() != true) return;

        // 更新文档选项
        document.PageWidth = dialog.Panel_PageOption.PageWidth;
        document.Padding.Top = dialog.Panel_PageOption.Top;
        document.Padding.Bottom = dialog.Panel_PageOption.Bottom;
        document.Padding.Left = dialog.Panel_PageOption.Left;
        document.Padding.Right = dialog.Panel_PageOption.Right;
        document.FirstLineIndent = int.Parse(dialog.Panel_ParagraphOption.Input_FirstLineIndent.Text);
        document.ParagraphInterval = int.Parse(dialog.Panel_ParagraphOption.Input_ParagraphInterval.Text);
        // 加载文档选项
        GetComponent<DocumentComponent>().LoadDocumentOption();
        // 更新页面
        GetComponent<PageComponent>().UpdatePageLayout();
        // 更新为未保存状态
        _host.Saved = false;
    }

    /// <summary>
    /// 插入图片
    /// </summary>
    private void InsertImage()
    {
        // 选择图片
        string imagePath = FM.Instance.OpenReadImageDialog("插入图片");
        if (imagePath == "") return;

        // 加载图片
        ImageInfo? imageInfo = ImageLoader.Instance.LoadImageFile(imagePath);
        if (imageInfo == null)
        {
            WM.ShowErrorTip($"加载图片“{imagePath}”失败");
            return;
        }

        // 创建图片块
        BlockImage block = new BlockImage
        {
            SourceWidth = imageInfo.Width,
            SourceHeight = imageInfo.Height,
            FrameList = imageInfo.FrameList,
            Duration = imageInfo.Duration,
            Caption = System.IO.Path.GetFileName(imagePath),
        };
        // 插入图片块
        GetComponent<PageComponent>().插入块(block, GetComponent<PageComponent>().获取当前块索引() + 1);
    }
}