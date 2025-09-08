using XLogic.Wpf.Behavior;
using XLogic.Wpf.Tool;

namespace GeekDocument.SubSystem.WindowSystem.ColorPick.Tool
{
    public class BarTool : ToolBase<ColorBar>
    {
        public BarTool(ColorBar host) : base(host) { }

        public override void Init()
        {
            // 左键按下 -> 松开
            NewTree(Behaviors.LeftDown, (_) =>
            {
                _host.MoveToMouse();
            });
            NewNode(Behaviors.LeftUp, (_) =>
            {
                _host.ReleaseBar();
                ResetTree();
            });
            // 左键按下 -> 移动 -> 松开
            BackToRoot();
            NewNode(Behaviors.Move, (_) =>
            {
                _host.Drag();
            });
            NewNode(Behaviors.LeftUp, (_) =>
            {
                _host.ReleaseBar();
                ResetTree();
            });
            Finish();
        }

        public override void OnLeftButtonDown(BehaviorArgs? args = null)
        {
            _host.CaptureBar();
            base.OnLeftButtonDown();
        }
    }
}