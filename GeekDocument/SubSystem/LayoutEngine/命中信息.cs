using System.Windows;

namespace GeekDocument.SubSystem.LayoutEngine
{
    public class 命中信息
    {
        public Point 坐标 { get; set; } = new Point();

        public 布局元素? 命中元素 { get; set; } = null;

        public Rect 命中区域 { get; set; } = Rect.Empty;

        public string 区域名称 { get; set; } = "未知区域";
    }
}