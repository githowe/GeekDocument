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
            // 更新悬停块
            // _host.UpdateHoveredBlock();
        });
        Finish();
    }

    private void 点击页面()
    {
        NewTree("点击页面", (_) =>
        {
            // 停止光标闪烁
            _host.StopBlinkIBeam();
            // 处理鼠标按下
            _host.HandleMouseDown();
        });
        NewNode(Behaviors.LeftUp, (_) =>
        {
            ResetTree();
            // 开始光标闪烁
            _host.StartBlinkIBeam();
        });
        BackToRoot();
        NewNode(Behaviors.Move, (_) =>
        {
            // 更新选中内容
        });
        NewNode(Behaviors.LeftUp, (_) =>
        {
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