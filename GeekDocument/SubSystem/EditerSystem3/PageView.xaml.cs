using GeekDocument.SubSystem.EditerSystem.Define;
using GeekDocument.SubSystem.EditerSystem3.Layer;
using GeekDocument.SubSystem.ImageSystem;
using GeekDocument.SubSystem.LayoutEngine;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using XLogic.Wpf.Drawing;

namespace GeekDocument.SubSystem.EditerSystem3
{
    public partial class PageView : UserControl
    {
        #region 构造方法

        public PageView() => InitializeComponent();

        #endregion

        #region 属性

        public Editer OwnerEditer { get; set; } = null!;

        /// <summary>内边距</summary>
        public Thickness PagePadding { get; set; } = new Thickness(0);

        public double PageHeight { get; set; } = 0;

        #endregion

        #region 事件

        public Action<段落>? 当前段落变化 { get; set; } = null;

        #endregion

        #region 公开方法

        public void Init(页面 page)
        {
            _page = page;
            // 更新边距标记
            double leftMargin = PagePadding.Left - _markSize;
            double topMargin = PagePadding.Top - _markSize;
            double rightMargin = PagePadding.Right - _markSize;
            double bottomMargin = PagePadding.Bottom - _markSize;
            Mark_01.Margin = new Thickness(leftMargin, topMargin, 0, 0);
            Mark_02.Margin = new Thickness(0, topMargin, rightMargin, 0);
            Mark_03.Margin = new Thickness(leftMargin, 0, 0, bottomMargin);
            Mark_04.Margin = new Thickness(0, 0, rightMargin, bottomMargin);
            // 添加页面中的图层
            PageBox.Children.Add(_page.Layer);
            // 设置图层坐标
            Canvas.SetLeft(_page.Layer, PagePadding.Left);
            Canvas.SetTop(_page.Layer, PagePadding.Top);
            // 构建示例页面
            // BuildDemoPage();

            // 添加图层
            _hoveredInfoLayer = AddLayer<HoveredInfoLayer>();
            _hitedInfoLayer = AddLayer<HitedInfoLayer>(false);
            _inputBoxLayer = AddLayer<InputBoxLayer>(false);
            _highlightLayer = AddLayer<HighlightLayer>();
            _caretLayer = AddLayer<CaretLayer>();
            // 初始化编辑工具
            _tool = new EditTool(this);
            _tool.Init();

            // 初始化光标定时器
            _blinkTimer.Interval = TimeSpan.FromMilliseconds(500);
            _blinkTimer.Tick += BlinkTimer_Tick;

            _editKeyDict.Add(Key.Up, EditKey.Up);
            _editKeyDict.Add(Key.Down, EditKey.Down);
            _editKeyDict.Add(Key.Left, EditKey.Left);
            _editKeyDict.Add(Key.Right, EditKey.Right);
            _editKeyDict.Add(Key.Home, EditKey.Home);
            _editKeyDict.Add(Key.End, EditKey.End);
            _editKeyDict.Add(Key.Back, EditKey.Backspace);
            _editKeyDict.Add(Key.Delete, EditKey.Delete);
            _editKeyDict.Add(Key.Enter, EditKey.Enter);
            _ctrlEditKeyList.Add(Key.A);
            _ctrlEditKeyList.Add(Key.X);
            _ctrlEditKeyList.Add(Key.C);
            _ctrlEditKeyList.Add(Key.V);
            _ctrlEditKeyList.Add(Key.Z);
            _ctrlEditKeyList.Add(Key.Y);
            _ctrlEditKeyList.Add(Key.S);
            _ctrlEditKeyList.Add(Key.Enter);
        }

        public void 更新页面()
        {
            _page.测量();
            Paper.Height = _page.页高 + PagePadding.Top + PagePadding.Bottom;
            _page.排列();
            _page.渲染();
        }

        public void InitEditSystem()
        {
            // 监听页面事件
            _page.高度变化 += 页面_高度变化;
            _page.当前段落变化 += 页面_当前段落变化;
            _page.当前行变化 += 页面_当前行变化;
            _page.光标移动 += 页面_光标移动;
            _page.高亮元素变化 += 页面_高亮元素变化;
            // 移动光标至页面开始位置
            _page.段落列表[0].移动光标至开头();
            // 开始闪烁光标
            StartBlinkIBeam();
            // 监听交互图层的鼠标事件
            InteractionLayer.MouseMove += InteractionLayer_MouseMove;
            InteractionLayer.MouseDown += InteractionLayer_MouseDown;
            InteractionLayer.MouseUp += InteractionLayer_MouseUp;
            InteractionLayer.MouseWheel += InteractionLayer_MouseWheel;
        }

        public void UpdateHoveredElement(布局元素? 元素)
        {
            _hoveredInfoLayer.HoveredElement = 元素;
            _hoveredInfoLayer.Update();
        }

        /// <summary>
        /// 处理按键按下
        /// </summary>
        public void HandleKeyDown(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                if (_editKeyDict.TryGetValue(e.Key, out EditKey editKey))
                {
                    StopBlinkIBeam();
                    _currentLine?.HandleEditKey(editKey);
                    StartBlinkIBeam();
                    e.Handled = true;
                }
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (_ctrlEditKeyList.Contains(e.Key))
                {
                    StopBlinkIBeam();
                    _currentLine?.HandleCtrlEditKey(e.Key);
                    StartBlinkIBeam();
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// 处理文本输入
        /// </summary>
        public void HandleTextInput(string text)
        {
            StopBlinkIBeam();
            ClearHighlight();
            _currentLine?.HandleTextInput(text);
            StartBlinkIBeam();
        }

        public void 插入图片(List<图片> list)
        {
            _currentLine.插入图片(list);
        }

        public void 插入表格(表格 table)
        {
            _currentLine.插入表格(table);
        }

        public void 插入公式(公式 公式)
        {
            _currentLine.插入公式(公式);
        }

        #endregion

        #region 工具方法

        public string 获取命中区域()
        {
            Point point = GetMousePoint();
            命中信息? info = _page.获取命中信息(point);
            _hitedInfoLayer.HitedInfo = info;
            _hitedInfoLayer.Update();
            if (info == null) return "无命中";
            return info.区域名称;
        }

        public void 点击页面()
        {
            Point point = GetMousePoint();
            段落 段落 = _page.获取最近段落(point);
            元素行 元素行 = 段落.获取最近元素行(point);
            _currentLine = 元素行;
            _page.更新当前段落((段落)元素行.Parent);
            _inputBoxLayer.Line = 元素行;
            _inputBoxLayer.Update();
            光标信息 info = 元素行.移动光标(point);
            ((段落)元素行.Parent).更新光标索引(元素行);
            _caretLayer.CaretX = info.X;
            _caretLayer.CaretY = info.Y;
            _caretLayer.CaretHeight = info.Height;
            _caretLayer.Update();
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

        public void ClearHighlight()
        {
            if (_page.高亮元素 == null) return;

            _page.清除高亮元素();
            _highlightLayer.HighlightElement = null;
            _highlightLayer.Clear();
            _caretLayer.Visibility = Visibility.Visible;
        }

        #endregion

        #region 页面事件

        private void 页面_当前段落变化(段落 段落)
        {
            当前段落变化?.Invoke(段落);
        }

        private void 页面_高度变化(double height)
        {
            Paper.Height = _page.页高 + PagePadding.Top + PagePadding.Bottom;
        }

        private void 页面_当前行变化(元素行 line)
        {
            _currentLine = line;
            _inputBoxLayer.Line = line;
            _inputBoxLayer.Update();
        }

        private void 页面_光标移动(光标信息 info)
        {
            _caretLayer.CaretX = info.X;
            _caretLayer.CaretY = info.Y;
            _caretLayer.CaretHeight = info.Height;
            _caretLayer.Update();
        }

        private void 页面_高亮元素变化(行内元素? 元素)
        {
            if (元素 != null)
            {
                StopBlinkIBeam();
                _caretLayer.Visibility = Visibility.Hidden;
            }
            else
            {
                _caretLayer.Visibility = Visibility.Visible;
                StartBlinkIBeam();
            }
            _highlightLayer.HighlightElement = 元素;
            _highlightLayer.Update();
        }

        #endregion

        #region 控件事件

        private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Focus();
        }

        private void InteractionLayer_MouseMove(object sender, MouseEventArgs e)
        {
            _tool.OnMouseMove();
        }

        private void InteractionLayer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _tool.OnMouseDown(e.ChangedButton);
        }

        private void InteractionLayer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _tool.OnMouseUp(e.ChangedButton);
        }

        private void InteractionLayer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            _tool.OnMouseWheel(e);
        }

        #endregion

        #region 私有方法

        private T AddLayer<T>(bool enabled = true) where T : SingleBoard
        {
            T layer = Activator.CreateInstance<T>();
            layer.Init();
            layer.IsEnabled = enabled;
            MarkBox.Children.Add(layer);
            Canvas.SetLeft(layer, PagePadding.Left);
            Canvas.SetTop(layer, PagePadding.Top);
            return layer;
        }

        private Point GetMousePoint()
        {
            Point mousePoint = Mouse.GetPosition(InteractionLayer);
            mousePoint.X -= PagePadding.Left;
            mousePoint.Y -= PagePadding.Top;
            return mousePoint;
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

        private 页面 _page = new 页面();

        private HoveredInfoLayer _hoveredInfoLayer;
        private HitedInfoLayer _hitedInfoLayer;
        private InputBoxLayer _inputBoxLayer;
        private HighlightLayer _highlightLayer;
        private CaretLayer _caretLayer;
        private EditTool _tool;

        /// <summary>光标可见性，用于闪烁光标</summary>
        private bool _ibeamVisible = true;
        private readonly DispatcherTimer _blinkTimer = new DispatcherTimer();

        private 元素行 _currentLine = null!;

        private readonly Dictionary<Key, EditKey> _editKeyDict = new Dictionary<Key, EditKey>();
        private readonly List<Key> _ctrlEditKeyList = new List<Key>();

        #endregion
    }
}