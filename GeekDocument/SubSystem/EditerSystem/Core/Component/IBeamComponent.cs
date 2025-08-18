using GeekDocument.SubSystem.EditerSystem.Core.Layer;
using System.Windows;
using System.Windows.Threading;
using XLogic.Base.UI;

namespace GeekDocument.SubSystem.EditerSystem.Core.Component
{
    /// <summary>
    /// 光标组件：绘制光标并处理闪烁效果
    /// </summary>
    public class IBeamComponent : Component<Editer>
    {
        #region 属性

        /// <summary>光标横坐标</summary>
        public int IBeamX => (int)_layer.IBeamPoint.X;

        public int Offset
        {
            get => _offset;
            set
            {
                _offset = value;
                _layer.Margin = new Thickness(0, -_offset + 16, 0, 0);
            }
        }

        #endregion

        #region 生命周期

        protected override void Init()
        {
            // 添加光标图层
            _layer = new IBeamLayer
            {
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 16, 0, 0),
            };
            _layer.Init();
            _host.LayerBox.Children.Add(_layer);
            // 初始化光标定时器
            _blinkTimer.Interval = TimeSpan.FromMilliseconds(500);
            _blinkTimer.Tick += BlinkTimer_Tick;
        }

        protected override void Enable()
        {
            StartBlinkIBeam();
        }

        protected override void Reset()
        {
            StopBlinkIBeam();
            StartBlinkIBeam();
        }

        protected override void Disable()
        {
            StopBlinkIBeam();
            _layer.Clear();
            _layerVisible = false;
        }

        protected override void Remove()
        {
            _host.LayerBox.Children.Remove(_layer);
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 开始闪烁光标
        /// </summary>
        public void StartBlinkIBeam()
        {
            _layerVisible = true;
            _layer.Update();
            _blinkTimer.Start();
        }

        /// <summary>
        /// 停止闪烁光标
        /// </summary>
        public void StopBlinkIBeam()
        {
            _blinkTimer.Stop();
            _layerVisible = true;
            _layer.Update();
        }

        /// <summary>
        /// 移动光标
        /// </summary>
        public void MoveIBeam(int x, int y, int height)
        {
            _layer.IBeamPoint = new Point(x, y);
            _layer.LineHeight = height;
            _layer.Update();
        }

        #endregion

        #region 私有方法

        private void BlinkTimer_Tick(object? sender, EventArgs e)
        {
            if (_layerVisible) _layer.Clear();
            else _layer.Update();
            _layerVisible = !_layerVisible;
        }

        #endregion

        #region 字段

        /// <summary>光标图层</summary>
        private IBeamLayer _layer;

        private bool _layerVisible = true;
        private readonly DispatcherTimer _blinkTimer = new DispatcherTimer();

        private int _offset = 0;

        #endregion
    }
}