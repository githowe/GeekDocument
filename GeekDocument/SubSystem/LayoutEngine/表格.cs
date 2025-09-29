using System.Windows;
using System.Windows.Media;

namespace GeekDocument.SubSystem.LayoutEngine
{
    public class 表格 : 行内元素
    {
        #region 构造方法

        public 表格()
        {
            Name = "表格";
            Icon = "Table";
        }

        #endregion

        #region 属性

        public int 行数 { get; set; } = 4;

        public int 列数 { get; set; } = 4;

        public double 单元格高度 { get; set; } = 24;

        public double 单元格宽度 { get; set; } = 136;

        public double 边框粗细 { get; set; } = 1;

        #endregion

        #region 运行时属性

        public List<double> 全部行高 { get; set; } = new List<double>();

        public List<double> 全部列宽 { get; set; } = new List<double>();

        public List<单元格> 单元格列表 { get; set; } = new List<单元格>();

        #endregion

        #region 布局元素核心方法

        public override void Init()
        {
            CanInput = true;
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
                全部行高.Add(单元格高度);
            for (int counter = 0; counter < 列数; counter++)
                全部列宽.Add(单元格宽度);
            // 生成单元格
            for (int line = 0; line < 行数; line++)
            {
                for (int list = 0; list < 列数; list++)
                {
                    段落 段落 = new 段落 { 首行缩进 = 0 };
                    段落.Init();
                    设置单元格内容(line, list, 段落);
                }
            }

            _borderBrush.Freeze();
            if (边框粗细 != 1) _borderPen = new Pen(_borderBrush, 边框粗细);
            _borderPen.Freeze();
        }

        public override void 测量()
        {
            // 测量单元格
            foreach (var cell in 单元格列表) cell.测量();
            // 同步行高
            foreach (var 行 in 行列表) 行.同步行高();
            // 更新全部行高
            for (int index = 0; index < 行数; index++)
                全部行高[index] = 行列表[index].行高;
            // 计算表格实际尺寸
            ActualWidth = 全部列宽.Sum() + 边框粗细 * (列数 + 1);
            ActualHeight = 全部行高.Sum() + 边框粗细 * (行数 + 1);
        }

        public override void 重新测量()
        {
            Parent?.重新测量();
        }

        public override void 排列()
        {
            foreach (var item in 单元格列表)
            {
                (double x, double y) = 计算单元格坐标(item.行号, item.列号);
                item.Left = x;
                item.Top = y;
                item.排列();
            }
        }

        public override void 渲染(DrawingContext? dc)
        {
            // 绘制边框
            DrawingContext self_dc = _表格绘图对象.RenderOpen();
            绘制表格线(self_dc);
            self_dc.Close();
            // 渲染单元格
            foreach (var item in 单元格列表) item.渲染(dc);
        }

        public override List<绘图对象> 获取绘图对象()
        {
            List<绘图对象> result = new List<绘图对象>();
            result.Add(_表格绘图对象);
            foreach (var item in 单元格列表)
                result.AddRange(item.获取绘图对象());
            return result;
        }

        public override 命中信息? 获取命中信息(Point point)
        {
            Rect 拖动手柄区域 = new Rect(Left - 16, Top - 16, 16, 16);
            Rect 表格上方 = new Rect(Left, Top - 16, ActualWidth + 3, 16);
            Rect 表格左侧 = new Rect(Left - 16, Top, 16, ActualHeight + 3);
            Rect 表格区域 = new Rect(Left, Top, ActualWidth, ActualHeight);

            if (拖动手柄区域.Contains(point))
            {
                return new 命中信息
                {
                    坐标 = point,
                    命中元素 = this,
                    命中区域 = 拖动手柄区域,
                    区域名称 = "拖动手柄区域",
                };
            }
            if (表格上方.Contains(point))
            {
                return new 命中信息
                {
                    坐标 = point,
                    命中元素 = this,
                    命中区域 = 表格上方,
                    区域名称 = "表格上方",
                };
            }
            if (表格左侧.Contains(point))
            {
                return new 命中信息
                {
                    坐标 = point,
                    命中元素 = this,
                    命中区域 = 表格左侧,
                    区域名称 = "表格左侧",
                };
            }
            if (表格区域.Contains(point))
            {
                单元格 命中单元格 = 获取命中单元格(point);
                Rect 单元格区域 = new Rect(命中单元格.Left, 命中单元格.Top, 命中单元格.ActualWidth, 命中单元格.ActualHeight);
                return new 命中信息
                {
                    坐标 = point,
                    命中元素 = 命中单元格,
                    命中区域 = 单元格区域,
                    区域名称 = "单元格",
                };
            }

            return null;
        }

        public override 元素行 获取最近元素行(Point point)
        {
            单元格 命中单元格 = 获取命中单元格(point);
            return 命中单元格.获取最近元素行(point);
        }

        public override void 移入光标至开头()
        {
            单元格列表[0].移入光标至开头();
        }

        public override void 移入光标至末尾()
        {
            单元格列表.Last().移入光标至末尾();
        }

        public void 移动光标至上一个单元格(单元格 cell)
        {
            // 获取当前单元格索引
            int cellIndex = 单元格列表.IndexOf(cell);
            // 有上一个单元格
            if (cellIndex > 0)
            {
                单元格 上一个 = 单元格列表[cellIndex - 1];
                上一个.移入光标至末尾();
            }
            // 无上一个单元格，从表格开头移出光标
            else Parent?.从开头移出光标(this);
        }

        public void 移动光标至下一个单元格(单元格 cell)
        {
            // 获取当前单元格索引
            int cellIndex = 单元格列表.IndexOf(cell);
            // 有下一个单元格
            if (cellIndex < 单元格列表.Count - 1)
            {
                单元格 下一个 = 单元格列表[cellIndex + 1];
                下一个.移入光标至开头();
            }
            // 无下一个单元格，从表格末尾移出光标
            else Parent?.从末尾移出光标(this);
        }

        #endregion

        #region 私有方法

        private void 设置单元格内容(int 行, int 列, 段落 段落)
        {
            if (行 < 0 || 行 >= 行数) return;
            if (列 < 0 || 列 >= 列数) return;

            // 创建单元格
            单元格 cell = new 单元格
            {
                行号 = 行,
                列号 = 列,
                Padding = _cellPadding,
                Width = 全部列宽[列],
                MinHeight = 全部行高[行],
                水平对齐 = _cellHorizontal,
                垂直对齐 = _cellVertical,
            };
            // 引用单元格
            行列表[行].单元格列表[列] = cell;
            列列表[列].单元格列表[行] = cell;
            // 添加到单元格列表
            单元格列表.Add(cell);
            // 排序。按左上至右下顺序
            单元格列表.Sort();
            // 添加段落
            cell.添加段落(段落);
            // 添加子元素至表格
            AddChild(cell);
        }

        private (double x, double y) 计算单元格坐标(int 行, int 列)
        {
            double x = Left + 边框粗细;
            double y = Top + 边框粗细;
            for (int col = 0; col < 列; col++) x += 全部列宽[col] + 边框粗细;
            for (int row = 0; row < 行; row++) y += 全部行高[row] + 边框粗细;
            return (x, y);
        }

        private 单元格 获取命中单元格(Point point)
        {
            // 先获取命中行
            int rowIndex = -1;
            for (int index = 0; index < 行列表.Count; index++)
            {
                (double y1, double y2) = 计算行区域(index);
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

        private readonly 绘图对象 _表格绘图对象 = new 绘图对象();

        private Brush _borderBrush = new SolidColorBrush(Color.FromRgb(100, 100, 100));
        private Pen _borderPen = new Pen(new SolidColorBrush(Color.FromRgb(100, 100, 100)), 1);

        #endregion
    }
}