namespace GeekDocument.SubSystem.LayoutEngine.Element
{
    public class 表格行
    {
        public int 行号 { get; set; }

        public List<单元格> 单元格列表 { get; set; } = new List<单元格>();

        public void SetHeight(double height)
        {
            foreach (var cell in 单元格列表)
                if (cell != null) cell.高度 = height;
        }
    }
}