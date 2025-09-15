using GeekDocument.SubSystem.LayoutEngine;

namespace GeekDocument.SubSystem.EditerSystemNew.Define
{
    public class 块
    {
        public 块类型 类型 { get; set; } = 块类型.Unknown;

        public int[] Margin { get; set; } = new int[4];

        public 布局元素? 根元素 { get; set; } = null;
    }
}