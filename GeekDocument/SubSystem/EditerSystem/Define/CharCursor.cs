namespace GeekDocument.SubSystem.EditerSystem.Define
{
    /// <summary>
    /// 字符游标
    /// </summary>
    public class CharCursor
    {
        /// <summary>块索引</summary>
        public int BlockIndex { get; set; } = -1;

        /// <summary>字符索引</summary>
        public int CharIndex { get; set; } = -1;

        public bool SameAs(CharCursor other)
        {
            if (other == null) return false;
            return BlockIndex == other.BlockIndex && CharIndex == other.CharIndex;
        }

        public int CompareTo(CharCursor other)
        {
            if (other == null) throw new Exception("另一个比较对象为空");
            // 比较块索引
            if (BlockIndex != other.BlockIndex)
                return BlockIndex.CompareTo(other.BlockIndex);
            // 如果块索引相同，则比较字符索引
            return CharIndex.CompareTo(other.CharIndex);
        }
    }

    /// <summary>
    /// 选区
    /// </summary>
    public class Selection
    {
        public CharCursor? Start { get; set; } = null;

        public CharCursor? End { get; set; } = null;

        public bool HasSelection => Start != null && End != null;
    }
}