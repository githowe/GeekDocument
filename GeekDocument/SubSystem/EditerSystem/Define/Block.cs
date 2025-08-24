using GeekDocument.SubSystem.StyleSystem;

namespace GeekDocument.SubSystem.EditerSystem.Define
{
    public abstract class Block
    {
        public BlockType Type { get; set; } = BlockType.Text;

        /// <summary>
        /// 更新视图数据
        /// </summary>
        public abstract void UpdateViewData(int blockWidth);

        /// <summary>
        /// 获取视图高度
        /// </summary>
        public virtual int GetViewHeight() => 0;

        /// <summary>
        /// 应用样式
        /// </summary>
        public virtual void ApplyStyle(StyleSheet? style) { }

        public abstract void LoadJson(string json);

        public abstract string ToJson();
    }
}