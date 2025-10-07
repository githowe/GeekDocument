using GeekDocument.SubSystem.EditerSystem.Define;
using GeekDocument.SubSystem.LayoutEngine.Element;
using XLogic.Base.StateTree;

namespace GeekDocument.SubSystem.LayoutEngine.Tool
{
    public class STElementLine
    {
        public STElementLine(元素行 Line) => this.Line = Line;

        public 元素行 Line { get; set; }

        public void Init()
        {
            Init_Left();
            Init_Right();

            Init_Backspace();
            Init_Enter();
        }

        public void HandleEditKey(EditKey key)
        {
            switch (key)
            {
                case EditKey.Up:
                    _up.Execute();
                    break;
                case EditKey.Down:
                    _down.Execute();
                    break;
                case EditKey.Left:
                    _left.Execute();
                    break;
                case EditKey.Right:
                    _right.Execute();
                    break;

                case EditKey.Home:
                    _home.Execute();
                    break;
                case EditKey.End:
                    _end.Execute();
                    break;

                case EditKey.Backspace:
                    _backspace.Execute();
                    break;
                case EditKey.Delete:
                    _delete.Execute();
                    break;
                case EditKey.Enter:
                    _enter.Execute();
                    break;
            }
        }

        private void Init_Left()
        {
            // 有高亮元素：取消高亮并移动光标至高亮元素左侧
            // 光标前有元素
            //     前元素支持输入：移入光标至前元素末尾
            //     前元素不支持输入：前移光标
            // 光标前无元素：调用所属段落的左移光标
            _left.NewNode("有高亮元素", Line.有高亮元素, Line.移动光标至高亮元素左侧);
            StateNode 光标前有元素 = _left.NewNode("光标前有元素", Line.光标前有元素, null);
            _left.NewNode("前元素支持输入", Line.前元素支持输入, Line.移入光标至前元素末尾, 光标前有元素);
            _left.NewNode("前元素不支持输入", () => !Line.前元素支持输入(), Line.前移光标, 光标前有元素);
            _left.NewNode("光标前无元素", () => !Line.光标前有元素(), Line.调用所属段落的左移光标);
        }

        private void Init_Right()
        {
            // 有高亮元素：取消高亮并移动光标至高亮元素右侧
            // 光标后有元素
            //     当前元素支持输入：移入光标至当前元素开头
            //     当前元素不支持输入：后移光标
            // 光标后无元素：调用所属段落的右移光标
            _right.NewNode("有高亮元素", Line.有高亮元素, Line.移动光标至高亮元素右侧);
            StateNode 光标后有元素 = _right.NewNode("光标后有元素", Line.光标后有元素, null);
            _right.NewNode("当前元素支持输入", Line.当前元素支持输入, Line.移入光标至当前元素开头, 光标后有元素);
            _right.NewNode("当前元素不支持输入", () => !Line.当前元素支持输入(), Line.后移光标, 光标后有元素);
            _right.NewNode("光标后无元素", () => !Line.光标后有元素(), Line.调用所属段落的右移光标);
        }

        private void Init_Backspace()
        {
            // 光标前有元素
            //     字元素：删除前元素
            //     非字元素
            //         未高亮：高亮元素
            //         已高亮：删除元素
            // 光标前无元素：调用所属段落的退格
            StateNode 光标前有元素 = _backspace.NewNode("光标前有元素", Line.光标前有元素, null);
            _backspace.NewNode("字元素", Line.光标前为字元素, Line.删除前字符, 光标前有元素);
            StateNode 非字元素 = _backspace.NewNode("非字元素", () => !Line.光标前为字元素(), null, 光标前有元素);
            _backspace.NewNode("未高亮", Line.前元素已高亮, Line.删除前元素, 非字元素);
            _backspace.NewNode("已高亮", () => !Line.前元素已高亮(), Line.高亮前元素, 非字元素);
            _backspace.NewNode("光标前无元素", () => !Line.光标前有元素(), Line.调用所属段落的退格);
        }

        private void Init_Enter()
        {
            // 元素行无法判断处于段落的什么位置，直接调用段落的处理回车
            _enter.NewNode("光标在任意处", () => true, Line.处理回车);
        }

        private void 无操作() => Console.WriteLine("无操作");

        private readonly StateTree _backspace = new StateTree("退格");
        private readonly StateTree _delete = new StateTree("删除");
        private readonly StateTree _enter = new StateTree("回车");
        private readonly StateTree _up = new StateTree("上");
        private readonly StateTree _down = new StateTree("下");
        private readonly StateTree _left = new StateTree("左");
        private readonly StateTree _right = new StateTree("右");
        private readonly StateTree _home = new StateTree("头");
        private readonly StateTree _end = new StateTree("尾");
    }
}