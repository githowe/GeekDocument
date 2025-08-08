using XLogic.Wpf.Drawing;

namespace GeekDocument.SubSystem.EditerSystem.Control.Layer
{
    /// <summary>
    /// 块图层
    /// </summary>
    public abstract class BlockLayer : SingleBoard
    {
        /// <summary>块高度：根据内容动态生成</summary>
        public abstract int BlockHeight { get; }
    }
}