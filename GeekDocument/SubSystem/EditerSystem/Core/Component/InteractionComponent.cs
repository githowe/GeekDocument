using GeekDocument.SubSystem.EditerSystem.Control.Layer;
using GeekDocument.SubSystem.EditerSystem.Define;
using System.Windows;
using System.Windows.Input;
using XLogic.Base.UI;

namespace GeekDocument.SubSystem.EditerSystem.Core.Component
{
    /// <summary>
    /// 命中元素类型
    /// </summary>
    public enum HitedElementType
    {
        Page,
        Link,
        Image,
    }

    /// <summary>
    /// 交互组件。处理鼠标、键盘事件
    /// </summary>
    public class InteractionComponent : Component<Editer>
    {
        #region 属性

        public HitedElementType HitedType => _hitedType;

        #endregion

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
            UpdateKeyModifier();
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

        /// <summary>
        /// 处理按键松开
        /// </summary>
        public void HandleKeyUp(KeyEventArgs e)
        {
            UpdateKeyModifier();
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 打开链接
        /// </summary>
        public void OpenLink()
        {
            // 使用默认浏览器打开链接
            if (_hitedLink != "") System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = _hitedLink,
                UseShellExecute = true
            });
        }

        /// <summary>
        /// 停止光标闪烁
        /// </summary>
        public void StopBlinkIBeam() => GetComponent<IBeamComponent>().StopBlinkIBeam();

        /// <summary>
        /// 开始光标闪烁
        /// </summary>
        public void StartBlinkIBeam() => GetComponent<IBeamComponent>().StartBlinkIBeam();

        public void CaptureMouse() => _host.DocArea.CaptureMouse();

        public void ReleaseMouse() => _host.DocArea.ReleaseMouseCapture();

        /// <summary>
        /// 处理鼠标移动
        /// </summary>
        public void HandleMouseMove()
        {
            Point mousePoint = Mouse.GetPosition(_host.DocArea);
            mousePoint.Y += GetComponent<PageComponent>().Offset - 16;
            GetComponent<PageComponent>().HandleMouseMove(mousePoint);
            // 更新命中元素类型
            UpdatedHitedElementType();
            // 如果命中链接，则显示跳转提示
            if (_hitedType == HitedElementType.Link)
            {
                _hitedLink = GetComponent<PageComponent>().HoveredBlockLayer.GetHoverLink(mousePoint);
                GetComponent<TipComponent>().ShowTip($"Ctrl+单击跳转：{_hitedLink}");
                if (Keyboard.Modifiers == ModifierKeys.Control) _host.DocArea.Cursor = Cursors.Hand;
                else _host.DocArea.Cursor = Cursors.IBeam;
            }
            else
            {
                _hitedLink = "";
                GetComponent<TipComponent>().HideTip();
                _host.DocArea.Cursor = Cursors.IBeam;
            }
        }

        /// <summary>
        /// 更新命中元素类型
        /// </summary>
        public void UpdatedHitedElementType()
        {
            _hitedType = HitedElementType.Page;
            // 获取鼠标相对于页面的坐标
            Point mousePoint = Mouse.GetPosition(_host.DocArea);
            mousePoint.Y += GetComponent<PageComponent>().Offset - 16;
            // 获取悬停块
            BlockLayer hovered = GetComponent<PageComponent>().HoveredBlockLayer;
            // 位于图片上
            if (hovered.SourceBlock.Type == BlockType.Image)
            {
                _hitedType = HitedElementType.Page;
            }
            // 位于文本块或代码块上
            if (hovered.SourceBlock.Type is BlockType.Text or BlockType.Code)
            {
                // 获取悬停处链接
                if (hovered.GetHoverLink(mousePoint) != "") _hitedType = HitedElementType.Link;
            }
        }

        /// <summary>
        /// 处理鼠标按下
        /// </summary>
        public void HandleMouseDown()
        {
            SelectComponent selectComponent = GetComponent<SelectComponent>();

            // 如果已有选区
            if (selectComponent.HasSelection)
            {
                // 取消选区并显示光标
                selectComponent.CancelSelection();
                GetComponent<IBeamComponent>().ShowIBeam();
            }

            // 获取鼠标相对于页面的坐标
            Point mousePoint = Mouse.GetPosition(_host.DocArea);
            mousePoint.Y += GetComponent<PageComponent>().Offset - 16;
            // 页面组件处理鼠标按下：移动光标
            GetComponent<PageComponent>().HandleMouseDown(mousePoint);
            // 选择组件处理鼠标按下：开始选择
            GetComponent<SelectComponent>().HandleMouseDown();
        }

        /// <summary>
        /// 处理鼠标滚轮
        /// </summary>
        public void HandleMouseWheel(MouseWheelEventArgs e) => GetComponent<ScrollBarComponent>().HandleMouseWheel(e);

        public void 滚动页面并更新选区(int delta)
        {
            // 隐藏光标
            GetComponent<IBeamComponent>().HideIBeam();
            // 滚动页面
            GetComponent<ScrollBarComponent>().Scroll(delta);
            // 获取鼠标相对于页面的坐标
            Point mousePoint = Mouse.GetPosition(_host.DocArea);
            mousePoint.Y += GetComponent<PageComponent>().Offset - 16;
            // 移动光标
            GetComponent<PageComponent>().HandleMouseDown(mousePoint);
            // 更新选区
            GetComponent<SelectComponent>().UpdateSelection();
        }

        public void 移动光标并更新选区()
        {
            // 获取鼠标相对于页面的坐标
            Point mousePoint = Mouse.GetPosition(_host.DocArea);
            mousePoint.Y += GetComponent<PageComponent>().Offset - 16;
            // 移动光标
            GetComponent<PageComponent>().HandleMouseDown(mousePoint);
            // 更新选区
            GetComponent<SelectComponent>().UpdateSelection();
            // 根据选区显示或隐藏光标
            CharCursor? 起始游标 = GetComponent<SelectComponent>().Start;
            CharCursor? 结束游标 = GetComponent<SelectComponent>().End;
            if (起始游标 == null || 结束游标 == null || 结束游标.SameAs(起始游标))
                GetComponent<IBeamComponent>().ShowIBeam();
            else
                GetComponent<IBeamComponent>().HideIBeam();
        }

        #endregion

        #region 控件事件

        private void DocArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _host.Focus();
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

        #region 私有方法

        private void UpdateKeyModifier()
        {
            _modifier = Keyboard.Modifiers;
            HandleModifiChanged();
        }

        /// <summary>
        /// 处理修饰键变化
        /// </summary>
        private void HandleModifiChanged()
        {
            if (_hitedType != HitedElementType.Link) return;
            if (_modifier == ModifierKeys.Control) _host.DocArea.Cursor = Cursors.Hand;
            else _host.DocArea.Cursor = Cursors.IBeam;
        }

        #endregion

        #region 字段

        private EditTool _tool;

        private readonly Dictionary<Key, EditKey> _editKeyDict = new Dictionary<Key, EditKey>();
        private readonly List<Key> _ctrlEditKeyList = new List<Key>();

        private HitedElementType _hitedType = HitedElementType.Page;
        private ModifierKeys _modifier = ModifierKeys.None;

        private string _hitedLink = "";

        #endregion
    }
}