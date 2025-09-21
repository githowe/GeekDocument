using GeekDocument.SubSystem.LayoutEngine.Ex;
using GeekDocument.SubSystem.LayoutEngine.Tool;
using System.Windows;

namespace GeekDocument.SubSystem.LayoutEngine.Element
{
    public class 单元格 : IDocumentElement, IComparable<单元格>
    {
        #region 构造方法

        public 单元格(段落 内容) => this.内容 = 内容;

        #endregion

        #region IDocumentElement 成员

        public string Icon { get; set; } = "Cell";

        public string Name
        {
            get => $"单元格_{行},{列}";
            set { }
        }

        public bool CanInput => true;

        public List<IDocumentElement> GetSubElementList()
        {
            return new List<IDocumentElement> { 内容 };
        }

        public Rect GetViewRect()
        {
            if (Owner != null)
            {
                (double x, double y) = Owner.计算单元格位置(行, 列);
                段落 root = Owner.GetRootParagraph();
                y += root.段落偏移;
                return new Rect(x - Padding.Left, y - Padding.Top, 宽度, 高度);
            }
            return new Rect(0 - Padding.Left, 0 - Padding.Top, 宽度, 高度);
        }

        public Rect GetHitTestRect() => GetViewRect();

        public IDocumentElement? GetHitedElement(Point point)
        {
            IDocumentElement? hited = 内容.GetHitedElement(point);
            if (hited != null) return hited;
            if (GetHitTestRect().Contains(point)) return this;
            return null;
        }

        public IDocumentElement GetNearestElement(Point point)
        {
            return 内容.GetNearestElement(point);
        }

        public void HandleMouseDown(Point point)
        {

        }

        public CaretInfo MoveCaret(Point point)
        {
            return 内容.MoveCaret(point);
        }

        public 元素行 GetHitedLine(Point point)
        {
            return 内容.GetHitedLine(point);
        }

        #endregion

        #region IComparable 成员

        public int CompareTo(单元格? other)
        {
            if (other == null) return 1;
            if (行 != other.行) return 行.CompareTo(other.行);
            return 列.CompareTo(other.列);
        }

        #endregion

        #region 属性

        public 表格? Owner { get; set; } = null;

        public int 行 { get; set; } = 0;

        public int 列 { get; set; } = 0;

        public double 宽度 { get; set; } = 136;

        public double 高度 { get; set; } = 44;

        public Thickness Padding { get; set; } = new Thickness(4);

        public 水平对齐方式 Horizontal { get; set; } = 水平对齐方式.Left;

        public 垂直对齐方式 Vertical { get; set; } = 垂直对齐方式.Top;

        public 段落 内容 { get; set; }

        #endregion

        #region 运行时属性

        public double HorizontalPadding => Padding.Left + Padding.Right;

        public double VerticalPadding => Padding.Top + Padding.Bottom;

        #endregion

        #region object 方法

        public override string ToString() => $"单元格({行},{列}) 宽度={宽度} 高度={高度} 内容={内容}";

        #endregion
    }
}