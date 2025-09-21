using GeekDocument.SubSystem.LayoutEngine.Tool;
using System.Windows;

namespace GeekDocument.SubSystem.LayoutEngine;

/// <summary>
/// 文档元素
/// </summary>
public interface IDocumentElement
{
    string Icon { get; set; }

    string Name { get; set; }

    /// <summary>可输入。例如段落、表格都是可以输入的，而图片（无图注）、公式不支持在内部输入</summary>
    bool CanInput { get; }

    /// <summary>
    /// 获取子元素列表
    /// </summary>
    List<IDocumentElement> GetSubElementList();

    /// <summary>
    /// 获取元素的可视区域
    /// </summary>
    Rect GetViewRect();

    /// <summary>
    /// 获取元素的命中测试区域
    /// </summary>
    Rect GetHitTestRect();

    /// <summary>
    /// 获取直接命中元素
    /// </summary>
    IDocumentElement? GetHitedElement(Point point);

    /// <summary>
    /// 获取离坐标最近的元素，用于光标定位
    /// </summary>
    IDocumentElement GetNearestElement(Point point);

    /// <summary>
    /// 处理鼠标按下，左键按下时调用
    /// </summary>
    void HandleMouseDown(Point point);

    /// <summary>
    /// 尝试移动光标至指定坐标处，返回光标信息
    /// </summary>
    CaretInfo MoveCaret(Point point);

    /// <summary>
    /// 获取命中的元素行
    /// </summary>
    元素行 GetHitedLine(Point point);
}