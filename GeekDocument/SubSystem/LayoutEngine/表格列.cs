using GeekDocument.SubSystem.LayoutEngine.Element;

namespace GeekDocument.SubSystem.LayoutEngine
{
    public class 表格列
    {
        public int 列号 { get; set; }

        public List<单元格> 单元格列表 { get; set; } = new List<单元格>();
    }
}