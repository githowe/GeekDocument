namespace GeekDocument.SubSystem.EditerSystem.Define
{
    public abstract class Block
    {
        public BlockType Type { get; set; } = BlockType.Text;

        public Document? OwnerDoc { get; set; } = null;

        /// <summary>
        /// 更新视图数据
        /// </summary>
        public abstract void UpdateViewData();

        /// <summary>
        /// 获取视图高度
        /// </summary>
        public virtual int GetViewHeight() => 0;

        public abstract void LoadJson(string json);

        public abstract string ToJson();
    }
}