using GeekDocument.SubSystem.EditerSystem.Control.Layer;
using XLogic.Base.StateTree;

namespace GeekDocument.SubSystem.EditerSystem.Control.LayerTool
{
    public class STFormulaBlock : BlockStateTree<FormulaBlockLayer>
    {
        public void Init(FormulaBlockLayer layer)
        {
            Layer = layer;

            Init_Up();
            Init_Down();
            Init_Left();
            Init_Right();

            Init_Home();
            Init_End();

            Init_Backspace();
            Init_Enter();
        }

        private void Init_Up()
        {
            // 有前块 - 移动至前块最后一行
            // 无前块 - 无操作

            _up.NewNode("有前块", () => Layer.HasPrevBlock, Layer.移动光标至前块最后一行, null);
            _up.NewNode("无前块", () => !Layer.HasPrevBlock, 无操作, null);
        }

        private void Init_Down()
        {
            // 有后块 - 移动至后块第一行
            // 无后块 - 无操作

            _down.NewNode("有后块", () => Layer.HasNextBlock, Layer.移动光标至后块第一行, null);
            _down.NewNode("无后块", () => !Layer.HasNextBlock, 无操作, null);
        }

        private void Init_Left()
        {
            // 光标前无字符
            //     有前块 - 移动光标至前块末尾
            //     无前块 - 无操作
            // 光标前有字符 - 前移光标

            StateNode 光标前无字符 = _left.NewNode("光标前无字符", () => Layer.CharIndex == 0, null);
            _left.NewNode("有前块", () => Layer.HasPrevBlock, Layer.移动光标至前块末尾, 光标前无字符);
            _left.NewNode("无前块", () => !Layer.HasPrevBlock, 无操作, 光标前无字符);
            _left.NewNode("光标前有字符", () => Layer.CharIndex > 0, Layer.左移光标);
        }

        private void Init_Right()
        {
            // 光标后无字符
            //     有后块 - 移动光标至后块开头
            //     无后块 - 无操作
            // 光标后有字符 - 后移光标

            StateNode 光标后无字符 = _right.NewNode("光标后无字符", () => Layer.CharIndex == Layer.CharIndexMax, null);
            _right.NewNode("有后块", () => Layer.HasNextBlock, Layer.移动光标至后块开头, 光标后无字符);
            _right.NewNode("无后块", () => !Layer.HasNextBlock, 无操作, 光标后无字符);
            _right.NewNode("光标后有字符", () => Layer.CharIndex < Layer.CharIndexMax, Layer.右移光标);
        }

        private void Init_Home()
        {
            _home.NewNode("光标不在行首", () => true, Layer.移动光标至行首);
        }

        private void Init_End()
        {
            _end.NewNode("光标不在行尾", () => true, Layer.移动光标至行尾);
        }

        private void Init_Backspace()
        {
            // 公式前
            //     有前块
            //         前块为空 - 删除前块
            //         前块不为空 - 移动光标至前块末尾
            //     无前块 - 无操作
            // 公式后
            //     有前块 - 删除块
            //     无前块 - 替换为空文本块

            StateNode 公式前 = _backspace.NewNode("公式前", () => Layer.CharIndex == 0, null);
            StateNode 有前块 = _backspace.NewNode("有前块", () => Layer.HasPrevBlock, null, 公式前);
            _backspace.NewNode("前块为空", () => Layer.PrevBlockIsEmpty, Layer.删除前块, 有前块);
            _backspace.NewNode("前块不为空", () => !Layer.PrevBlockIsEmpty, Layer.移动光标至前块末尾, 有前块);
            _backspace.NewNode("无前块", () => !Layer.HasPrevBlock, 无操作, 公式前);

            StateNode 公式后 = _backspace.NewNode("公式后", () => Layer.CharIndex == 1, null);
            _backspace.NewNode("有前块", () => Layer.HasPrevBlock, Layer.用退格键删除块, 公式后);
            _backspace.NewNode("无前块", () => !Layer.HasPrevBlock, Layer.替换为空文本块, 公式后);
        }

        private void Init_Enter()
        {
            // 公式前 - 在块前插入空文本块
            // 公式后 - 在块后插入空文本块

            _enter.NewNode("公式前", () => Layer.CharIndex == 0, Layer.在块前插入空文本块, null);
            _enter.NewNode("公式后", () => Layer.CharIndex == 1, Layer.在块后插入空文本块, null);
        }
    }
}