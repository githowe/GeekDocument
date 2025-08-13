using GeekDocument.SubSystem.EditerSystem.Control.Layer;
using GeekDocument.SubSystem.EditerSystem.Core.Layer;
using GeekDocument.SubSystem.EditerSystem.Define;
using GeekDocument.SubSystem.OptionSystem;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Page = GeekDocument.SubSystem.EditerSystem.Control.Page;

namespace GeekDocument.SubSystem.EditerSystem.Core
{
    public partial class Editer : UserControl
    {
        public Editer() => InitializeComponent();

        public Document Document { get; private set; }

        #region 公开方法

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            InitPage();
            InitLayer();
        }

        /// <summary>
        /// 加载文档
        /// </summary>
        public void LoadDocument()
        {
            List<string> fontList = new List<string>
            {
                "仿宋",
                "楷体",
                "新宋体",
                "微软雅黑",
                "霞鹜文楷",
            };
            List<Block> blockList = new List<Block>();
            // 读取文本文件
            foreach (var item in File.ReadAllLines("D:/示例文档3.txt"))
            {
                BlockText blockText = new BlockText
                {
                    Content = item,
                    LineSpace = 4,
                    // FontFamily = fontList.RandomElement(),
                };
                blockText.UpdateViewData();
                blockList.Add(blockText);
            }
            _page.LoadBlock(blockList);
            // 初始化编辑系统
            InitEditSystem();
        }

        /// <summary>
        /// 加载文档
        /// </summary>
        public void LoadDocument(Document document)
        {
            // 设置文档实例
            Document = document;
            // 加载块
            _page.LoadBlock(document.BlockList);
            // 初始化编辑系统
            InitEditSystem();
        }

        /// <summary>
        /// 处理按键按下
        /// </summary>
        public void HandleKeyDown(Key key)
        {
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                switch (key)
                {
                    case Key.Back:
                        _page.HandleEditKey(EditKey.Backspace);
                        _page.UpdateBlockPoint();
                        _page.UpdatePageHeight();
                        break;
                    case Key.Delete:
                        _page.HandleEditKey(EditKey.Delete);
                        break;
                    case Key.Enter:
                        _page.HandleEditKey(EditKey.Enter);
                        break;
                    case Key.Up:
                        _page.HandleEditKey(EditKey.Up);
                        break;
                    case Key.Down:
                        _page.HandleEditKey(EditKey.Down);
                        break;
                    case Key.Left:
                        _page.HandleEditKey(EditKey.Left);
                        break;
                    case Key.Right:
                        _page.HandleEditKey(EditKey.Right);
                        break;
                    case Key.Home:
                        _page.HandleEditKey(EditKey.Home);
                        break;
                    case Key.End:
                        _page.HandleEditKey(EditKey.End);
                        break;
                }
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (key)
                {
                    case Key.A:
                        break;
                    case Key.X:
                        break;
                    case Key.C:
                        break;
                    case Key.V:
                        break;
                    case Key.Z:
                        break;
                    case Key.Y:
                        break;
                    case Key.Enter:
                        break;
                }
            }
        }

        /// <summary>
        /// 有上一个块
        /// </summary>
        public bool HasPrevBlock(BlockLayer layer)
        {
            int index = _page.BlockList.IndexOf(layer);
            return index > 0;
        }

        /// <summary>
        /// 有下一个块
        /// </summary>
        public bool HasNextBlock(BlockLayer layer)
        {
            int index = _page.BlockList.IndexOf(layer);
            return index < _page.BlockList.Count - 1;
        }

        /// <summary>
        /// 获取上一个块
        /// </summary>
        public BlockLayer? GetPrevBlock(BlockLayer layer) => _page.GetPrevBlock(layer);

        /// <summary>
        /// 获取下一个块
        /// </summary>
        public BlockLayer? GetNextBlock(BlockLayer layer) => _page.GetNextBlock(layer);

        /// <summary>
        /// 移除块
        /// </summary>
        public void RemoveBlock(BlockLayer layer)
        {
            // 获取上一个块
            BlockLayer? prevBlock = _page.GetPrevBlock(layer);
            // 移除当前块
            _page.RemoveBlock(layer);
            // 将上一个块设为当前块
            _page.SetCurrentBlock(prevBlock);
            // 移动光标至上一个块末尾
            prevBlock.MoveIBeamToEnd();
        }

        /// <summary>
        /// 获取块索引
        /// </summary>
        public int GetBlockIndex(BlockLayer layer) => _page.BlockList.IndexOf(layer);

        /// <summary>
        /// 插入块
        /// </summary>
        public void InsertBlock(Block block, int index)
        {

        }

        #endregion

        #region 光标控制

        /// <summary>
        /// 移动光标
        /// </summary>
        public void MoveIBeam(int x, int y, int height)
        {
            _markLayer.IBeamPoint = new Point(x, y);
            _markLayer.LineHeight = height;
            _markLayer.Update();
        }

        /// <summary>
        /// 开始闪烁光标
        /// </summary>
        public void StartBlinkIBeam()
        {
            _ibeamVisible = true;
            _markLayer.Update();
            _blinkTimer.Start();
        }

        /// <summary>
        /// 停止闪烁光标
        /// </summary>
        public void StopBlinkIBeam()
        {
            _blinkTimer.Stop();
            _ibeamVisible = true;
            _markLayer.Update();
        }

        /// <summary>
        /// 移动光标至上一个块
        /// </summary>
        public void MoveIBeamToPrevBlock(BlockLayer layer)
        {

        }

        /// <summary>
        /// 移动光标至下一个块
        /// </summary>
        public void MoveIBeamToNextBlock(BlockLayer layer)
        {

        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 更新悬停块
        /// </summary>
        public void UpdateHoveredBlock()
        {
            Point mousePoint = Mouse.GetPosition(DocArea);
            mousePoint.Y += PageScrollBar.Value - 16;
            _page.UpdateHoveredBlock(mousePoint);
        }

        /// <summary>
        /// 移动光标至鼠标位置
        /// </summary>
        public void MoveIBeamToMousePoint()
        {
            if (_page.HoveredBlock == null) return;

            Point mousePoint = Mouse.GetPosition(DocArea);
            mousePoint.Y += PageScrollBar.Value - 16;
            // 设置命中块为悬停块
            _page.SetCurrentBlock(_page.HoveredBlock);
            // 获取悬停区域
            Rect rect = _page.HoveredBlock.GetHoveredRect(mousePoint);
            // 移动光标至鼠标坐标处，返回实际光标横坐标
            double x = _page.HoveredBlock.MoveIBeam(mousePoint);
            // 光标纵坐标为悬停区域顶部
            double y = rect.Top;
            // 更新光标
            _markLayer.IBeamPoint = new Point((int)x, y);
            _markLayer.LineHeight = rect.Height;
            _markLayer.Update();
        }

        #endregion

        #region 私有方法

        private void InitPage()
        {
            // 设置文档宽度
            var pageOptoin = Options.Instance.Page;
            int docWidth = pageOptoin.PageWidth + pageOptoin.PageMargin.Left + pageOptoin.PageMargin.Right;
            DocArea.Width = docWidth;
            // 新建页面添加至文档区域
            _page = new Page { Editer = this };
            PageBox.Children.Add(_page);
            _page.UpdateLayout();
            _page.Init();
        }

        /// <summary>
        /// 初始化图层
        /// </summary>
        private void InitLayer()
        {
            _markLayer = new MarkLayer();
            _markLayer.Init();
            LayerBox.Children.Add(_markLayer);
        }

        /// <summary>
        /// 初始化编辑系统
        /// </summary>
        private void InitEditSystem()
        {
            // 设置块的编辑器实例
            foreach (var item in _page.BlockList) item.Editer = this;
            // 初始化滚动条
            InitScrollBar();
            // 启动光标闪烁
            _blinkTimer.Interval = TimeSpan.FromMilliseconds(500);
            _blinkTimer.Tick += BlinkTimer_Tick;
            _blinkTimer.Start();
            // 获取第一个块
            BlockLayer? firstBlock = _page.GetFirstBlock();
            // 移动光标至开头
            if (firstBlock != null)
            {
                _page.SetCurrentBlock(firstBlock);
                firstBlock.MoveIBeamToHead();
            }
            // 初始化编辑工具、状态树
            InitTool();
        }

        /// <summary>
        /// 初始化工具
        /// </summary>
        private void InitTool()
        {
            // 创建并初始化工具
            _editTool = new EditTool(this);
            _editTool.Init();
            // 监听文档区域鼠标事件
            DocArea.MouseDown += DocArea_MouseDown;
            DocArea.MouseMove += DocArea_MouseMove;
            DocArea.MouseUp += DocArea_MouseUp;
            DocArea.MouseLeave += DocArea_MouseLeave;
        }

        /// <summary>
        /// 初始化滚动条
        /// </summary>
        private void InitScrollBar()
        {
            // 设置滚动条范围
            PageScrollBar.ViewportSize = DocArea.ActualHeight;
            PageScrollBar.Maximum = _page.PageHeight - DocArea.ActualHeight;
            // 监听滚动条、滚轮与高度变化
            PageScrollBar.ValueChanged += PageScrollBar_ValueChanged;
            MainGrid.MouseWheel += MainGrid_MouseWheel;
            DocArea.SizeChanged += DocArea_SizeChanged;
            _page.PageHeightChanged = Page_PageHeightChanged;
        }

        private void BlinkTimer_Tick(object? sender, EventArgs e)
        {
            if (_ibeamVisible)
            {
                _markLayer.Clear();
                _ibeamVisible = false;
            }
            else
            {
                _markLayer.Update();
                _ibeamVisible = true;
            }
        }

        #endregion

        #region 控件事件

        private void MainGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            PageScrollBar.Value -= e.Delta / 120 * 64;
        }

        private void DocArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _editTool.OnMouseDown(e.ChangedButton);
        }

        private void DocArea_MouseMove(object sender, MouseEventArgs e)
        {
            _editTool.OnMouseMove();
        }

        private void DocArea_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _editTool.OnMouseUp(e.ChangedButton);
        }

        private void DocArea_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void DocArea_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.HeightChanged)
            {
                PageScrollBar.ViewportSize = DocArea.ActualHeight;
                PageScrollBar.Maximum = _page.PageHeight - DocArea.ActualHeight;
            }
        }

        private void PageScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _page.PageOffset = (int)PageScrollBar.Value;
            _markLayer.Offset = (int)PageScrollBar.Value;
        }

        private void Page_PageHeightChanged()
        {
            PageScrollBar.Maximum = _page.PageHeight - DocArea.ActualHeight;
        }

        #endregion

        #region 字段

        private Page _page;

        private MarkLayer _markLayer;

        private bool _ibeamVisible = true;
        private readonly DispatcherTimer _blinkTimer = new DispatcherTimer();

        private EditTool _editTool;

        #endregion
    }
}