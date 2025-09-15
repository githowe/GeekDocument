using System.Windows;
using System.Windows.Controls;

namespace GeekDocument.SubSystem.LayoutEngine
{
    /// <summary>
    /// 页面。从上至下排列块元素
    /// </summary>
    public partial class Page : UserControl
    {
        public Page() => InitializeComponent();

        /// <summary>内边距</summary>
        public Thickness PagePadding { get; set; } = new Thickness(0);

        /// <summary>块间距</summary>
        public double BlockInterval { get; set; } = 16;

        /// <summary>首行缩进</summary>
        public int FirstLineIndent { get; set; } = 0;

        /// <summary>默认正文字体</summary>
        public string TextFont { get; set; } = "霞鹜文楷";

        /// <summary>默认正文字号。单位：像素</summary>
        public int TextSize { get; set; } = 16;

        /// <summary>块元素列表</summary>
        public List<块元素> BlockList { get; set; } = new List<块元素>();

        /// <summary>浮动元素列表</summary>
        public List<浮动元素> FloatList { get; set; } = new List<浮动元素>();

        public void Init()
        {
            // 更新边距标记
            double leftMargin = PagePadding.Left - _markSize;
            double topMargin = PagePadding.Top - _markSize;
            double rightMargin = PagePadding.Right - _markSize;
            double bottomMargin = PagePadding.Bottom - _markSize;
            Mark_01.Margin = new Thickness(leftMargin, topMargin, 0, 0);
            Mark_02.Margin = new Thickness(0, topMargin, rightMargin, 0);
            Mark_03.Margin = new Thickness(leftMargin, 0, 0, bottomMargin);
            Mark_04.Margin = new Thickness(0, 0, rightMargin, bottomMargin);
            // 添加块至画布
            foreach (var block in BlockList) BlockBox.Children.Add(block);
        }

        public void 更新页面()
        {
            // 先更新浮动元素布局，以计算浮动元素占用区域
            foreach (var item in FloatList) item.UpdateLayout();
            // 确定起始坐标与块宽度
            double x = PagePadding.Left;
            double y = PagePadding.Top;
            double blockWidth = Width - PagePadding.Left - PagePadding.Right;
            // 遍历块
            foreach (var item in BlockList)
            {
                // 设置坐标与宽度
                item.BlockLeft = x;
                item.BlockTop = y;
                item.BlockWidth = blockWidth;
                // 设置空白区域
                item.SpaceRectList = 获取重叠区域(item.BlockLeft, item.BlockTop, item.BlockLeft + item.BlockWidth);
                // 更新布局
                item.UpdateElementLayout();
                // 更新纵坐标
                y += item.BlockHeight + BlockInterval;
            }
            // 更新高度
            Height = 0;
            foreach (var item in BlockList) Height += item.BlockHeight;
            Height += (BlockList.Count - 1) * BlockInterval;
            Height += PagePadding.Top + PagePadding.Bottom;
            // 更新块坐标
            foreach (var block in BlockList)
            {
                Canvas.SetLeft(block, block.BlockLeft);
                Canvas.SetTop(block, block.BlockTop);
            }
            // 绘制块
            foreach (var item in BlockList) item.Update();
        }

        private List<Rect> 获取重叠区域(double left, double top, double right)
        {
            List<Rect> result = new List<Rect>();
            foreach (var item in FloatList)
            {
                if (item.Left + item.Width <= left) continue;
                if (item.Top + item.Height <= top) continue;
                if (item.Left >= right) continue;
                result.Add(new Rect(item.Left, item.Top, item.Width, item.Height));
            }
            return result;
        }

        private readonly double _markSize = 24;
    }
}