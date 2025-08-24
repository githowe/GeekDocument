namespace GeekDocument.SubSystem.StyleSystem
{
    /// <summary>
    /// 样式项
    /// </summary>
    public class StyleItem
    {
        /// <summary>样式名</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>样式唯一编号</summary>
        public int ID { get; set; } = 0;

        /// <summary>样式值</summary>
        public string Value { get; set; } = string.Empty;

        public StyleItem() { }

        public StyleItem(string name, int id, string value)
        {
            Name = name;
            ID = id;
            Value = value;
        }
    }

    /// <summary>
    /// 样式表。定义文档中各种元素的样式
    /// </summary>
    public class StyleSheet
    {
        /// <summary>样式表名</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>样式项列表</summary>
        public List<StyleItem> ItemList { get; set; } = new List<StyleItem>();

        public StyleSheet() { }

        public StyleSheet(string name) => Name = name;

        public void AddItem(string name, int id, string value)
            => ItemList.Add(new StyleItem(name, id, value));
    }
}