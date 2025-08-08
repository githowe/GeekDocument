namespace GeekDocument.SubSystem.GlyphSystem
{
    /// <summary>
    /// 字形节点
    /// </summary>
    public class GlyphNode
    {
        public string Name { get; set; } = "";

        public GlyphImage? Image { get; set; } = null;

        public GlyphNode? Parent { get; set; } = null;

        public List<GlyphNode> NodeList { get; set; } = new List<GlyphNode>();

        public override string ToString() => GetNodePath();

        public GlyphNode? FindNode(string name)
        {
            foreach (var item in NodeList)
                if (item.Name == name) return item;
            return null;
        }

        public string GetNodePath()
        {
            if (Parent == null) return Name;
            else return Parent.GetNodePath() + " > " + Name;
        }
    }
}