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
    }
}