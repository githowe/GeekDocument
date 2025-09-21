using GeekDocument.SubSystem.EditerSystem.Define;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            // 光标前有元素
            //     前元素支持输入：移入光标至前元素末尾
            //     前元素不支持输入：前移光标
            // 光标前无元素：调用所属段落的左移光标
            StateNode 光标前有元素 = _left.NewNode("光标前有元素", Line.光标前有元素, null);
            _left.NewNode("前元素支持输入", Line.前元素支持输入, Line.移入光标至前元素末尾, 光标前有元素);
            _left.NewNode("前元素不支持输入", () => !Line.前元素支持输入(), Line.前移光标, 光标前有元素);
            _left.NewNode("光标前无元素", () => !Line.光标前有元素(), Line.调用所属段落的左移光标);
        }

        private void Init_Right()
        {

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