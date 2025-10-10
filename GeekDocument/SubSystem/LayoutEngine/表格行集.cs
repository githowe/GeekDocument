using System.Windows;

namespace GeekDocument.SubSystem.LayoutEngine
{
    public class 表格行集 : IDocElement
    {
        public string Name => "行集";

        public string Icon => "Set";

        public List<IDocElement> ChildrenElement => 行列表.Cast<IDocElement>().ToList();

        public Action<IDocElement>? ChildrenChanged { get; set; } = null;

        public Action<IDocElement>? Removed { get; set; } = null;

        public Rect GetViewRect() => Rect.Empty;

        public List<表格行> 行列表 { get; set; } = new List<表格行>();
    }
}