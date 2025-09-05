using GeekDocument.SubSystem.EditerSystem.Control.Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLogic.Base.StateTree;

namespace GeekDocument.SubSystem.EditerSystem.Control.LayerTool
{
    public class STImageBlock : BlockStateTree<ImageBlockLayer>
    {
        public void Init(ImageBlockLayer layer)
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
            // 图片区
            //     图片前
            //         有前块 - 移动光标至前块末尾
            //             前块为空 - 删除前块
            //             前块不为空 - 移动光标至前块末尾
            //         无前块 - 无操作
            //     图片后 - 删除块
            // 图注区
            //     无字符 - 删除图注
            //     光标前有字符 - 删除字符
            //     光标前无字符 - 左移光标(移动至图片后)

            StateNode 图片区 = _backspace.NewNode("图片区", () => Layer.处于图片区, null);

            StateNode 图片前 = _backspace.NewNode("图片前", () => Layer.CharIndex == 0, null, 图片区);
            StateNode 有前块 = _backspace.NewNode("有前块", () => Layer.HasPrevBlock, null, 图片前);
            _backspace.NewNode("前块为空", () => Layer.PrevBlockIsEmpty, Layer.删除前块, 有前块);
            _backspace.NewNode("前块不为空", () => !Layer.PrevBlockIsEmpty, Layer.移动光标至前块末尾, 有前块);
            _backspace.NewNode("无前块", () => !Layer.HasPrevBlock, 无操作, 图片前);

            _backspace.NewNode("图片后", () => Layer.CharIndex == 1, Layer.用退格键删除块, 图片区);

            StateNode 图注区 = _backspace.NewNode("图注区", () => !Layer.处于图片区, null);
            _backspace.NewNode("无字符", () => Layer.EmptyCaption, Layer.删除图注, 图注区);
            _backspace.NewNode("光标前有字符", () => Layer.CharIndex > 2, Layer.用退格键删除字符, 图注区);
            _backspace.NewNode("光标前无字符", () => Layer.CharIndex == 2, Layer.左移光标, 图注区);
        }

        private void Init_Enter()
        {
            // 图片区
            //     图片前 - 在块前插入空文本块
            //     图片后
            //         无图注 - 插入空图注并移动至图注区
            //         有图注 - 移动至图注末尾
            // 图注区 - 在块后插入空文本块

            var 图片区 = _enter.NewNode("图片区", () => Layer.处于图片区, null);
            _enter.NewNode("图片前", () => Layer.CharIndex == 0, Layer.在块前插入空文本块, 图片区);
            var 图片后 = _enter.NewNode("图片后", () => Layer.CharIndex == 1, null, 图片区);
            _enter.NewNode("无图注", () => Layer.NoneCaption, Layer.插入空图注并移动至图注区, 图片后);
            _enter.NewNode("有图注", () => !Layer.NoneCaption, Layer.移动光标至图注末尾, 图片后);
            _enter.NewNode("图注区", () => !Layer.处于图片区, Layer.在块后插入空文本块, null);
        }
    }
}