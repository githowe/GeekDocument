using GeekDocument.SubSystem.EditerSystemNew.Core.Layer;
using System.Windows;
using System.Windows.Controls;

namespace GeekDocument.SubSystem.LayoutEngine
{
    /// <summary>
    /// 页面。从上至下排列段落
    /// </summary>
    public partial class Page : UserControl
    {
        #region 构造方法

        public Page() => InitializeComponent();

        #endregion

        #region 属性

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

        public List<段落块> BlockList { get; set; } = new List<段落块>();

        #endregion

        #region 公开方法

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
            // 添加段落中的图层至画布
            foreach (var 块 in BlockList)
            {
                块.InitLayer();
                foreach (var layer in 块.LayerList) BlockBox.Children.Add(layer);
            }
            Initlayer();
        }

        public void 更新页面()
        {
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
                item.TopOffset = y - PagePadding.Top;
                item.BlockWidth = blockWidth;
                // 更新布局
                item.UpdateBlockLayout();
                // 更新纵坐标
                y += item.BlockHeight + BlockInterval;
            }
            // 更新高度
            Paper.Height = 0;
            foreach (var item in BlockList) Paper.Height += item.BlockHeight;
            Paper.Height += (BlockList.Count - 1) * BlockInterval;
            Paper.Height += PagePadding.Top + PagePadding.Bottom;
            // 更新块坐标
            foreach (var block in BlockList)
            {
                foreach (var layer in block.LayerList)
                {
                    Canvas.SetLeft(layer, block.BlockLeft);
                    Canvas.SetTop(layer, block.BlockTop);
                }
            }
            // 绘制块
            foreach (var item in BlockList) item.Update();
        }

        /// <summary>
        /// 更新悬停元素
        /// </summary>
        public void UpdateHoverElement(IDocumentElement? element)
        {
            if (element == null)
            {
                _layer.RectList.Clear();
                _layer.Clear();
                return;
            }
            _layer.UpdateRect(element.GetElementRect());
            _layer.Update();
        }

        #endregion

        #region 私有方法

        private void Initlayer()
        {
            _layer = new ElementBoxLayer();
            _layer.Init();
            MarkBox.Children.Add(_layer);
            Canvas.SetLeft(_layer, PagePadding.Left);
            Canvas.SetTop(_layer, PagePadding.Top);
        }

        #endregion

        #region 字段

        private readonly double _markSize = 24;

        private ElementBoxLayer _layer;

        #endregion
    }
}