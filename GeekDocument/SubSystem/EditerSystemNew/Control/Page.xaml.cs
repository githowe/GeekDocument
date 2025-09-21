using GeekDocument.SubSystem.EditerSystemNew.Core.Layer;
using GeekDocument.SubSystem.EditerSystemNew.Define;
using GeekDocument.SubSystem.LayoutEngine;
using GeekDocument.SubSystem.LayoutEngine.Element;
using GeekDocument.SubSystem.LayoutEngine.Tool;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace GeekDocument.SubSystem.EditerSystemNew.Control
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
            // 初始化图层、工具
            Initlayer();
            _tool = new EditTool(this);
            _tool.Init();
            // 初始化光标定时器
            _blinkTimer.Interval = TimeSpan.FromMilliseconds(500);
            _blinkTimer.Tick += BlinkTimer_Tick;
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
                y += item.段前距;
                // 设置坐标与宽度
                item.BlockLeft = x;
                item.BlockTop = y;
                item.TopOffset = y - PagePadding.Top;
                item.BlockWidth = blockWidth;
                // 更新布局
                item.UpdateBlockLayout();
                // 更新纵坐标
                y += item.BlockHeight + item.段后距 + BlockInterval;
            }
            // 更新高度
            Paper.Height = 0;
            foreach (var item in BlockList)
            {
                Paper.Height += item.段前距;
                Paper.Height += item.BlockHeight;
                Paper.Height += item.段后距;
            }
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
        /// 初始化光标
        /// </summary>
        public void InitCaret()
        {
            MoveCaretByPoint(new Point());
            StartBlinkIBeam();
        }

        /// <summary>
        /// 更新悬停元素视图
        /// </summary>
        public void UpdateHoverElementView(IDocumentElement? element)
        {
            if (element == null)
            {
                _hoverBoxLayer.RectList.Clear();
                _hoverBoxLayer.Clear();
                return;
            }
            _hoverBoxLayer.UpdateRect(element.GetViewRect());
            _hoverBoxLayer.Update();
        }

        /// <summary>
        /// 更新命中元素视图
        /// </summary>
        public void UpdateHitedElementView(IDocumentElement? element)
        {
            if (element == null)
            {
                _hitBoxLayer.HitedRect = Rect.Empty;
                _hitBoxLayer.Clear();
                return;
            }
            _hitBoxLayer.HitedRect = element.GetViewRect();
            _hitBoxLayer.Update();
        }

        /// <summary>
        /// 开始闪烁光标
        /// </summary>
        public void StartBlinkIBeam()
        {
            _ibeamVisible = true;
            _caretLayer.Update();
            _blinkTimer.Start();
        }

        /// <summary>
        /// 停止闪烁光标
        /// </summary>
        public void StopBlinkIBeam()
        {
            _blinkTimer.Stop();
            _ibeamVisible = true;
            _caretLayer.Update();
        }

        #endregion

        #region 工具方法

        public void HandleMouseMove()
        {
            // 获取相对于页面起始点的坐标
            Point mousePoint = GetMousePoint();
            // 反向遍历段落块，找到悬停的元素
            _hoveredElement = null;
            for (int index = BlockList.Count - 1; index >= 0; index--)
            {
                段落块 block = BlockList[index];
                _hoveredElement = block.段落.GetHitedElement(mousePoint);
                if (_hoveredElement != null) break;
                if (block.段落.GetHitTestRect().Contains(mousePoint))
                {
                    _hoveredElement = block.段落;
                    break;
                }
            }
            // 更新悬停元素
            UpdateHoverElementView(_hoveredElement);
        }

        public void HandleMouseDown()
        {
            Point point = GetMousePoint();
            // 更新命中行
            UpdateHitedLine(point);
            // 移动光标至鼠标位置
            MoveCaretByPoint(point);
        }

        #endregion

        #region 控件事件

        private void InteractionLayer_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _tool.OnMouseMove();
        }

        private void InteractionLayer_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _tool.OnMouseDown(e.ChangedButton);
        }

        private void InteractionLayer_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _tool.OnMouseUp(e.ChangedButton);
        }

        #endregion

        #region 私有方法

        private void Initlayer()
        {
            _hoverBoxLayer = new HoverBoxLayer();
            _hoverBoxLayer.Init();
            MarkBox.Children.Add(_hoverBoxLayer);
            Canvas.SetLeft(_hoverBoxLayer, PagePadding.Left);
            Canvas.SetTop(_hoverBoxLayer, PagePadding.Top);

            _hitBoxLayer = new HitBoxLayer();
            _hitBoxLayer.Init();
            MarkBox.Children.Add(_hitBoxLayer);
            Canvas.SetLeft(_hitBoxLayer, PagePadding.Left);
            Canvas.SetTop(_hitBoxLayer, PagePadding.Top);

            _inputBoxLayer = new InputBoxLayer();
            _inputBoxLayer.Init();
            MarkBox.Children.Add(_inputBoxLayer);
            Canvas.SetLeft(_inputBoxLayer, PagePadding.Left);
            Canvas.SetTop(_inputBoxLayer, PagePadding.Top);

            _caretLayer = new CaretLayer();
            _caretLayer.Init();
            MarkBox.Children.Add(_caretLayer);
            Canvas.SetLeft(_caretLayer, PagePadding.Left);
            Canvas.SetTop(_caretLayer, PagePadding.Top);
        }

        private Point GetMousePoint()
        {
            Point mousePoint = Mouse.GetPosition(InteractionLayer);
            mousePoint.X -= PagePadding.Left;
            mousePoint.Y -= PagePadding.Top;
            return mousePoint;
        }

        /// <summary>
        /// 更新命中行
        /// </summary>
        private void UpdateHitedLine(Point point)
        {
            段落 命中段落 = GetHitedParagraph(point);
            _inputBoxLayer.Line = 命中段落.GetHitedLine(point);
            _inputBoxLayer.Update();
        }

        /// <summary>
        /// 根据坐标移动光标
        /// </summary>
        private void MoveCaretByPoint(Point point)
        {
            _hitedElement = GetHitedParagraph(point);
            CaretInfo info = _hitedElement.MoveCaret(point);
            _caretLayer.CaretX = info.X;
            _caretLayer.CaretY = info.Y;
            _caretLayer.CaretHeight = info.Height;
            _caretLayer.Update();
        }

        /// <summary>
        /// 获取命中段落
        /// </summary>
        private 段落 GetHitedParagraph(Point point)
        {
            段落? 命中段落 = null;

            // 首先通过垂直坐标找到命中元素，当块重叠时，优先命中最上层的块
            for (int index = BlockList.Count - 1; index >= 0; index--)
            {
                段落块 block = BlockList[index];
                // 获取块的视图区域
                Rect viewRect = block.段落.GetViewRect();
                if (point.Y >= viewRect.Top && point.Y < viewRect.Bottom)
                {
                    命中段落 = block.段落;
                    break;
                }
            }
            // 块之间有间距时，会无法命中块，此时通过最近距离找到命中元素
            if (命中段落 == null)
            {
                double 当前距离 = double.MaxValue;
                命中段落 = BlockList[0].段落;
                foreach (var block in BlockList)
                {
                    段落 段落 = block.段落;
                    Rect viewRect = 段落.GetViewRect();
                    double 距离 = Math.Min(Math.Abs(point.Y - viewRect.Top), Math.Abs(point.Y - viewRect.Bottom));
                    if (距离 < 当前距离)
                    {
                        当前距离 = 距离;
                        命中段落 = 段落;
                    }
                }
            }

            return 命中段落;
        }

        private void BlinkTimer_Tick(object? sender, EventArgs e)
        {
            if (_ibeamVisible) _caretLayer.Clear();
            else _caretLayer.Update();
            _ibeamVisible = !_ibeamVisible;
        }

        #endregion

        #region 字段

        private readonly double _markSize = 24;

        private HoverBoxLayer _hoverBoxLayer;
        private HitBoxLayer _hitBoxLayer;
        private InputBoxLayer _inputBoxLayer;
        private CaretLayer _caretLayer;

        private EditTool _tool;

        /// <summary>当前悬停元素</summary>
        private IDocumentElement? _hoveredElement = null;
        /// <summary>当前命中元素</summary>
        private IDocumentElement? _hitedElement = null;

        private 元素行? _currentLine = null;

        /// <summary>光标可见性，用于闪烁光标</summary>
        private bool _ibeamVisible = true;
        private readonly DispatcherTimer _blinkTimer = new DispatcherTimer();

        #endregion
    }
}