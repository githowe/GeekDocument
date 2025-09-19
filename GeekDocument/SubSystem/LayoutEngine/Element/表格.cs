using GeekDocument.SubSystem.LayoutEngine.Ex;
using System.Windows;
using System.Windows.Media;

namespace GeekDocument.SubSystem.LayoutEngine.Element
{
    public class 单元格 : IDocumentElement, IComparable<单元格>
    {
        public 单元格(段落 内容) => this.内容 = 内容;

        #region IDocumentElement 成员

        public string Icon { get; set; } = "Cell";

        public string Name
        {
            get => $"单元格_{行},{列}";
            set { }
        }

        public List<IDocumentElement> GetSubElementList()
        {
            return new List<IDocumentElement> { 内容 };
        }

        public Rect GetElementRect()
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

        #endregion

        public int CompareTo(单元格? other)
        {
            if (other == null) return 1;
            if (行 != other.行) return 行.CompareTo(other.行);
            return 列.CompareTo(other.列);
        }

        public 表格? Owner { get; set; } = null;

        public int 行 { get; set; } = 0;

        public int 列 { get; set; } = 0;

        public double 宽度 { get; set; } = 136;

        public double 高度 { get; set; } = 44;

        public Thickness Padding { get; set; } = new Thickness(4);

        public 水平对齐方式 Horizontal { get; set; } = 水平对齐方式.Left;

        public 垂直对齐方式 Vertical { get; set; } = 垂直对齐方式.Top;

        public 段落 内容 { get; set; }

        #region 运行时属性

        public double HorizontalPadding => Padding.Left + Padding.Right;

        public double VerticalPadding => Padding.Top + Padding.Bottom;

        #endregion

        public override string ToString() => $"单元格({行},{列}) 宽度={宽度} 高度={高度} 内容={内容}";
    }

    public class 表格行
    {
        public int 行号 { get; set; }

        public List<单元格?> 单元格列表 { get; set; } = new List<单元格?>();

        public void SetHeight(double height)
        {
            foreach (var cell in 单元格列表)
                if (cell != null) cell.高度 = height;
        }
    }

    public class 表格列
    {
        public int 列号 { get; set; }

        public List<单元格?> 单元格列表 { get; set; } = new List<单元格?>();

        public void SetWidth(double width)
        {
            foreach (var cell in 单元格列表)
                if (cell != null) cell.宽度 = width;
        }
    }

    public class 表格 : 布局元素
    {
        public 表格() => 类型 = 元素类型.表格;

        #region IDocumentElement 成员

        public override string Icon { get; set; } = "Table";

        public override string Name { get; set; } = "表格";

        public override List<IDocumentElement> GetSubElementList()
        {
            List<IDocumentElement> result = new List<IDocumentElement>();
            foreach (var item in 单元格列表)
                result.Add(item);
            return result;
        }

        #endregion

        #region 属性

        public int 行数 { get; set; } = 4;

        public int 列数 { get; set; } = 4;

        public List<double> 全部行高 { get; set; } = new List<double>();

        public List<double> 全部列宽 { get; set; } = new List<double>();

        public double 边框粗细 { get; set; } = 1;

        #endregion

        #region 布局元素方法

        public override List<ElementLayer> GetLayerList()
        {
            List<ElementLayer> result = new List<ElementLayer>();
            foreach (var item in 单元格列表)
                result.AddRange(item.内容.GetLayerList());
            return result;
        }

        public override void Init()
        {
            // 生成行
            for (int line = 0; line < 行数; line++)
            {
                表格行 行 = new 表格行 { 行号 = line };
                for (int list = 0; list < 列数; list++) 行.单元格列表.Add(null);
                行列表.Add(行);
            }
            // 生成列
            for (int list = 0; list < 列数; list++)
            {
                表格列 列 = new 表格列 { 列号 = list };
                for (int line = 0; line < 行数; line++) 列.单元格列表.Add(null);
                列列表.Add(列);
            }

            for (int counter = 0; counter < 行数; counter++)
                全部行高.Add(_cellHeight);
            for (int counter = 0; counter < 列数; counter++)
                全部列宽.Add(_cellWidth);

            _borderBrush.Freeze();
            if (边框粗细 != 1) _borderPen = new Pen(_borderBrush, 边框粗细);
            _borderPen.Freeze();
        }

        public override void Measure()
        {
            foreach (var item in 单元格列表)
            {
                item.内容.MaxWidth = item.宽度 - item.HorizontalPadding;
                item.内容.MaxHeight = item.高度 - item.VerticalPadding;
                item.内容.Measure();
                if (item.内容 is 段落 段) 段.FitContentWidth();
            }
            ActualWidth = 全部列宽.Sum() + 边框粗细 * (列数 + 1);
            ActualHeight = 全部行高.Sum() + 边框粗细 * (行数 + 1);
        }

        public override void Arrange()
        {
            // 遍历单元格，计算位置并排列内容
            foreach (var item in 单元格列表)
            {
                (double x, double y) = 计算单元格位置(item.行, item.列);
                switch (item.Horizontal)
                {
                    case 水平对齐方式.Left:
                    case 水平对齐方式.Justify:
                        item.内容.Left = x;
                        break;
                    case 水平对齐方式.Center:
                        item.内容.Left = x + (item.宽度 - item.HorizontalPadding - item.内容.ActualWidth) / 2;
                        break;
                    case 水平对齐方式.Right:
                        item.内容.Left = x + item.宽度 - item.HorizontalPadding - item.内容.ActualWidth;
                        break;
                }
                switch (item.Vertical)
                {
                    case 垂直对齐方式.Top:
                        item.内容.Top = y;
                        break;
                    case 垂直对齐方式.Center:
                        item.内容.Top = y + (item.高度 - item.VerticalPadding - item.内容.ActualHeight) / 2;
                        break;
                    case 垂直对齐方式.Bottom:
                        item.内容.Top = y + item.高度 - item.VerticalPadding - item.内容.ActualHeight;
                        break;
                }
                item.内容.Arrange();
            }
        }

        public override void 绘图(DrawingContext dc)
        {
            绘制表格线(dc);
            foreach (var item in 单元格列表)
                item.内容.绘图(dc);
        }

        #endregion

        #region 公开方法

        public void 设置单元格内容(int 行, int 列, 布局元素 内容)
        {
            if (行 < 0 || 行 >= 行数) return;
            if (列 < 0 || 列 >= 列数) return;

            // 包装为段落
            段落 段落;
            if (内容.类型 == 元素类型.段落) 段落 = (段落)内容;
            else
            {
                段落 = new 段落 { 首行缩进 = 0 };
                段落.AddLayoutElement(内容);
            }
            段落.Parent = this;
            // 创建单元格
            单元格 cell = new 单元格(段落)
            {
                Owner = this,
                行 = 行,
                列 = 列,
                宽度 = 全部列宽[列],
                高度 = 全部行高[行],
                Padding = _cellPadding,
                Horizontal = _cellHorizontal,
                Vertical = _cellVertical,
            };
            // 引用单元格
            行列表[行].单元格列表[列] = cell;
            列列表[列].单元格列表[行] = cell;
            // 添加到单元格列表
            单元格列表.Add(cell);
            // 排序。按左上至右下顺序
            单元格列表.Sort();
        }

        public T? 获取单元格内容<T>(int 行, int 列) where T : 布局元素
        {
            if (行 < 0 || 行 >= 行数) return null;
            if (列 < 0 || 列 >= 列数) return null;
            // 获取单元格
            单元格? cell = 行列表[行].单元格列表[列];
            if (cell is null) return null;
            // 转换类型并返回
            return cell.内容 as T;
        }

        public void 移除单元格内容(int 行, int 列)
        {
            if (行 < 0 || 行 >= 行数) return;
            if (列 < 0 || 列 >= 列数) return;
            // 获取单元格
            单元格? cell = 行列表[行].单元格列表[列];
            if (cell is null) return;
            // 从单元格列表中移除
            单元格列表.Remove(cell);
            // 清空引用
            行列表[行].单元格列表[列] = null;
            列列表[列].单元格列表[行] = null;
        }

        public void 设置行高(int row, double height)
        {
            行列表[row].SetHeight(height);
            全部行高[row] = height;
        }

        public void 设置列宽(int col, double width)
        {
            列列表[col].SetWidth(width);
            全部列宽[col] = width;
        }

        public (double x, double y) 计算单元格位置(int 行, int 列)
        {
            double x = Left + 边框粗细;
            double y = Top + 边框粗细;
            for (int col = 0; col < 列; col++) x += 全部列宽[col] + 边框粗细;
            for (int row = 0; row < 行; row++) y += 全部行高[row] + 边框粗细;
            return (x + 4, y + 4);
        }

        #endregion

        #region 私有方法

        private void 绘制表格线(DrawingContext dc)
        {
            double tableOffset = 边框粗细 / 2;
            double x = Math.Round(Left) + tableOffset;
            double y = Math.Round(Top) + tableOffset;
            // 绘制横线
            for (int row = 0; row <= 行数; row++)
            {
                dc.DrawLine(_borderPen, new Point(x - tableOffset, y), new Point(x + ActualWidth - tableOffset, y));
                if (row < 行数) y += 全部行高[row] + 边框粗细;
            }
            // 绘制竖线
            y = Math.Round(Top) + tableOffset;
            for (int col = 0; col <= 列数; col++)
            {
                dc.DrawLine(_borderPen, new Point(x, y - tableOffset), new Point(x, y + ActualHeight - tableOffset));
                if (col < 列数) x += 全部列宽[col] + 边框粗细;
            }
        }

        #endregion

        #region 默认单元格参数

        /// <summary>单元格高度</summary>
        private double _cellHeight = 44;
        /// <summary>单元格高度</summary>
        private double _cellWidth = 136;
        /// <summary>单元格内边距：左、上、右、下</summary>
        private Thickness _cellPadding = new Thickness(4);
        /// <summary>单元格内容水平对齐</summary>
        private 水平对齐方式 _cellHorizontal = 水平对齐方式.Center;
        /// <summary>单元格内容垂直对齐</summary>
        private 垂直对齐方式 _cellVertical = 垂直对齐方式.Center;

        #endregion

        #region 字段

        private List<表格行> 行列表 = new List<表格行>();
        private List<表格列> 列列表 = new List<表格列>();
        private List<单元格> 单元格列表 = new List<单元格>();

        private Brush _borderBrush = new SolidColorBrush(Color.FromRgb(100, 100, 100));
        private Pen _borderPen = new Pen(new SolidColorBrush(Color.FromRgb(100, 100, 100)), 1);

        #endregion
    }
}