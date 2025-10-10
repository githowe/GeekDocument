using GeekDocument.SubSystem.LayoutEngine.Element;
using System.Windows;

namespace GeekDocument.SubSystem.LayoutEngine
{
    public class 表格列 : IDocElement
    {
        public string Name => "表格列";

        public string Icon => "TableCol";

        public List<IDocElement> ChildrenElement => _empty;

        public Action<IDocElement>? ChildrenChanged { get; set; } = null;

        public Action<IDocElement>? Removed { get; set; } = null;

        public int 列号 { get; set; }

        public List<单元格> 单元格列表 { get; set; } = new List<单元格>();

        public Rect GetViewRect()
        {
            double left = 单元格列表[0].Left;
            double top = 单元格列表[0].Top;
            单元格 last = 单元格列表.Last();
            double right = last.Left + last.ActualWidth;
            double bottom = last.Top + last.ActualHeight;
            return new Rect(left, top, right - left, bottom - top);
        }

        private readonly List<IDocElement> _empty = new List<IDocElement>();
    }
}