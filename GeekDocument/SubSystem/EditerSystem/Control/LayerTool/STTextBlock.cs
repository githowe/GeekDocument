using GeekDocument.SubSystem.EditerSystem.Control.Layer;
using XLogic.Base.StateTree;

namespace GeekDocument.SubSystem.EditerSystem.Control.LayerTool
{
    public class STTextBlock : BlockStateTree<TextBlockLayer>
    {
        public void Init()
        {
            Init_Backspace();
            Init_Enter();
        }

        private void Init_Backspace()
        {
            // 无字符
            //     有前块 - 删除块
            //     无前块 - 无操作
            // 光标前无字符
            //     有前块
            //         可以合并 - 合并块
            //         不可合并 - 移动光标至前块末尾
            //     无前块 - 无操作
            // 光标前有字符 - 删除字符

            StateNode 无字符 = _backspace.NewNode("无字符", () => Layer.IsEmpty, null);
            _backspace.NewNode("有前块", () => Layer.HasPrevBlock, Layer.删除块, 无字符);
            _backspace.NewNode("无前块", () => !Layer.HasPrevBlock, 无操作, 无字符);

            StateNode 光标前无字符 = _backspace.NewNode("光标前无字符", () => Layer.CharIndex == 0, null);
            StateNode 有前块 = _backspace.NewNode("有前块", () => Layer.HasPrevBlock, null, 光标前无字符);
            _backspace.NewNode("可以合并", Layer.能否合并, Layer.合并块, 有前块);
            _backspace.NewNode("不可合并", () => !Layer.能否合并(), Layer.移动光标至前块末尾, 有前块);
            _backspace.NewNode("无前块", () => !Layer.HasPrevBlock, 无操作, 光标前无字符);

            _backspace.NewNode("光标前有字符", () => Layer.CharIndex > 0, Layer.删除字符);
        }

        private void Init_Enter()
        {
            // 光标后无字符 - 创建空的正文块
            // 光标后有字符 - 创建带文本的正文块

            _enter.NewNode("光标后无字符", () => Layer.CharIndex == Layer.TextLength, Layer.创建空文本块);
            _enter.NewNode("光标后有字符", () => Layer.CharIndex < Layer.TextLength, Layer.创建文本块);
        }
    }
}