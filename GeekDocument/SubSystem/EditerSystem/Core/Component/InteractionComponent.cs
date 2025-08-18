using GeekDocument.SubSystem.EditerSystem.Define;
using System.Windows;
using System.Windows.Input;
using XLogic.Base.UI;

namespace GeekDocument.SubSystem.EditerSystem.Core.Component
{
    /// <summary>
    /// 交互组件。处理鼠标、键盘事件
    /// </summary>
    public class InteractionComponent : Component<Editer>
    {
        #region 生命周期

        protected override void Init()
        {
            _tool = new EditTool(this);
            _tool.Init();

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

        protected override void Enable()
        {
            _host.DocArea.MouseDown += DocArea_MouseDown;
            _host.DocArea.MouseMove += DocArea_MouseMove;
            _host.DocArea.MouseUp += DocArea_MouseUp;
            _host.DocArea.MouseLeave += DocArea_MouseLeave;
            _host.DocArea.MouseWheel += DocArea_MouseWheel;
        }

        protected override void Disable()
        {
            _host.DocArea.MouseDown -= DocArea_MouseDown;
            _host.DocArea.MouseMove -= DocArea_MouseMove;
            _host.DocArea.MouseUp -= DocArea_MouseUp;
            _host.DocArea.MouseLeave -= DocArea_MouseLeave;
            _host.DocArea.MouseWheel -= DocArea_MouseWheel;
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 处理按键按下
        /// </summary>
        public void HandleKeyDown(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                if (_editKeyDict.TryGetValue(e.Key, out EditKey editKey))
                {
                    GetComponent<IBeamComponent>().StopBlinkIBeam();
                    GetComponent<PageComponent>().HandleEditKey(editKey);
                    GetComponent<IBeamComponent>().StartBlinkIBeam();
                    e.Handled = true;
                }
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (_ctrlEditKeyList.Contains(e.Key))
                {
                    GetComponent<IBeamComponent>().StopBlinkIBeam();
                    GetComponent<PageComponent>().HandleCtrl_Key(e.Key);
                    GetComponent<IBeamComponent>().StartBlinkIBeam();
                    e.Handled = true;
                }
            }
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 停止光标闪烁
        /// </summary>
        public void StopBlinkIBeam() => GetComponent<IBeamComponent>().StopBlinkIBeam();

        /// <summary>
        /// 开始光标闪烁
        /// </summary>
        public void StartBlinkIBeam() => GetComponent<IBeamComponent>().StartBlinkIBeam();

        /// <summary>
        /// 处理鼠标移动
        /// </summary>
        public void HandleMouseMove()
        {

        }

        /// <summary>
        /// 处理鼠标按下
        /// </summary>
        public void HandleMouseDown()
        {
            // 获取鼠标相对于页面的坐标
            Point mousePoint = Mouse.GetPosition(_host.DocArea);
            mousePoint.Y += GetComponent<PageComponent>().Offset - 16;
            GetComponent<PageComponent>().HandleMouseDown(mousePoint);
        }

        /// <summary>
        /// 处理鼠标滚轮
        /// </summary>
        public void HandleMouseWheel(MouseWheelEventArgs e) => GetComponent<ScrollBarComponent>().HandleMouseWheel(e);

        #endregion

        #region 控件事件

        private void DocArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _tool.OnMouseDown(e.ChangedButton);
        }

        private void DocArea_MouseMove(object sender, MouseEventArgs e)
        {
            _tool.OnMouseMove();
        }

        private void DocArea_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _tool.OnMouseUp(e.ChangedButton);
        }

        private void DocArea_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void DocArea_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            _tool.OnMouseWheel(e);
        }

        #endregion

        #region 字段

        private EditTool _tool;

        private readonly Dictionary<Key, EditKey> _editKeyDict = new Dictionary<Key, EditKey>();
        private readonly List<Key> _ctrlEditKeyList = new List<Key>();

        #endregion
    }
}