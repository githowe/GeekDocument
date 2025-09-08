using XLogic.Wpf.Behavior;
using XLogic.Wpf.Tool;

namespace GeekDocument.SubSystem.WindowSystem.ColorPick.Tool
{
    public class PickTool : ToolBase<ColorPicker>
    {
        public PickTool(ColorPicker host) : base(host) { }

        public override void Init()
        {
            // 左键按下 -> 松开
            NewTree(Behaviors.LeftDown, (_) =>
            {
                _host.MovePickFrame();
            });
            NewNode(Behaviors.LeftUp, (_) =>
            {
                _host.ReleaseCube();
                ResetTree();
            });
            // 左键按下 -> 移动 -> 松开
            BackToRoot();
            NewNode(Behaviors.Move, (_) =>
            {
                _host.MovePickFrame();
            });
            NewNode(Behaviors.LeftUp, (_) =>
            {
                _host.ReleaseCube();
                ResetTree();
            });
            Finish();
        }

        public override void OnLeftButtonDown(BehaviorArgs? args = null)
        {
            _host.CaptureCube();
            base.OnLeftButtonDown();
        }
    }
}