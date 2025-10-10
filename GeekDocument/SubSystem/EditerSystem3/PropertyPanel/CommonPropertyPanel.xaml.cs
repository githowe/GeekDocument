using GeekDocument.SubSystem.EditerSystem3.PropertyBar;
using GeekDocument.SubSystem.LayoutEngine;
using System.Windows;

namespace GeekDocument.SubSystem.EditerSystem3.PropertyPanel
{
    public partial class CommonPropertyPanel : PropertyPanel
    {
        public CommonPropertyPanel() => InitializeComponent();

        public IPropertyObject Instance { get; set; } = null!;

        public override void Init()
        {
            foreach (var item in Instance.PropertyList)
            {
                switch (item.Type)
                {
                    case "int":
                    case "double":
                        AddDoubleProperty(item.Name, item.Value, item.ReadOnly);
                        break;
                }
            }
        }

        private void AddDoubleProperty(string name, string value, bool readOnly)
        {
            TextInput bar = new TextInput()
            {
                Title = name,
                ReadOnly = readOnly,
                Margin = new Thickness(0, 10, 0, 0),
            };
            if (PropertyStack.Children.Count == 0)
                bar.Margin = new Thickness(0);
            PropertyStack.Children.Add(bar);
            bar.LoadProperty(value);
            if (readOnly) return;
            bar.TextChanged += (text) =>
            {
                Instance.SetProperty(bar.Title, text);
            };
        }
    }
}