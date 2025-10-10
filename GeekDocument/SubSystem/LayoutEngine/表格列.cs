using GeekDocument.SubSystem.LayoutEngine.Element;
using System.Windows;

namespace GeekDocument.SubSystem.LayoutEngine
{
    public class 表格列 : IDocElement, IPropertyObject
    {
        #region 接口

        public string Name => "表格列";

        public string Icon => "TableCol";

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

        public List<Property> PropertyList
        {
            get
            {
                List<Property> result = new List<Property>();
                result.Add(new Property("列号", "int", (列号 + 1).ToString(), true));
                result.Add(new Property("固定列宽", "double", OwnerTable.获取列宽(列号).ToString()));
                return result;
            }
        }

        public void SetProperty(string name, string value)
        {
            switch (name)
            {
                case "固定列宽":
                    OwnerTable.设置列宽(列号, double.Parse(value));
                    break;
            }
        }

        #endregion

        public 表格 OwnerTable { get; set; } = null!;

        public int 列号 { get; set; }

        public List<单元格> 单元格列表 { get; set; } = new List<单元格>();

        private readonly List<IDocElement> _empty = new List<IDocElement>();
    }
}