using GeekDocument.SubSystem.EditerSystem.Core;
using GeekDocument.SubSystem.EditerSystem.Define;
using System.Windows;
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

        /// <summary>块实例</summary>
        public Block SourceBlock { get; set; }

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

        /// <summary>
        /// 获取鼠标悬停区域
        /// </summary>
        public virtual Rect GetHoveredRect(Point mousePoint) { return new Rect(); }

        /// <summary>
        /// 移动光标
        /// </summary>
        public virtual double MoveIBeam(Point mousePoint) => 0;

        /// <summary>
        /// 处理编辑键
        /// </summary>
        public virtual void HandleEditKey(EditKey key) { }

        #region 状态树接口

        public bool HasPrevBlock => Editer.HasPrevBlock(this);

        public bool HasNextBlock => Editer.HasNextBlock(this);

        public void 删除块() => Editer.RemoveBlock(this);

        public void 移动光标至前块末尾() => Editer.MoveIBeamToPrevBlock(this);

        public void 移动光标至后块开头() => Editer.MoveIBeamToNextBlock(this);

        public virtual bool 能否合并() { return false; }

        public virtual void 合并块() { }

        #endregion
    }
}