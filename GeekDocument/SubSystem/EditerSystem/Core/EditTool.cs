using GeekDocument.SubSystem.EditerSystem.Core.Component;
using XLogic.Wpf.Behavior;
using XLogic.Wpf.Tool;

namespace GeekDocument.SubSystem.EditerSystem.Core;

public class EditTool : ToolBase<InteractionComponent>
{
    public EditTool(InteractionComponent host) : base(host) { }

    public override void Init()
    {
        移动();
        点击页面();
        滚轮();
    }

    public override void OnLeftButtonDown(BehaviorArgs? args = null)
    {
        Invoke("点击页面");
    }

    private void 移动()
    {
        NewTree(Behaviors.Move, (_) =>
        {
            ResetTree();
            // 处理鼠标移动
            _host.HandleMouseMove();
        });
        Finish();
    }

    private void 点击页面()
    {
        // 点击页面 -> 松开左键
        BehaviorNode leftDown = NewTree("点击页面", (_) =>
        {
            _host.CaptureMouse();
            // 停止光标闪烁
            _host.StopBlinkIBeam();
            // 处理鼠标按下
            _host.HandleMouseDown();
        });
        // 按下左键后，滚动滚轮时的行为
        leftDown.MouseWheel = _host.滚动页面并更新选区;
        NewNode(Behaviors.LeftUp, (_) =>
        {
            _host.ReleaseMouse();
            ResetTree();
            // 开始光标闪烁
            _host.StartBlinkIBeam();
        });
        BackToRoot();
        // 点击页面 -> 移动鼠标 -> 松开左键
        BehaviorNode mouseMove = NewNode(Behaviors.Move, (_) =>
        {
            _host.移动光标并更新选区();
        });
        // 移动鼠标后，滚动滚轮时的行为
        mouseMove.MouseWheel = _host.滚动页面并更新选区;
        NewNode(Behaviors.LeftUp, (_) =>
        {
            _host.ReleaseMouse();
            ResetTree();
            _host.StartBlinkIBeam();
        });
        Finish();
    }

    private void 滚轮()
    {
        NewTree(Behaviors.Wheel, (e) =>
        {
            ResetTree();
            // 处理鼠标滚轮
            _host.HandleMouseWheel(((MouseWheelBehaviorArgs)e).WheelArgs);
        });
        Finish();
    }
}