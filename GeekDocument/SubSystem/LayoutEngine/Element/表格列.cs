namespace GeekDocument.SubSystem.LayoutEngine.Element
{
    public class 表格列
    {
        public int 列号 { get; set; }

        public List<单元格> 单元格列表 { get; set; } = new List<单元格>();

        public void SetWidth(double width)
        {
            foreach (var cell in 单元格列表)
                if (cell != null) cell.宽度 = width;
        }
    }
}