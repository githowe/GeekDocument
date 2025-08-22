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
        #region 属性

        /// <summary>页面实例</summary>
        public IPage Page { get; set; }

        /// <summary>块实例</summary>
        public Block SourceBlock { get; set; }

        /// <summary>块宽度：外部设置</summary>
        public int BlockWidth { get; set; } = 0;

        /// <summary>块高度：根据内容动态生成</summary>
        public abstract int BlockHeight { get; }

        #endregion

        #region 光标接口

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
        /// 移动光标至索引
        /// </summary>
        public virtual void MoveIBeamToIndex(int index) { }

        /// <summary>
        /// 移动光标至坐标
        /// </summary>
        public virtual void MoveIBeamToPoint(Point point) { }

        /// <summary>
        /// 同步光标
        /// </summary>
        public virtual void SyncIBeam() { }

        #endregion

        /// <summary>
        /// 处理编辑键
        /// </summary>
        public virtual void HandleEditKey(EditKey key) { }

        /// <summary>
        /// 输入文本
        /// </summary>
        public virtual void InputText(string text) { }

        #region 状态树接口

        public bool HasPrevBlock => Page.有上一个块(this);

        public bool HasNextBlock => Page.有下一个块(this);

        public void 移动光标至前块末尾() => Page.移动光标至前块末尾(this);

        public void 移动光标至后块开头() => Page.移动光标至后块开头(this);

        public void 移动光标至前块最后一行() => Page.移动光标至前块最后一行(this);

        public void 移动光标至后块第一行() => Page.移动光标至后块第一行(this);

        public virtual bool 能否合并() { return false; }

        public virtual void 合并块() { }

        #endregion
    }
}