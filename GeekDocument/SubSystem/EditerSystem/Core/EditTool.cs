using XLogic.Wpf.Behavior;
using XLogic.Wpf.Tool;

namespace GeekDocument.SubSystem.EditerSystem.Core;

public class EditTool : ToolBase<Editer>
{
    public EditTool(Editer host) : base(host) { }

    public override void Init()
    {
        移动();
        点击页面();
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
            _host.UpdateHoveredBlock();
        });
        Finish();
    }

    private void 点击页面()
    {
        NewTree("点击页面", (_) =>
        {
            // 停止光标闪烁
            _host.StopBlinkIBeam();
            // 移动光标至鼠标位置
            _host.MoveIBeamToMousePoint();
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
}