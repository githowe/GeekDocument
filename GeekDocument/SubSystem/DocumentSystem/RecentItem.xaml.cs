using GeekDocument.SubSystem.CacheSystem.Define;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using XLogic.Base.Ex;

namespace GeekDocument.SubSystem.DocumentSystem
{
    /// <summary>
    /// 最近项
    /// </summary>
    public partial class RecentItem : UserControl
    {
        #region 构造方法

        public RecentItem() => InitializeComponent();

        #endregion

        #region 属性

        /// <summary>最近文档实例</summary>
        public RecentDocument? Instance { get; set; } = null;

        public bool Hover
        {
            get => _mouseHover;
            set
            {
                _mouseHover = value;
                UpdateBackground();
            }
        }

        public bool Pressed
        {
            get => _mousePressed;
            set
            {
                _mousePressed = value;
                UpdateBackground();
            }
        }

        #endregion

        #region 委托

        /// <summary>单击项</summary>
        public Action<RecentItem>? Click { get; set; } = null;

        /// <summary>单击标记</summary>
        public Action<RecentItem>? MarkClick { get; set; } = null;

        /// <summary>单击移除</summary>
        public Action<RecentItem>? RemoveClick { get; set; } = null;

        #endregion

        #region 公开方法

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            if (Instance == null)
            {
                // 禁用点击并隐藏
                IsHitTestVisible = false;
                Visibility = Visibility.Hidden;
            }
            else
            {
                // 工具提示
                MainGrid.ToolTip = Instance.FullPath;
                // 名称与路径
                (string, string) pathInfo = Instance.FullPath.ParsePath();
                Block_Name.Text = pathInfo.Item2;
                Block_Path.Text = pathInfo.Item1;
                // 日期
                Block_Time.Text = Instance.EditTime;
                // 标记
                if (Instance.Marked)
                {
                    Grid_Mark.Visibility = Visibility.Visible;
                    Tool_Mark.Opacity = 0;
                }
                else
                {
                    Grid_Mark.Visibility = Visibility.Hidden;
                    Tool_Mark.Opacity = 1;
                }
                // 启用点击并设置为可见
                IsHitTestVisible = true;
                Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region 工具方法

        public void CaptureControl() => MainGrid.CaptureMouse();

        public void ReleaseControl() => MainGrid.ReleaseMouseCapture();

        public void InvokeClick() => Click?.Invoke(this);

        public void UpdatePressedState()
        {
            Point point = Mouse.GetPosition(this);
            if (point.X < 0 || point.X > ActualWidth || point.Y < 0 || point.Y > ActualHeight)
                Pressed = false;
            else Pressed = true;
        }

        #endregion

        #region 控件事件

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _clickTool = new ClickTool(this);
            _clickTool.Init();
        }

        private void MainGrid_MouseEnter(object sender, MouseEventArgs e) => Hover = true;

        private void MainGrid_MouseLeave(object sender, MouseEventArgs e) => Hover = false;

        private void MainGrid_MouseDown(object sender, MouseButtonEventArgs e) => _clickTool.OnMouseDown(e.ChangedButton);

        private void MainGrid_MouseMove(object sender, MouseEventArgs e) => _clickTool.OnMouseMove();

        private void MainGrid_MouseUp(object sender, MouseButtonEventArgs e) => _clickTool.OnMouseUp(e.ChangedButton);

        private void Tool_Mark_Click(object sender, RoutedEventArgs e) => MarkClick?.Invoke(this);

        private void Tool_Folder_Click(object sender, RoutedEventArgs e)
        {
            string? path = System.IO.Path.GetDirectoryName(Instance.FullPath);
            if (path != null) Process.Start("explorer.exe", path);
        }

        private void Tool_Remove_Click(object sender, RoutedEventArgs e) => RemoveClick?.Invoke(this);

        #endregion

        #region 私有方法

        /// <summary>
        /// 更新背景
        /// </summary>
        private void UpdateBackground()
        {
            MainGrid.Background = _default;
            if (_mouseHover) MainGrid.Background = _hover;
            if (_mousePressed) MainGrid.Background = _pressed;
        }

        #endregion

        #region 字段

        private ClickTool _clickTool;

        /// <summary>鼠标悬停</summary>
        private bool _mouseHover = false;
        /// <summary>鼠标已按下</summary>
        private bool _mousePressed = false;

        #endregion

        #region 笔刷

        /// <summary>默认背景</summary>
        private readonly Brush _default = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
        /// <summary>悬停时背景</summary>
        private readonly Brush _hover = new SolidColorBrush(Color.FromArgb(15, 255, 255, 255));
        /// <summary>按下时背景</summary>
        private readonly Brush _pressed = new SolidColorBrush(Color.FromArgb(31, 255, 255, 255));

        #endregion
    }
}