using GeekDocument.AppTool.Ex;
using GeekDocument.SubSystem.WindowSystem.ColorPick.Component;
using GeekDocument.SubSystem.WindowSystem.ColorPick.Tool;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using XLogic.Base.Ex;
using XLogic.Base.UI;
using XLogic.Wpf.Window;
using MyColor = XLogic.Base.Color;

namespace GeekDocument.SubSystem.WindowSystem.ColorPick
{
    public partial class ColorPicker : XDialog
    {
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        public static extern uint TimeBeginPeriod(uint ms);

        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        public static extern uint TimeEndPeriod(uint ms);

        #region 构造方法

        public ColorPicker()
        {
            InitializeComponent();
            OK.Click += OK_Click;
            Closed += ColorPicker_Closed;
        }

        #endregion

        #region 属性

        /// <summary>主色</summary>
        public MyColor MainColor { get; set; } = new MyColor(255, 255, 255);

        public ColorSyncTool SyncTool => _syncTool;

        #endregion

        #region 事件

        /// <summary>颜色变更时</summary>
        public Action<Color>? OnColorChanged { get; set; } = null;

        /// <summary>确定颜色时</summary>
        public Action<Color>? OnColorConfirm { get; set; } = null;

        #endregion

        #region 工具方法

        /// <summary>
        /// 捕获色谱区域
        /// </summary>
        public void CaptureCube() => Grid_Cube.CaptureMouse();

        /// <summary>
        /// 释放色谱区域
        /// </summary>
        public void ReleaseCube() => Grid_Cube.ReleaseMouseCapture();

        /// <summary>
        /// 将拾取框移动至鼠标处
        /// </summary>
        public void MovePickFrame()
        {
            // 移动拾取框
            Point mousePoint = Mouse.GetPosition(Grid_Cube);
            _componentBox.GetComponent<ColorCubeComponent>().MovePickFrame(mousePoint.X.Limit(0, 255), mousePoint.Y.Limit(0, 255));
        }

        #endregion

        #region 窗口事件

        protected override void XWindowLoaded()
        {
            TimeBeginPeriod(1);
            // 添加组件
            ComparerComponent? comparer = _componentBox.AddComponent<ComparerComponent>(this, "颜色对比组件");
            ColorCubeComponent? colorCube = _componentBox.AddComponent<ColorCubeComponent>(this, "色立方组件");
            ColorBarComponent? colorBar = _componentBox.AddComponent<ColorBarComponent>(this, "色条组件");
            ColorValueComponent? colorValue = _componentBox.AddComponent<ColorValueComponent>(this, "色值组件");
            ColorCodeComponent? colorCode = _componentBox.AddComponent<ColorCodeComponent>(this, "颜色代码组件");
            // 初始化组件
            _componentBox.Init();

            // 注册至颜色同步工具
            _syncTool.RegisterSyncHandler(comparer);
            _syncTool.RegisterSyncHandler(colorCube);
            _syncTool.RegisterSyncHandler(colorBar);
            _syncTool.RegisterSyncHandler(colorValue);
            _syncTool.RegisterSyncHandler(colorCode);
            // 初始化颜色
            _syncTool.InitColor(MainColor.ToMediaColor());

            // 创建并初始化拾取工具
            _pickTool = new PickTool(this);
            _pickTool.Init();

            // 监听颜色变更
            _componentBox.GetComponent<ComparerComponent>().ColorChanged = (color) => OnColorChanged?.Invoke(color);
        }

        private void ColorPicker_Closed(object? sender, EventArgs e)
        {
            // if (!_okClicked) OnColorConfirm?.Invoke(MainColor.ToMediaColor());
            TimeEndPeriod(1);
        }

        #endregion

        #region 控件事件

        private void Grid_Cube_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _pickTool?.OnMouseDown(e.ChangedButton);
        }

        private void Grid_Cube_MouseMove(object sender, MouseEventArgs e)
        {
            _pickTool?.OnMouseMove();
        }

        private void Grid_Cube_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _pickTool?.OnMouseUp(e.ChangedButton);
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            // _okClicked = true;
            OnColorConfirm?.Invoke(_componentBox.GetComponent<ComparerComponent>().Color);
            Close();
        }

        #endregion

        #region 字段

        private PickTool _pickTool;
        private readonly ComponentBox<ColorPicker> _componentBox = new ComponentBox<ColorPicker>();
        private readonly ColorSyncTool _syncTool = new ColorSyncTool();

        #endregion
    }
}