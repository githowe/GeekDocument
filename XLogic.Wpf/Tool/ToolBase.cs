using System.Windows.Input;
using XLogic.Wpf.Behavior;

namespace XLogic.Wpf.Tool
{
    /// <summary>
    /// 鼠标滚轮行为参数
    /// </summary>
    public class MouseWheelBehaviorArgs : BehaviorArgs
    {
        public MouseWheelEventArgs? WheelArgs { get; set; }
    }

    /// <summary>
    /// 工具基类
    /// </summary>
    public abstract class ToolBase
    {
        /// <summary>当前行为树</summary>
        public BehaviorTree? CurrentTree => _handler.CurrentTree;

        /// <summary>重置完成</summary>
        public Action<ToolBase>? ResetFinish { get; set; } = null;

        /// <summary>处理事件</summary>
        public Action<string>? HandleEvent { get; set; } = null;

        /// <summary>
        /// 初始化
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset() => ResetTree();

        /// <summary>
        /// 清理
        /// </summary>
        public virtual void Clear() { }

        #region 输入事件

        /// <summary>
        /// 鼠标按下
        /// </summary>
        public virtual void OnMouseDown(MouseButton button, BehaviorArgs? args = null)
        {
            switch (button)
            {
                case MouseButton.Left:
                    OnLeftButtonDown(args);
                    break;
                case MouseButton.Middle:
                    _handler.Invoke(Behaviors.MiddleDown, args);
                    break;
                case MouseButton.Right:
                    OnRightButtonDown(args);
                    break;
            }
        }

        /// <summary>
        /// 鼠标左键双击
        /// </summary>
        public virtual void OnDoubleClick(BehaviorArgs? args = null)
        {
            _handler.Invoke(Behaviors.DoubleClick, args);
        }

        /// <summary>
        /// 鼠标左键按下
        /// </summary>
        public virtual void OnLeftButtonDown(BehaviorArgs? args = null)
        {
            _handler.Invoke(Behaviors.LeftDown, args);
        }

        /// <summary>
        /// 鼠标右键按下
        /// </summary>
        public virtual void OnRightButtonDown(BehaviorArgs? args = null)
        {
            _handler.Invoke(Behaviors.RightDown, args);
        }

        /// <summary>
        /// 鼠标移动
        /// </summary>
        public virtual void OnMouseMove(BehaviorArgs? args = null)
        {
            _handler.Invoke(Behaviors.Move, args);
        }

        /// <summary>
        /// 鼠标松开
        /// </summary>
        public virtual void OnMouseUp(MouseButton button, BehaviorArgs? args = null)
        {
            switch (button)
            {
                case MouseButton.Left:
                    _handler.Invoke(Behaviors.LeftUp, args);
                    break;
                case MouseButton.Middle:
                    _handler.Invoke(Behaviors.MiddleUp, args);
                    break;
                case MouseButton.Right:
                    _handler.Invoke(Behaviors.RightUp, args);
                    break;
            }
        }

        /// <summary>
        /// 鼠标滚轮
        /// </summary>
        public virtual void OnMouseWheel(MouseWheelEventArgs e)
        {
            _handler.Invoke(Behaviors.Wheel, new MouseWheelBehaviorArgs { WheelArgs = e });
            _handler.HandleMouseWheel(e.Delta / 120);
        }

        /// <summary>
        /// 键盘按下
        /// </summary>
        public virtual void OnKeyDown(Key key) => _handler.HandleKeyDown(key.ToString());

        public virtual void OnKeyDown(KeyEventArgs e) => _handler.HandleKeyDown(e.Key.ToString());

        /// <summary>
        /// 键盘松开
        /// </summary>
        public virtual void OnKeyUp(Key key) => _handler.HandleKeyUp(key.ToString());

        public virtual void OnKeyUp(KeyEventArgs e) => _handler.HandleKeyUp(e.Key.ToString());

        /// <summary>
        /// 鼠标进入
        /// </summary>
        public virtual void OnMouseEnter() => _handler.Invoke(Behaviors.Enter, null);

        /// <summary>
        /// 鼠标离开
        /// </summary>
        public virtual void OnMouseLeave() => _handler.Invoke(Behaviors.Leave, null);

        #endregion

        #region 行为处理

        protected BehaviorNode NewTree(string name, Action<BehaviorArgs?>? action) => _handler.NewBehaviorTree(name, action);

        protected BehaviorNode NewNode(string name, Action<BehaviorArgs?>? action) => _handler.AddBehaviorNode(name, action);

        protected void BackToRoot() => _handler.BackToRoot();

        protected void Finish() => _handler.FnishAdd();

        public void Invoke(string name, BehaviorArgs? args = null) => _handler.Invoke(name, args);

        public void SetEnable(List<string> nodeList, bool enable) => _handler.SetEnable(nodeList, enable);

        protected void ResetTree()
        {
            _handler.Reset();
            ResetFinish?.Invoke(this);
        }

        /// <summary>行为处理器</summary>
        private readonly BehaviorHandler _handler = new BehaviorHandler();

        #endregion
    }

    public abstract class ToolBase<THost> : ToolBase where THost : class
    {
        public ToolBase(THost host)
        {
            Host = host;
            Init();
        }

        public THost Host { get => _host; set => _host = value; }

        protected THost _host;
    }
}