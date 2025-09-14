using GeekDocument.SubSystem.LayoutEngine.Tool;
using System.Windows.Media;

namespace GeekDocument.SubSystem.LayoutEngine.Element
{
    public class 段落 : 布局元素
    {
        #region 属性

        public string Text { get; set; } = "";

        public double FontSize { get; set; } = 16;

        public double 首行缩进 { get; set; } = 32;

        public double 左缩进 { get; set; } = 0;

        public double 右缩进 { get; set; } = 0;

        /// <summary>行间距</summary>
        public double LineSpace { get; set; } = 4;

        public bool 内嵌一个文本 { get; set; } = false;

        #endregion

        #region 布局元素 方法

        public override void Init()
        {
            _index_01 = new Random().Next(0, Text.Length);
            _index_02 = new Random().Next(0, Text.Length);
        }

        public override void UpdateLayout()
        {
            ActualWidth = MaxWidth;
            生成布局元素();
            更新元素布局();
        }

        public override void 绘图(DrawingContext dc, double left, double top)
        {
            double y = top;
            int index = 0;
            foreach (var 行 in 行列表)
            {
                double x = left;
                if (index == 0) x = left + 首行缩进;
                foreach (var 元素 in 行.元素列表)
                    元素.绘图(dc, x + 元素.Left, y + 元素.Top);
                y += 行.行高 + LineSpace;
                index++;
            }
        }

        #endregion

        #region 私有方法

        private void 生成布局元素()
        {
            List<布局元素> 元素列表 = new List<布局元素>();
            队列 = new 元素队列 { 元素列表 = 元素列表 };
            // 每个字符生成一个字元素
            foreach (var c in Text)
            {
                字 element = new 字
                {
                    文本 = c.ToString(),
                    字号列表 = new List<double> { FontSize },
                };
                element.Init();
                element.UpdateLayout();
                元素列表.Add(element);
            }
            // 添加一个图片元素
            图片 imgElement = new 图片
            {
                MaxWidth = 64,
                MaxHeight = 64,
            };
            imgElement.Init();
            imgElement.UpdateLayout();
            if (Text.Length > 0) 元素列表.Insert(_index_01, imgElement);
            // 嵌套一个文本元素
            if (内嵌一个文本)
            {
                段落 nestedElement = new 段落
                {
                    Text = "  被称为宁老 先生的是一个  名为宁擒水的老人，老人年逾古稀，头发花白",
                    FontSize = FontSize - 4,
                    首行缩进 = 0,
                    MaxWidth = 160,
                    水平对齐 = 水平对齐方式.Center,
                };
                nestedElement.Init();
                nestedElement.UpdateLayout();
                元素列表.Insert(_index_02, nestedElement);
            }
            // 添加中英文间距
            布局元素? 当前;
            布局元素? 下一个;
            for (int index = 0; index < 元素列表.Count - 1; index++)
            {
                当前 = 元素列表[index];
                下一个 = 元素列表[index + 1];
                // 两个元素都为字时，需要添加字间距
                if (当前 is 字 当前字 && 下一个 is 字 下一个字)
                {
                    if (当前字.类型 == 字类型.Chinese && 下一个字.类型 == 字类型.English ||
                        当前字.类型 == 字类型.English && 下一个字.类型 == 字类型.Chinese)
                    {
                        当前字.RightMargin = 当前字.最后一个字宽() * 0.25;
                    }
                }
            }
        }

        private void 更新元素布局()
        {
            更新行列表();
            更新高度();
        }

        private void 更新行列表()
        {
            行列表.Clear();
            while (true)
            {
                double 行宽 = MaxWidth - 左缩进 - 右缩进;
                if (行列表.Count == 0 && 首行缩进 > 0) 行宽 -= 首行缩进;
                // 生成行
                元素行? 行 = 队列.生成元素行(行宽, 水平对齐);
                if (行 != null)
                {
                    行.更新行高(FontSize);
                    行列表.Add(行);
                }
                else break;
            }
            // 没有生成任何行时，添加一个空行
            if (行列表.Count == 0)
            {
                元素行 空行 = new 元素行();
                空行.更新行高(FontSize);
                行列表.Add(空行);
                return;
            }
            更新行内布局();
        }

        private void 更新高度()
        {
            ActualHeight = 0;
            foreach (var item in 行列表) ActualHeight += item.行高;
            ActualHeight += LineSpace * (行列表.Count - 1);
        }

        private void 更新行内布局()
        {
            // 单行
            if (行列表.Count == 1)
            {
                // 只有一行时，如果是两端对齐，改为左对齐
                元素行 第一行 = 行列表[0];
                if (水平对齐 == 水平对齐方式.Justify) 第一行.更新元素坐标(水平对齐方式.Left, 垂直对齐);
                else 第一行.更新元素坐标(水平对齐, 垂直对齐);
                return;
            }
            // 多行时，先更新除最后一行
            for (int index = 0; index < 行列表.Count - 1; index++)
            {
                行列表[index].更新元素坐标(水平对齐, 垂直对齐);
            }
            // 再更新最后一行
            元素行 最后一行 = 行列表.Last();
            if (水平对齐 == 水平对齐方式.Justify) 最后一行.更新元素坐标(水平对齐方式.Left, 垂直对齐);
            else 最后一行.更新元素坐标(水平对齐, 垂直对齐);
        }

        #endregion

        #region 字段

        private 元素队列 队列 = new 元素队列();
        private List<元素行> 行列表 = new List<元素行>();

        private int _index_01 = 0;
        private int _index_02 = 0;

        #endregion
    }
}