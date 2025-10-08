using System.Windows;

namespace GeekDocument.SubSystem.LayoutEngine
{
    public interface IDocElement
    {
        string Name { get; }

        string Icon { get; }

        List<IDocElement> ChildrenElement { get; }

        public Action<IDocElement>? ChildrenChanged { get; set; }

        public Action<IDocElement>? Removed { get; set; }

        Rect GetViewRect();
    }
}