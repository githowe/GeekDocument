using GeekDocument.SubSystem.LayoutEngine.Element;
using System.Windows;

namespace GeekDocument.SubSystem.LayoutEngine
{
    public class 表格行 : IDocElement, IPropertyObject
    {
        #region IDocElement 接口

        public string Name => "表格行";

        public string Icon => "TableRow";

        public List<IDocElement> ChildrenElement => _empty;

        public Action<IDocElement>? ChildrenChanged { get; set; } = null;

        public Action<IDocElement>? Removed { get; set; } = null;

        public Rect GetViewRect()
        {
            double left = 单元格列表[0].Left;
            double top = 单元格列表[0].Top;
            单元格 last = 单元格列表.Last();
            double right = last.Left + last.ActualWidth;
            double bottom = last.Top + last.ActualHeight;
            return new Rect(left, top, right - left, bottom - top);
        }

        #endregion

        #region IPropertyObject 接口

        public List<Property> PropertyList { get; } = new List<Property>()
        {
            new Property() { Name="行号", Type="int", Value="0", ReadOnly=true },
            new Property() { Name="行高", Type="double", Value="Auto", ReadOnly=true },
        };

        public void SetProperty(string name, string value)
        {

        }

        #endregion

        #region 属性

        public int 行号 { get; set; }

        public List<单元格> 单元格列表 { get; set; } = new List<单元格>();

        /// <summary>手动设置的高度，此高度表示行的最小高度</summary>
        public double 行高 { get; set; } = double.NaN;

        /// <summary>适应行中所有单元格中最高的高度，需要默认设置为手动高度</summary>
        public double 自适应行高 { get; set; } = double.NaN;

        #endregion

        #region 公开方法

        /// <summary>
        /// 每次同步行高时，需要重置为手动行高
        /// </summary>
        public void 重置自适应行高() => 自适应行高 = 行高;

        public void 同步行高()
        {
            // 计算最大行高
            double 最大行高 = 0;
            foreach (var item in 单元格列表)
                if (item.ActualHeight > 最大行高) 最大行高 = item.ActualHeight;
            // 同步行高
            自适应行高 = 最大行高;
        }

        #endregion

        private readonly List<IDocElement> _empty = new List<IDocElement>();
    }
}