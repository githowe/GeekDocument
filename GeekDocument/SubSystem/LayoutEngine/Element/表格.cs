using GeekDocument.SubSystem.LayoutEngine.Ex;
using GeekDocument.SubSystem.LayoutEngine.Tool;
using System.Windows;
using System.Windows.Media;

namespace GeekDocument.SubSystem.LayoutEngine.Element
{
    public class 表格 : 布局元素
    {
        public 表格() => 类型 = 元素类型.表格;

        #region IDocumentElement 成员

        public override string Icon { get; set; } = "Table";

        public override string Name { get; set; } = "表格";

        public override bool CanInput => true;

        public override List<IDocumentElement> GetSubElementList()
        {
            List<IDocumentElement> result = new List<IDocumentElement>(单元格列表);
            return result;
        }

        public override Rect GetHitTestRect()
        {
            Rect viewRect = GetViewRect();
            Rect hitRect = new Rect(viewRect.Left - 16, viewRect.Top - 16, viewRect.Width + 16, viewRect.Height + 16);
            return hitRect;
        }

        public override CaretInfo MoveCaret(Point point)
        {
            单元格 命中单元格 = 获取命中单元格(point);
            return 命中单元格.内容.MoveCaret(point);
        }

        public override 元素行 GetHitedLine(Point point)
        {
            单元格 命中单元格 = 获取命中单元格(point);
            return 命中单元格.内容.GetHitedLine(point);
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
            // 生成行高与列宽
            for (int counter = 0; counter < 行数; counter++)
                全部行高.Add(_cellHeight);
            for (int counter = 0; counter < 列数; counter++)
                全部列宽.Add(_cellWidth);
            // 生成单元格
            for (int line = 0; line < 行数; line++)
            {
                for (int list = 0; list < 列数; list++)
                {
                    段落 段落 = new 段落 { 首行缩进 = 0 };
                    设置单元格内容(line, list, 段落);
                }
            }

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

            // 先移除已有内容
            移除单元格内容(行, 列);
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
            // 解除引用
            cell.内容.Parent = null;
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

        public (double y1, double y2) 计算行区域(int 行)
        {
            double y = Top + 边框粗细;
            for (int row = 0; row < 行; row++) y += 全部行高[row] + 边框粗细;
            return (y, y + 全部行高[行]);
        }

        public (double x1, double x2) 计算列区域(int 列)
        {
            double x = Left + 边框粗细;
            for (int col = 0; col < 列; col++) x += 全部列宽[col] + 边框粗细;
            return (x, x + 全部列宽[列]);
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

        private 单元格 获取命中单元格(Point point)
        {
            段落 root = this.GetRootParagraph();
            double top = root.段落偏移 + Top;

            int rowIndex = -1;
            // 先获取命中行
            for (int index = 0; index < 行列表.Count; index++)
            {
                (double y1, double y2) = 计算行区域(index);
                y1 += top;
                y2 += top;
                double y = point.Y;
                if (y1 <= y && y < y2)
                {
                    rowIndex = index;
                    break;
                }
            }
            // 无命中行，可能点在了边线上
            if (rowIndex == -1)
            {
                rowIndex = 0;
                double 最小距离 = double.MaxValue;
                for (int index = 0; index < 行列表.Count; index++)
                {
                    (double y1, double y2) = 计算行区域(index);
                    y1 += top;
                    y2 += top;
                    double distance = Math.Min(Math.Abs(point.Y - y1), Math.Abs(point.Y - y2));
                    if (distance < 最小距离)
                    {
                        最小距离 = distance;
                        rowIndex = index;
                    }
                }
            }

            // 再获取命中列
            int colIndex = -1;
            for (int index = 0; index < 列列表.Count; index++)
            {
                (double x1, double x2) = 计算列区域(index);
                double x = point.X;
                if (x1 <= x && x < x2)
                {
                    colIndex = index;
                    break;
                }
            }
            // 无命中列，可能点在了边线上
            if (colIndex == -1)
            {
                colIndex = 0;
                double 最小距离 = double.MaxValue;
                for (int index = 0; index < 列列表.Count; index++)
                {
                    (double x1, double x2) = 计算列区域(index);
                    double distance = Math.Min(Math.Abs(point.X - x1), Math.Abs(point.X - x2));
                    if (distance < 最小距离)
                    {
                        最小距离 = distance;
                        colIndex = index;
                    }
                }
            }

            return 行列表[rowIndex].单元格列表[colIndex];
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
        private 水平对齐方式 _cellHorizontal = 水平对齐方式.Left;
        /// <summary>单元格内容垂直对齐</summary>
        private 垂直对齐方式 _cellVertical = 垂直对齐方式.Top;

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