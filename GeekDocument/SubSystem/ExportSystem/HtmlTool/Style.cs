namespace GeekDocument.SubSystem.ExportSystem.HtmlTool
{
    public abstract class StyleItem
    {
        public string Name { get; set; } = "";

        public abstract string ToLine();
    }

    public class Item_Double : StyleItem
    {
        public Item_Double(string name, double value = 0)
        {
            Name = name;
            Value = value;
        }

        public double Value { get; set; } = 0;

        public override string ToLine() => $"{Name}: {Value}px;";
    }

    public class Item_Enum : StyleItem
    {
        public Item_Enum(string name, string value = "")
        {
            Name = name;
            Value = value;
        }

        public string Value { get; set; } = "";

        public override string ToLine() => $"{Name}: {Value};";
    }

    public class Style
    {
        public List<StyleItem> StyleItemList { get; set; } = new List<StyleItem>();

        public string ToLine()
        {
            List<string> lineList = new List<string>();
            foreach (var item in StyleItemList) lineList.Add(item.ToLine());
            return string.Join(" ", lineList);
        }
    }
}