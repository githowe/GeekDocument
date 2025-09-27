namespace GeekDocument.SubSystem.LayoutEngine
{
    public class 表格行
    {
        public int 行号 { get; set; }

        public List<单元格> 单元格列表 { get; set; } = new List<单元格>();

        public double 行高 { get; private set; } = double.NaN;

        public void 同步行高()
        {
            // 计算最大行高
            double 最大行高 = 0;
            foreach (var item in 单元格列表)
                if (item.ActualHeight > 最大行高) 最大行高 = item.ActualHeight;
            // 同步行高
            foreach (var item in 单元格列表) item.同步高度(最大行高);
            行高 = 最大行高;
        }
    }
}