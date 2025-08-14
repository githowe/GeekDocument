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
        /// 移动光标至第一行
        /// </summary>
        public virtual void MoveIBeamToFirstLine(double mouse_x) { }

        /// <summary>
        /// 移动光标至最后一行
        /// </summary>
        public virtual void MoveIBeamToLastLine(double mouse_x) { }

        /// <summary>
        /// 移动光标至指定索引处
        /// </summary>
        public virtual void MoveIBeamTo(int index) { }

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

        public void 删除块()
        {
            Editer.RemoveBlock(this);
            Editer.UpdateIBeamX();
        }

        public void 移动光标至前块末尾()
        {
            Editer.MoveIBeamToPrevBlock(this);
            Editer.UpdateIBeamX();
        }

        public void 移动光标至后块开头()
        {
            Editer.MoveIBeamToNextBlock(this);
            Editer.UpdateIBeamX();
        }

        public void 移动光标至前块最后一行() => Editer.MoveIBeamToPrevLine(this);

        public void 移动光标至后块第一行() => Editer.MoveIBeamToNextLine(this);

        public virtual bool 能否合并() { return false; }

        public virtual void 合并块() { }

        #endregion
    }
}