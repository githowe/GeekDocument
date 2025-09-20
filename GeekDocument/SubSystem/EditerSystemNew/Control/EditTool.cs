using XLogic.Wpf.Behavior;
using XLogic.Wpf.Tool;

namespace GeekDocument.SubSystem.EditerSystemNew.Control;

public class EditTool : ToolBase<Page>
{
    public EditTool(Page host) : base(host) { }

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
            // _host.HandleMouseMove();
        });
        Finish();
    }

    private void 点击页面()
    {
        BehaviorNode leftDown = NewTree("点击页面", (_) =>
        {
            _host.StopBlinkIBeam();
            _host.HandleMouseDown();
        });
        NewNode(Behaviors.LeftUp, (_) =>
        {
            ResetTree();
            _host.StartBlinkIBeam();
        });
        Finish();
    }
}