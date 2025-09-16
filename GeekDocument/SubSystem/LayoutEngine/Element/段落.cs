using GeekDocument.SubSystem.LayoutEngine.Tool;
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
        public 段落() => 类型 = 元素类型.段落;

        #region 属性

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

        #region 布局元素 方法

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

        #region 公开方法

        /// <summary>
        /// 插入布局元素
        /// </summary>
        public void InsertLayoutElement(布局元素 element, int index)
        {

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
                if (行.行宽 > ActualWidth) ActualWidth = 行.行宽;
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
                子段落 子段落 = new 子段落();
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
                子段落列表.Add(子段落);
                lineIndex++;
            }
        }

        private void 分割行()
        {
            // 注意：此方法仅用于分割行之后计算元素高度，不会根据对齐方式调整元素坐标

            总元素行列表.Clear();
            foreach (var 子段落 in 子段落列表)
            {
                子段落.元素行列表.Clear();
                元素队列 队列 = new 元素队列 { 元素列表 = 子段落.元素列表 };
                while (true)
                {
                    double 行宽 = MaxWidth;
                    if (子段落.元素行列表.Count == 0 && 首行缩进 > 0) 行宽 -= 首行缩进;
                    // 生成行
                    元素行? 行 = 队列.生成元素行(行宽, 水平对齐 == 水平对齐方式.Justify);
                    if (行 != null)
                    {
                        行.更新行高(FontSize);
                        子段落.元素行列表.Add(行);
                        总元素行列表.Add(行);
                    }
                    else break;
                }
                if (子段落.元素行列表.Count == 0)
                {
                    元素行 空行 = new 元素行();
                    空行.更新行高(FontSize);
                    子段落.元素行列表.Add(空行);
                    总元素行列表.Add(空行);
                }
            }
        }

        private void 排列行()
        {
            double y = Top;
            foreach (var 子段落 in 子段落列表)
            {
                int index = 0;
                foreach (var 行 in 子段落.元素行列表)
                {
                    行.Left = Left;
                    if (index == 0 && 首行缩进 > 0) 行.Left += 首行缩进;
                    行.Top = y;
                    y += 行.行高 + LineSpace;
                    index++;
                }
            }
        }

        private void 更新行内布局()
        {
            foreach (var 子段落 in 子段落列表)
            {
                // 单行
                if (子段落.元素行列表.Count == 1)
                {
                    // 只有一行时，如果是两端对齐，改为左对齐
                    元素行 第一行 = 子段落.元素行列表[0];
                    if (水平对齐 == 水平对齐方式.Justify) 第一行.更新元素坐标(水平对齐方式.Left, 垂直对齐);
                    else 第一行.更新元素坐标(水平对齐, 垂直对齐);
                    continue;
                }
                // 多行时，先更新除最后一行
                for (int index = 0; index < 子段落.元素行列表.Count - 1; index++)
                {
                    子段落.元素行列表[index].更新元素坐标(水平对齐, 垂直对齐);
                }
                // 再更新最后一行
                元素行 最后一行 = 子段落.元素行列表.Last();
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

        #endregion

        #region 字段

        private class 子段落
        {
            public List<布局元素> 元素列表 { get; set; } = new List<布局元素>();

            public List<元素行> 元素行列表 { get; set; } = new List<元素行>();
        }

        private List<string> _lineList = new List<string>();
        private List<子段落> 子段落列表 = new List<子段落>();
        private List<元素行> 总元素行列表 = new List<元素行>();

        #endregion
    }
}