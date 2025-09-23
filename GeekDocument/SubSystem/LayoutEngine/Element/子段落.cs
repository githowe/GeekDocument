using GeekDocument.SubSystem.LayoutEngine.Tool;

namespace GeekDocument.SubSystem.LayoutEngine.Element
{
    public class 子段落
    {
        public string SourceText { get; set; } = "";

        public List<元素集> 元素集列表 { get; set; } = new List<元素集>();

        public List<元素行> 元素行列表 { get; set; } = new List<元素行>();

        public void Init()
        {
            // 使用占位标记分割文本
            string[] parts = SourceText.Split(_placeholder);
            // 每个部分创建一个元素集
            foreach (var item in parts)
            {
                元素集 集 = new 元素集 { Text = item };
                元素集列表.Add(集);
            }
        }

        public List<布局元素> GetAllElement()
        {
            List<布局元素> result = new List<布局元素>();
            foreach (var item in 元素集列表)
                result.AddRange(item.ElementList);
            return result;
        }

        /// <summary>
        /// 移动光标至指定索引
        /// </summary>
        public void MoveCaretTo(int index)
        {
            // 确定光标的位置：元素行与元素行内索引
            元素行 目标元素行 = 元素行列表[0];
            int charIndex = 0;
            int indexInLine = 0;
            foreach (var 元素行 in 元素行列表)
            {
                int startIndex = charIndex;
                int endIndex = charIndex + 元素行.元素列表.Count;
                if (startIndex <= index && index <= endIndex)
                {
                    目标元素行 = 元素行;
                    indexInLine = index - startIndex;
                    // 光标在末尾，且有下一个元素行
                    if (index == endIndex && 元素行 != 元素行列表.Last())
                    {
                        int lineIndex = 元素行列表.IndexOf(元素行);
                        目标元素行 = 元素行列表[lineIndex + 1];
                        indexInLine = 0;
                    }
                    break;
                }
                charIndex += 元素行.元素列表.Count;
            }
            // 移动光标
            目标元素行.MoveCaretTo(indexInLine);
        }

        /// <summary>占位标记</summary>
        private readonly string _placeholder = "%%";
    }
}