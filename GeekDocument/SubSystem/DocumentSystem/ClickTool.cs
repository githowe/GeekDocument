using XLogic.Wpf.Behavior;
using XLogic.Wpf.Tool;

namespace GeekDocument.SubSystem.DocumentSystem
{
    public class ClickTool : ToolBase<RecentItem>
    {
        public ClickTool(RecentItem host) : base(host) { }

        public override void Init()
        {
            NewTree(Behaviors.LeftDown, (_) =>
            {
                // 捕获控件
                _host.CaptureControl();
                // 更新为按下状态
                _host.Pressed = true;
            });
            NewNode(Behaviors.LeftUp, (_) =>
            {
                ResetTree();
                // 释放控件
                _host.ReleaseControl();
                // 取消按下状态
                _host.Pressed = false;
                // 触发单击
                _host.InvokeClick();
            });
            BackToRoot();
            NewNode(Behaviors.Move, (_) =>
            {
                // 更新按下状态
                _host.UpdatePressedState();
            });
            NewNode(Behaviors.LeftUp, (_) =>
            {
                ResetTree();
                // 释放控件
                _host.ReleaseControl();
                // 触发单击并取消按下状态
                if (_host.Pressed)
                {
                    _host.InvokeClick();
                    _host.Pressed = false;
                }
            });
            Finish();
        }
    }
}