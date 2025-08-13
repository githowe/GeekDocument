using GeekDocument.SubSystem.EditerSystem.Control.Layer;
using GeekDocument.SubSystem.EditerSystem.Define;
using XLogic.Base.StateTree;

namespace GeekDocument.SubSystem.EditerSystem.Control.LayerTool
{
    /// <summary>
    /// 块状态树。用于处理块的不同状态下的操作
    /// </summary>
    public class BlockStateTree<T> where T : BlockLayer
    {
        public T Layer { get; set; }

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

        protected void 无操作() => Console.WriteLine("无操作");

        protected readonly StateTree _backspace = new StateTree("退格");
        protected readonly StateTree _delete = new StateTree("删除");
        protected readonly StateTree _enter = new StateTree("回车");
        protected readonly StateTree _up = new StateTree("上");
        protected readonly StateTree _down = new StateTree("下");
        protected readonly StateTree _left = new StateTree("左");
        protected readonly StateTree _right = new StateTree("右");
        protected readonly StateTree _home = new StateTree("头");
        protected readonly StateTree _end = new StateTree("尾");
    }
}