namespace GeekDocument.SubSystem.LayoutEngine
{
    /// <summary>
    /// 表示对象的一个属性
    /// </summary>
    public class Property
    {
        public Property() { }

        public Property(string name, string type, string value, bool readOnly = false)
        {
            Name = name;
            Type = type;
            Value = value;
            ReadOnly = readOnly;
        }

        public string Name { get; set; } = "";

        public string Type { get; set; } = "";

        public string Value { get; set; } = "";

        public bool ReadOnly { get; set; } = false;
    }

    /// <summary>
    /// 表示包含属性的对象
    /// </summary>
    public interface IPropertyObject
    {
        List<Property> PropertyList { get; }

        void SetProperty(string name, string value);
    }
}