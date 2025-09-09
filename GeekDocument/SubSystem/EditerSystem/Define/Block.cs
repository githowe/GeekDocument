using GeekDocument.SubSystem.StyleSystem;

namespace GeekDocument.SubSystem.EditerSystem.Define
{
    public abstract class Block
    {
        public BlockType Type { get; set; } = BlockType.Text;

        public int MarginTop { get; set; } = 0;

        public int MarginBottom { get; set; } = 0;

        public int MarginLeft { get; set; } = 0;

        public int MarginRight { get; set; } = 0;

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init() { }

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

        /// <summary>
        /// 设置子样式。即为子元素设置样式
        /// </summary>
        public virtual void SetSubStyle(AppendStyle style, int startIndex, int endIndex) { }

        public abstract void LoadJson(string json);

        public abstract string ToJson();
    }
}