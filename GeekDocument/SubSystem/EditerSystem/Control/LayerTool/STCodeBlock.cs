using GeekDocument.SubSystem.EditerSystem.Control.Layer;
using System.Windows.Input;
using XLogic.Base.StateTree;

namespace GeekDocument.SubSystem.EditerSystem.Control.LayerTool
{
    public class STCodeBlock : BlockStateTree<CodeBlockLayer>
    {
        public void Init(CodeBlockLayer layer)
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
            // 有上一行 - 移动至上一行
            // 无上一行
            //     有前块 - 移动至前块最后一行
            //     无前块 - 无操作

            _up.NewNode("有上一行", () => Layer.HasPrevLine, Layer.上移光标);
            StateNode 无上一行 = _up.NewNode("无上一行", () => !Layer.HasPrevLine, null);
            _up.NewNode("有前块", () => Layer.HasPrevBlock, Layer.移动光标至前块最后一行, 无上一行);
            _up.NewNode("无前块", () => !Layer.HasPrevBlock, 无操作, 无上一行);
        }

        private void Init_Down()
        {
            // 有下一行 - 移动至下一行
            // 无下一行
            //     有后块 - 移动至后块第一行
            //     无后块 - 无操作

            _down.NewNode("有下一行", () => Layer.HasNextLine, Layer.下移光标);
            StateNode 无下一行 = _down.NewNode("无下一行", () => !Layer.HasNextLine, null);
            _down.NewNode("有后块", () => Layer.HasNextBlock, Layer.移动光标至后块第一行, 无下一行);
            _down.NewNode("无后块", () => !Layer.HasNextBlock, 无操作, 无下一行);
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

            StateNode 光标后无字符 = _right.NewNode("光标后无字符", () => Layer.CharIndex == Layer.TextLength, null);
            _right.NewNode("有后块", () => Layer.HasNextBlock, Layer.移动光标至后块开头, 光标后无字符);
            _right.NewNode("无后块", () => !Layer.HasNextBlock, 无操作, 光标后无字符);
            _right.NewNode("光标后有字符", () => Layer.CharIndex < Layer.TextLength, Layer.右移光标);
        }

        private void Init_Home()
        {
            _home.NewNode("光标不在行首", () => !Layer.光标在行首, Layer.移动光标至行首);
        }

        private void Init_End()
        {
            _end.NewNode("光标不在行尾", () => !Layer.光标在行尾, Layer.移动光标至行尾);
        }

        private void Init_Backspace()
        {
            // 单行
            //    空行
            //        有前块 - 用退格键删除块
            //        无前块 - 替换为文本块
            //    非空行
            //        光标前有字符 - 删除字符
            //        光标前无字符
            //            有前块
            //                前块为空 - 删除前块
            //                前块不为空 - 移动光标至前块末尾
            //            无前块 - 无操作
            // 多行
            //     空行
            //        有上一行 - 删除当前行
            //        无上一行
            //            有前块 - 删除当前行并移动光标至前块末尾
            //            无前块 - 无操作
            //     非空行
            //         光标前有字符 - 删除字符
            //         光标前无字符
            //             有上一行 - 合并当前行至上一行
            //             无上一行
            //                 有前块
            //                     前块为空 - 删除前块
            //                     前块不为空 - 移动光标至前块末尾
            //                 无前块 - 无操作

            StateNode 单行 = _backspace.NewNode("单行", () => Layer.LineCount == 1, null);

            StateNode 空行 = _backspace.NewNode("空行", () => Layer.EmptyLine, null, 单行);
            _backspace.NewNode("有前块", () => Layer.HasPrevBlock, Layer.用退格键删除块, 空行);
            _backspace.NewNode("无前块", () => !Layer.HasPrevBlock, Layer.替换为文本块, 空行);

            StateNode 非空行 = _backspace.NewNode("非空行", () => !Layer.EmptyLine, null, 单行);
            _backspace.NewNode("光标前有字符", () => Layer.光标前有字符, Layer.删除字符, 非空行);
            StateNode 光标前无字符 = _backspace.NewNode("光标前无字符", () => !Layer.光标前有字符, null, 非空行);
            StateNode 有前块 = _backspace.NewNode("有前块", () => Layer.HasPrevBlock, null, 光标前无字符);
            _backspace.NewNode("前块为空", () => Layer.PrevBlockIsEmpty, Layer.删除前块, 有前块);
            _backspace.NewNode("前块不为空", () => !Layer.PrevBlockIsEmpty, Layer.移动光标至前块末尾, 有前块);
            _backspace.NewNode("无前块", () => !Layer.HasPrevBlock, 无操作, 光标前无字符);

            StateNode 多行 = _backspace.NewNode("多行", () => Layer.LineCount > 1, null);
            空行 = _backspace.NewNode("空行", () => Layer.EmptyLine, null, 多行);
            _backspace.NewNode("有上一行", () => Layer.HasPrevLine, Layer.删除当前行, 空行);
            StateNode 无上一行 = _backspace.NewNode("无上一行", () => !Layer.HasPrevLine, null, 空行);
            _backspace.NewNode("有前块", () => Layer.HasPrevBlock, Layer.删除当前行并移动光标至前块末尾, 无上一行);
            _backspace.NewNode("无前块", () => !Layer.HasPrevBlock, 无操作, 无上一行);
            非空行 = _backspace.NewNode("非空行", () => !Layer.EmptyLine, null, 多行);
            _backspace.NewNode("光标前有字符", () => Layer.光标前有字符, Layer.删除字符, 非空行);
            光标前无字符 = _backspace.NewNode("光标前无字符", () => !Layer.光标前有字符, null, 非空行);
            _backspace.NewNode("有上一行", () => Layer.HasPrevLine, Layer.合并当前行至上一行, 光标前无字符);
            无上一行 = _backspace.NewNode("无上一行", () => !Layer.HasPrevLine, null, 光标前无字符);
            有前块 = _backspace.NewNode("有前块", () => Layer.HasPrevBlock, null, 无上一行);
            _backspace.NewNode("前块为空", () => Layer.PrevBlockIsEmpty, Layer.删除前块, 有前块);
            _backspace.NewNode("前块不为空", () => !Layer.PrevBlockIsEmpty, Layer.移动光标至前块末尾, 有前块);
            _backspace.NewNode("无前块", () => !Layer.HasPrevBlock, 无操作, 无上一行);
        }

        private void Init_Enter()
        {
            // 修饰键为Ctrl
            //     光标处于块头 - 在块前插入文本块
            //     光标未处于块头 - 在块后插入文本块
            // 光标处于行尾 - 创建空行
            // 光标未处于行尾 - 创建行

            StateNode 修饰键Ctrl = _enter.NewNode("修饰键Ctrl", () => Keyboard.Modifiers == ModifierKeys.Control, null);
            _enter.NewNode("光标处于块头", () => Layer.光标处于块头, Layer.在块前插入文本块, 修饰键Ctrl);
            _enter.NewNode("光标未处于块头", () => !Layer.光标处于块头, Layer.在块后插入文本块, 修饰键Ctrl);
            _enter.NewNode("光标处于行尾", () => Layer.光标在行尾, Layer.创建空行);
            _enter.NewNode("光标未处于行尾", () => !Layer.光标在行尾, Layer.创建行);
        }
    }
}