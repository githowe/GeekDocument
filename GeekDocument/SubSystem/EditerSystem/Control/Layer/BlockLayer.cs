using GeekDocument.SubSystem.EditerSystem.Core;
using XLogic.Wpf.Drawing;

namespace GeekDocument.SubSystem.EditerSystem.Control.Layer
{
    /// <summary>
    /// 块图层
    /// </summary>
    public abstract class BlockLayer : SingleBoard
    {
        /// <summary>编辑器实例</summary>
        public Editer Editer { get; set; }

        /// <summary>块高度：根据内容动态生成</summary>
        public abstract int BlockHeight { get; }

        /// <summary>
        /// 移动光标至开头
        /// </summary>
        public virtual void MoveIBeamToHead() { }

        /// <summary>
        /// 移动光标至末尾
        /// </summary>
        public virtual void MoveIBeamToEnd() { }
    }
}