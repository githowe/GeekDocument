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
            // 设置块的编辑器实例
            foreach (var item in _page.BlockList) item.Editer = this;
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
            // 设置块的编辑器实例
            foreach (var item in _page.BlockList) item.Editer = this;
            // 初始化编辑系统
            InitEditSystem();
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

        #endregion

        #region 私有方法

        private void InitPage()
        {
            // 设置文档宽度
            var pageOptoin = Options.Instance.Page;
            int docWidth = pageOptoin.PageWidth + pageOptoin.PageMargin.Left + pageOptoin.PageMargin.Right;
            DocArea.Width = docWidth;
            // 新建页面添加至文档区域
            _page = new Page();
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

        private void PageScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _page.PageOffset = (int)PageScrollBar.Value;
            _markLayer.Offset = (int)PageScrollBar.Value;
        }

        private void MainGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            PageScrollBar.Value -= e.Delta / 120 * 64;
        }

        private void DocArea_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.HeightChanged)
            {
                PageScrollBar.ViewportSize = DocArea.ActualHeight;
                PageScrollBar.Maximum = _page.PageHeight - DocArea.ActualHeight;
            }
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

        #endregion
    }
}