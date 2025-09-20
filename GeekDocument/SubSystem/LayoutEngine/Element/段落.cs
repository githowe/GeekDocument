using GeekDocument.SubSystem.LayoutEngine.Tool;
using System.Windows;
using System.Windows.Media;

namespace GeekDocument.SubSystem.LayoutEngine.Element
{
    public class 段落内嵌元素
    {
        public int LineIndex { get; set; } = 0;

        public int CharIndex { get; set; } = 0;

        public List<布局元素> ElementList { get; set; } = new List<布局元素>();
    }

    public class 段落 : 布局元素
    {
        #region 构造方法

        public 段落() => 类型 = 元素类型.段落;

        #endregion

        #region 属性

        /// <summary>此偏移相对于第一个段落的纵坐标</summary>
        public double 段落偏移 { get; set; } = 0;

        public double Width { get; set; } = 0;

        public string Text { get; set; } = "";

        public string Font { get; set; } = "霞鹜文楷";

        public double FontSize { get; set; } = 16;

        public double 首行缩进 { get; set; } = 32;

        /// <summary>行间距</summary>
        public double LineSpace { get; set; } = 4;

        public List<段落内嵌元素> 内嵌元素列表 { get; set; } = new List<段落内嵌元素>();

        /// <summary>纯文本模式。该模式只允许包含字元素</summary>
        public bool PlainText { get; set; } = false;

        #endregion

        #region 布局元素方法

        public override List<ElementLayer> GetLayerList()
        {
            List<ElementLayer> result = new List<ElementLayer>();

            foreach (var 内嵌元素 in 内嵌元素列表)
                foreach (var 元素 in 内嵌元素.ElementList)
                    result.AddRange(元素.GetLayerList());

            return result;
        }

        public override void Measure()
        {
            // 没有设置最大宽度时，不符合规范，直接抛异常
            if (double.IsNaN(MaxWidth)) throw new Exception("段落未设置最大宽度");
            // 小于或等于零视为无限宽度
            if (MaxWidth <= 0) MaxWidth = double.PositiveInfinity;
            // 设置实际宽度为最大宽度
            ActualWidth = MaxWidth;

            计算内嵌元素大小();
            生成字元素();
            分割行();
            更新高度();

            // 如果实际宽度为无限，需更新为内容实际宽度
            if (double.IsPositiveInfinity(ActualWidth))
            {
                ActualWidth = 0;
                foreach (var item in 总元素行列表)
                {
                    double lineWidth = item.获取实际宽度();
                    if (item.首行) lineWidth += 首行缩进;
                    if (lineWidth > ActualWidth) ActualWidth = lineWidth;
                }
            }
        }

        public override void Arrange()
        {
            排列行();
            更新行内布局();
        }

        public override void 绘图(DrawingContext dc)
        {
            foreach (var 行 in 总元素行列表)
                foreach (var 元素 in 行.元素列表)
                    元素.绘图(dc);
        }

        #endregion

        #region IDocumentElement 成员

        public override string Icon { get; set; } = "Paragraph";

        public override string Name { get; set; } = "段落";

        public override bool CanInput => true;

        public override List<IDocumentElement> GetSubElementList()
        {
            List<IDocumentElement> result = new List<IDocumentElement>();
            int index = 0;
            foreach (var 行 in 行列表)
            {
                foreach (var 元素行 in 行.元素行列表)
                {
                    元素行.Name = $"行_{index + 1:00}";
                    result.Add(元素行);
                    index++;
                }
            }
            return result;
        }

        public override IDocumentElement GetNearestElement(Point point)
        {
            IDocumentElement? 间接命中 = null;
            List<元素行> 元素行列表 = GetElementLineList();
            // 先通过纵纵坐标获取命中行
            for (int index = 元素行列表.Count - 1; index >= 0; index--)
            {
                元素行 元素行 = 元素行列表[index];
                Rect viewRect = 元素行.GetViewRect();
                double y = point.Y;
                if (y >= viewRect.Top && y < viewRect.Bottom)
                {
                    间接命中 = 元素行;
                    break;
                }
            }
            // 无命中，说明行之间有间隔，此时通过距离找到最近行
            if (间接命中 == null)
            {
                间接命中 = 元素行列表[0];
                double 最小距离 = double.MaxValue;
                foreach (var 元素行 in 元素行列表)
                {
                    Rect viewRect = 元素行.GetViewRect();
                    double distance = Math.Min(Math.Abs(point.Y - viewRect.Top), Math.Abs(point.Y - viewRect.Bottom));
                    if (distance < 最小距离)
                    {
                        最小距离 = distance;
                        间接命中 = 元素行;
                    }
                }
            }
            // 此时，间接命中行一定不为空，继续查找行内最近元素
            return 间接命中.GetNearestElement(point);
        }

        public override CaretInfo MoveCaret(Point point)
        {
            IDocumentElement? 间接命中 = null;
            List<元素行> 元素行列表 = GetElementLineList();
            // 先通过纵纵坐标获取命中行
            for (int index = 元素行列表.Count - 1; index >= 0; index--)
            {
                元素行 元素行 = 元素行列表[index];
                Rect viewRect = 元素行.GetViewRect();
                double y = point.Y;
                if (y >= viewRect.Top && y < viewRect.Bottom)
                {
                    间接命中 = 元素行;
                    break;
                }
            }
            // 无命中，说明行之间有间隔，此时通过距离找到最近行
            if (间接命中 == null)
            {
                间接命中 = 元素行列表[0];
                double 最小距离 = double.MaxValue;
                foreach (var 元素行 in 元素行列表)
                {
                    Rect viewRect = 元素行.GetViewRect();
                    double distance = Math.Min(Math.Abs(point.Y - viewRect.Top), Math.Abs(point.Y - viewRect.Bottom));
                    if (distance < 最小距离)
                    {
                        最小距离 = distance;
                        间接命中 = 元素行;
                    }
                }
            }
            return 间接命中.MoveCaret(point);
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 添加布局元素
        /// </summary>
        public void AddLayoutElement(布局元素 element)
        {
            if (行列表.Count == 0)
            {
                InsertLayoutElement(element, 0, 0);
                return;
            }
            int lineIndex = 行列表.Count - 1;
            int charIndex = 行列表[lineIndex].Length;
            InsertLayoutElement(element, lineIndex, charIndex);
        }

        /// <summary>
        /// 插入布局元素
        /// </summary>
        public void InsertLayoutElement(布局元素 element, int lineIndex, int charIndex)
        {
            // 查找内嵌元素
            段落内嵌元素? 内嵌元素 = null;
            foreach (var item in 内嵌元素列表)
            {
                if (item.LineIndex == lineIndex && item.CharIndex == charIndex)
                {
                    内嵌元素 = item;
                    break;
                }
            }
            // 没有找到时，创建新的内嵌元素
            if (内嵌元素 == null)
            {
                内嵌元素 = new 段落内嵌元素
                {
                    LineIndex = lineIndex,
                    CharIndex = charIndex,
                };
                内嵌元素列表.Add(内嵌元素);
            }
            // 插入元素
            内嵌元素.ElementList.Insert(0, element);
        }

        /// <summary>
        /// 适配内容实际宽度
        /// </summary>
        public void FitContentWidth()
        {
            ActualWidth = 0;
            foreach (var 行 in 总元素行列表)
            {
                行.行宽 = 行.获取实际宽度();
                double 总宽 = 行.行宽;
                if (行.首行) 总宽 += 首行缩进;
                if (总宽 > ActualWidth) ActualWidth = 总宽;
            }
        }

        #endregion

        #region 私有方法

        private void 计算内嵌元素大小()
        {
            foreach (var 内嵌元素 in 内嵌元素列表)
                foreach (var element in 内嵌元素.ElementList)
                {
                    element.Parent = this;

                    // 内嵌元素没有设置最大宽度时，设置为段落最大宽度
                    if (double.IsNaN(element.MaxWidth)) element.MaxWidth = MaxWidth;
                    // 或者最大宽度大于段落最大宽度时，设置为段落最大宽度
                    else if (element.MaxWidth > MaxWidth) element.MaxWidth = MaxWidth;

                    // 如果设置了最大高度，则限定最大高度
                    if (!double.IsNaN(MaxHeight)) element.MaxHeight = MaxHeight;

                    element.Measure();
                }
        }

        private void 生成字元素()
        {
            Text = Text.Replace("\r\n", "\n");
            _lineList = Text.Split('\n').ToList();
            int lineIndex = 0;
            foreach (var line in _lineList)
            {
                行 子段落 = new 行();
                // 遍历字
                foreach (var c in line)
                {
                    字 element = new 字
                    {
                        Parent = this,
                        文本 = c.ToString(),
                        字体列表 = new List<string> { Font },
                        字号列表 = new List<double> { FontSize },
                    };
                    element.Init();
                    element.Measure();
                    子段落.元素列表.Add(element);
                }
                // 插入内嵌元素
                foreach (var 内嵌元素 in 内嵌元素列表)
                {
                    if (内嵌元素.LineIndex != lineIndex) continue;
                    子段落.元素列表.InsertRange(内嵌元素.CharIndex, 内嵌元素.ElementList);
                }
                // 添加中英文间距
                布局元素? 当前;
                布局元素? 下一个;
                for (int index = 0; index < 子段落.元素列表.Count - 1; index++)
                {
                    当前 = 子段落.元素列表[index];
                    下一个 = 子段落.元素列表[index + 1];
                    // 两个元素都为字时，需要添加字间距
                    if (当前 is 字 当前字 && 下一个 is 字 下一个字)
                    {
                        if (当前字.字类型 == 字类型.Chinese && 下一个字.字类型 == 字类型.English ||
                            当前字.字类型 == 字类型.English && 下一个字.字类型 == 字类型.Chinese)
                            当前字.RightMargin = 当前字.最后一个字宽() * 0.25;
                    }
                }
                // 添加子段落
                行列表.Add(子段落);
                lineIndex++;
            }
        }

        private void 分割行()
        {
            // 注意：此方法仅用于分割行之后计算元素高度，不会根据对齐方式调整元素坐标

            总元素行列表.Clear();
            foreach (var 行 in 行列表)
            {
                行.元素行列表.Clear();
                元素队列 队列 = new 元素队列 { 元素列表 = 行.元素列表 };
                while (true)
                {
                    double 行宽 = MaxWidth;
                    if (行.元素行列表.Count == 0 && 首行缩进 > 0) 行宽 -= 首行缩进;
                    // 生成行
                    元素行? 元素行 = 队列.生成元素行(行宽, 行.元素行列表.Count == 0, 水平对齐 == 水平对齐方式.Justify);
                    if (元素行 != null)
                    {
                        元素行.Owner = this;
                        元素行.更新行高(FontSize);
                        行.元素行列表.Add(元素行);
                        总元素行列表.Add(元素行);
                    }
                    else break;
                }
                if (行.元素行列表.Count == 0)
                {
                    元素行 空行 = new 元素行();
                    空行.Owner = this;
                    空行.更新行高(FontSize);
                    行.元素行列表.Add(空行);
                    总元素行列表.Add(空行);
                }
            }
        }

        private void 排列行()
        {
            double y = Top;
            foreach (var 行 in 行列表)
            {
                int index = 0;
                foreach (var 元素行 in 行.元素行列表)
                {
                    元素行.Left = Left;
                    if (index == 0 && 首行缩进 > 0) 元素行.Left += 首行缩进;
                    元素行.Top = y;
                    y += 元素行.行高 + LineSpace;
                    index++;
                }
            }
        }

        private void 更新行内布局()
        {
            foreach (var 行 in 行列表)
            {
                // 单行
                if (行.元素行列表.Count == 1)
                {
                    // 只有一行时，如果是两端对齐，改为左对齐
                    元素行 第一行 = 行.元素行列表[0];
                    if (水平对齐 == 水平对齐方式.Justify) 第一行.更新元素坐标(水平对齐方式.Left, 垂直对齐);
                    else 第一行.更新元素坐标(水平对齐, 垂直对齐);
                    continue;
                }
                // 多行时，先更新除最后一行
                for (int index = 0; index < 行.元素行列表.Count - 1; index++)
                {
                    行.元素行列表[index].更新元素坐标(水平对齐, 垂直对齐);
                }
                // 再更新最后一行
                元素行 最后一行 = 行.元素行列表.Last();
                if (水平对齐 == 水平对齐方式.Justify) 最后一行.更新元素坐标(水平对齐方式.Left, 垂直对齐);
                else 最后一行.更新元素坐标(水平对齐, 垂直对齐);
            }
        }

        private void 更新高度()
        {
            ActualHeight = 0;
            foreach (var item in 总元素行列表) ActualHeight += item.行高;
            ActualHeight += LineSpace * (总元素行列表.Count - 1);
        }

        private List<元素行> GetElementLineList()
        {
            List<元素行> result = new List<元素行>();
            foreach (var 行 in 行列表)
                foreach (var 元素行 in 行.元素行列表)
                    result.Add(元素行);
            return result;
        }

        #endregion

        #region 字段

        private class 行
        {
            public List<布局元素> 元素列表 { get; set; } = new List<布局元素>();

            public List<元素行> 元素行列表 { get; set; } = new List<元素行>();

            public int Length => 元素列表.Count;
        }

        private List<string> _lineList = new List<string>();
        private readonly List<行> 行列表 = new List<行>();
        private readonly List<元素行> 总元素行列表 = new List<元素行>();

        #endregion
    }
}