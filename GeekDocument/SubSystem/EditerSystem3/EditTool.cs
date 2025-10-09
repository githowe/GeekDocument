using XLogic.Wpf.Behavior;
using XLogic.Wpf.Tool;

namespace GeekDocument.SubSystem.EditerSystem3
{
    public class EditTool : ToolBase<PageView>
    {
        public EditTool(PageView host) : base(host) { }

        public override void Init()
        {
            移动();
            点击页面();
        }

        public override void OnLeftButtonDown(BehaviorArgs? args = null)
        {
            switch (_host.获取命中区域())
            {
                case "图片":
                    // Invoke("点击图片");
                    Invoke("点击页面");
                    break;
                case "选中文本":
                    Invoke("点击选中文本");
                    break;
                case "表格手柄":
                    Invoke("点击表格手柄");
                    break;
                case "插入列按钮":
                    Invoke("点击插入列");
                    break;
                case "插入行按钮":
                    Invoke("点击插入行");
                    break;
                case "调整列宽按钮":
                    Invoke("点击调整列宽");
                    break;
                case "调整行高按钮":
                    Invoke("点击调整行高");
                    break;
                case "选择列":
                    Invoke("点击选择列");
                    break;
                case "选择行":
                    Invoke("点击选择行");
                    break;
                case "拖动手柄区域":
                case "表格上方":
                case "表格左侧":
                    break;
                default:
                    Invoke("点击页面");
                    break;
            }
        }

        private void 移动()
        {
            NewTree(Behaviors.Move, (_) =>
            {
                _host.处理鼠标移动();
                ResetTree();
            });
            Finish();
        }

        private void 点击页面()
        {
            BehaviorNode leftDown = NewTree("点击页面", (_) =>
            {
                _host.StopBlinkIBeam();
                _host.ClearHighlight();
                _host.点击页面();
            });
            NewNode(Behaviors.LeftUp, (_) =>
            {
                ResetTree();
                _host.StartBlinkIBeam();
            });
            Finish();
        }
    }
}