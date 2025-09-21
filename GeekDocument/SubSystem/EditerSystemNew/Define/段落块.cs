using GeekDocument.SubSystem.EditerSystemNew.Control;
using GeekDocument.SubSystem.LayoutEngine;
using GeekDocument.SubSystem.LayoutEngine.Element;
using System.Windows.Media;

namespace GeekDocument.SubSystem.EditerSystemNew.Define;

public class 段落块
{
    public 段落块(段落 段落)
    {
        段落.OwnerBlock = this;
        this.段落 = 段落;
    }

    public Page OwnerPage { get; set; } = null!;

    public 段落 段落 { get; set; }

    /// <summary>块横坐标。由页面控件设置</summary>
    public double BlockLeft { get; set; } = 0;

    /// <summary>块纵坐标。由页面控件设置</summary>
    public double BlockTop { get; set; } = 0;

    /// <summary>纵向偏移，相对于第一个块纵坐标</summary>
    public double TopOffset { get; set; } = 0;

    /// <summary>块宽度。由页面控件设置</summary>
    public double BlockWidth { get; set; } = 0;

    /// <summary>块高度。根据内部元素布局设置</summary>
    public double BlockHeight { get; private set; } = 0;

    public double 段前距 { get; set; } = 0;

    public double 段后距 { get; set; } = 0;

    public List<ElementLayer> LayerList => _layerList;

    public void InitLayer()
    {
        _layerList.Add(_layer);
        _layerList.AddRange(段落.GetLayerList());
    }

    /// <summary>
    /// 更新块布局
    /// </summary>
    public void UpdateBlockLayout()
    {
        // 设置段落宽度并计算大小
        段落.MaxWidth = BlockWidth;
        段落.Measure();
        // 初始化段落坐标，因为默认坐标为非数字（double.NaN）
        段落.Left = 0;
        段落.Top = 0;
        段落.段落偏移 = TopOffset;
        // 排列段落内部元素
        段落.Arrange();
        // 设置块高度为段落实际高度
        BlockHeight = 段落.ActualHeight;
    }

    public void Update()
    {
        DrawingContext dc = _layer.Open();
        段落.绘图(dc);
        dc.Close();
    }

    public void MoveLeftCaret()
    {
        OwnerPage.MoveCaretToPrevBlock(this);
    }

    public void MoveCaretToEnd()
    {
        段落.MoveInCaretToEnd();
    }

    private readonly ElementLayer _layer = new ElementLayer();
    private readonly List<ElementLayer> _layerList = new List<ElementLayer>();
}