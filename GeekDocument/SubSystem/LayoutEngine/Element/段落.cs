using GeekDocument.SubSystem.LayoutEngine.Tool;
using System.Windows.Media;

namespace GeekDocument.SubSystem.LayoutEngine.Element
{
    public class 段落 : 布局元素
    {
        public 段落() => 类型 = 元素类型.段落;

        #region 属性

        public string Text { get; set; } = "";

        public double FontSize { get; set; } = 16;

        public double 首行缩进 { get; set; } = 32;

        /// <summary>行间距</summary>
        public double LineSpace { get; set; } = 4;

        #endregion

        #region 布局元素 方法

        public override void Init()
        {
            Text = Text.Replace("\r\n", "\n");
            _lineList = Text.Split('\n').ToList();
        }

        public override void UpdateLayout()
        {
            ActualWidth = MaxWidth;
            生成布局元素();
            更新元素布局();
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

        #endregion

        #region 私有方法

        private void 生成布局元素()
        {
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
                        字号列表 = new List<double> { FontSize },
                    };
                    element.Init();
                    element.UpdateLayout();
                    子段落.元素列表.Add(element);
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
            }
        }

        private void 更新元素布局()
        {
            更新行列表();
            更新行内布局();
            更新高度();
        }

        private void 更新行列表()
        {
            // 行起始纵坐标
            double y = Top;
            // 遍历子段落
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
                        // 设置行坐标
                        行.Left = Left;
                        if (子段落.元素行列表.Count == 1 && 首行缩进 > 0) 行.Left += 首行缩进;
                        行.Top = y;
                        // 更新下一行纵坐标
                        y += 行.行高 + LineSpace;
                    }
                    else break;
                }
                if (子段落.元素行列表.Count == 0)
                {
                    元素行 空行 = new 元素行();
                    空行.更新行高(FontSize);
                    子段落.元素行列表.Add(空行);
                    总元素行列表.Add(空行);
                    空行.Left = Left + 首行缩进;
                    空行.Top = y;
                    y += 空行.行高 + LineSpace;
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