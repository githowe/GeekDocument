using GeekDocument.SubSystem.LayoutEngine;
using GeekDocument.SubSystem.LayoutEngine.Element;

namespace GeekDocument.SubSystem.ExportSystem.HtmlTool
{
    public class 表格行节点 : HtmlNode
    {
        public 表格行节点()
        {
            Markup = "tr";
        }

        public 表格行 Element { get; set; } = null!;

        /// <summary>第一行</summary>
        public bool FirstLine { get; set; } = false;

        /// <summary>偶数行</summary>
        public bool EvenLine { get; set; } = false;

        public override void Init()
        {
            // 设置样式
            if (FirstLine) Class = "headRow";
            else if (EvenLine) Class = "evenRow";
            // 设置行高
            Style style = new Style();
            style.StyleItemList.Add(new Item_Double("height", Element.自适应行高));
            PropertyList.Add(new NodeProperty
            {
                Name = "style",
                Value = style.ToLine()
            });
        }

        public override string ToLine()
        {
            string startTag = GenerateStartTag();
            string endTag = GenerateEndTag();
            string innerText = "";
            // 遍历单元格
            foreach (var item in Element.单元格列表)
            {
                单元格节点 node = new 单元格节点
                {
                    Element = item,
                    TableHead = FirstLine,
                };
                node.Init();
                innerText += node.ToLine();
            }
            return $"{startTag}{innerText}{endTag}";
        }
    }

    public class 单元格节点 : HtmlNode
    {
        public 单元格节点()
        {
            Markup = "td";
        }

        public 单元格 Element { get; set; } = null!;

        public bool TableHead { get; set; } = false;

        public override void Init()
        {
            // 如果是表头，使用<th>标签
            if (TableHead) Markup = "th";
            // 设置单元格宽度
            Style style = new Style();
            style.StyleItemList.Add(new Item_Double("width", Element.ActualWidth));
            PropertyList.Add(new NodeProperty
            {
                Name = "style",
                Value = style.ToLine()
            });
        }

        public override string ToLine()
        {
            string startTag = GenerateStartTag();
            string endTag = GenerateEndTag();
            段落节点 node = new 段落节点
            {
                Element = Element.段落,
                CellContent = true,
            };
            node.Init();
            string innerText = node.ToLine();
            return $"{startTag}{innerText}{endTag}";
        }
    }

    public class 表格节点 : HtmlNode
    {
        public 表格节点()
        {
            Markup = "table";
        }

        public 表格 Element { get; set; } = null!;

        public override void Init()
        {
            Class = "normalTable";
        }

        public override string ToLine()
        {
            string startTag = GenerateStartTag();
            string endTag = GenerateEndTag();
            string innerText = "";

            foreach (var 表格行 in Element.全部行)
            {
                表格行节点 node = new 表格行节点
                {
                    Element = 表格行,
                    FirstLine = 表格行.行号 == 0,
                    EvenLine = 表格行.行号 % 2 == 0,
                };
                node.Init();
                innerText += node.ToLine();
            }

            return $"{startTag}{innerText}{endTag}";
        }
    }
}