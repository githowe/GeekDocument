namespace GeekDocument.SubSystem.LayoutEngine.Tool
{
    public class 元素队列
    {
        public List<布局元素> 元素列表 { get; set; } = new List<布局元素>();

        public 元素行? 生成元素行(double 行宽, 水平对齐方式 对齐)
        {
            // 已取完
            if (当前索引 >= 元素列表.Count) return null;

            // 创建行
            元素行 行 = new 元素行 { 行宽 = 行宽 };
            // 循环添加元素至行
            while (当前索引 < 元素列表.Count)
            {
                // 取出元素
                布局元素 元素 = 元素列表[当前索引];
                // 空白元素直接添加，必然添加成功
                if (元素.IsSpace)
                {
                    当前索引++;
                    行.尝试添加元素(元素, 对齐 == 水平对齐方式.Justify);
                    continue;
                }
                // 处理可断开元素
                if (元素.CanBreak && 行.无可见元素 && 行.剩余空间 > 0 && 元素.ActualWidth > 行.剩余空间)
                {
                    // 非两端对齐，
                    if (对齐 != 水平对齐方式.Justify || 元素.压缩实际宽度() > 行.剩余空间)
                    {
                        布局元素 断开部分 = 元素.断开(行.剩余空间);
                        元素列表.Insert(当前索引, 断开部分);
                        continue;
                    }
                }
                // 添加至行，添加失败表示行已满
                bool added = 行.尝试添加元素(元素, 对齐 == 水平对齐方式.Justify);
                if (added) 当前索引++;
                else break;
            }
            // 返回行
            return 行;
        }

        private int 当前索引 = 0;
    }
}