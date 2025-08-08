namespace XLogic.Base.Ex
{
    public static class ListExtension
    {
        /// <summary>
        /// 判断索引是否越界
        /// </summary>
        public static bool IndexOut<T>(this List<T> list, int index) => index < 0 || index > list.Count - 1;

        /// <summary>
        /// 移除最后一个元素
        /// </summary>
        public static void RemoveLast<T>(this List<T> list) => list.RemoveAt(list.Count - 1);

        /// <summary>
        /// 列表转字符串
        /// </summary>
        public static string ToListString<T>(this List<T> list, string split = " ")
        {
            if (list.Count == 0) return "";

            string result = "";
            foreach (var item in list) result += $"{item}{split}";
            return result[..^split.Length];
        }

        /// <summary>
        /// 封装为列表
        /// </summary>
        public static List<T> PackToList<T>(this T item) => new List<T> { item };

        public static double[] CutTo(this double[] source, int resultLength)
        {
            double[] result = new double[resultLength];
            for (int index = 0; index < resultLength; index++)
                result[index] = source[index];
            return result;
        }

        /// <summary>
        /// 复制元素至
        /// </summary>
        public static void CopyTo<T>(this List<T> source, List<T> target)
        {
            int copyLength = Math.Min(source.Count, target.Count);
            for (int index = 0; index < copyLength; index++)
                target[index] = source[index];
        }

        /// <summary>
        /// 偏移列表
        /// </summary>
        public static List<T> Offset<T>(this List<T> source, int offset)
        {
            List<T> result = new List<T>();
            offset %= source.Count;
            if (offset < 0) offset = source.Count + offset;
            for (int index = offset; index < source.Count; index++)
                result.Add(source[index]);
            for (int index = 0; index < offset; index++)
                result.Add(source[index]);
            return result;
        }

        /// <summary>
        /// 反相
        /// </summary>
        public static void Invert(this List<byte> source)
        {
            for (int index = 0; index < source.Count; index++)
                source[index] = (byte)(255 - source[index]);
        }

        /// <summary>
        /// 随机元素顺序
        /// </summary>
        public static List<int> RandomElementOrder(this List<int> source)
        {
            List<int> result = new List<int>();

            Random random = new Random();
            int index;
            int count = source.Count;
            while (count > 0)
            {
                // 随机索引
                index = random.Next(source.Count);
                // 添加元素至结果列表
                result.Add(source[index]);
                // 从源列表移除元素
                source.RemoveAt(index);
                // 更新计数
                count--;
            }

            return result;
        }

        /// <summary>
        /// 按子列表长度分割列表
        /// </summary>
        public static List<List<T>> SplitBySubLength<T>(this List<T> source, int subListLength)
        {
            List<List<T>> result = new List<List<T>>();

            // 子列表数量
            int subListCount = (int)Math.Ceiling((double)source.Count / subListLength);
            // 创建子列表
            for (int index = 0; index < subListCount; index++) result.Add(new List<T>());
            // 填充子列表
            for (int index = 0; index < source.Count; index++)
            {
                int subListIndex = index / subListLength;
                result[subListIndex].Add(source[index]);
            }

            return result;
        }

        /// <summary>
        /// 获取命中区间索引
        /// </summary>
        public static int GetHitedRange(this List<double> source, double number)
        {
            // 小于第一个元素，返回第一个区间索引
            if (number < source[0]) return 0;
            // 返回命中区间左索引
            for (int index = 0; index < source.Count - 1; index++)
            {
                if (number >= source[index] && number < source[index + 1])
                    return index;
            }
            // 大于最后一个元素，返回最后一个区间索引
            return source.Count - 2;
        }

        /// <summary>
        /// 获取命中横坐标
        /// </summary>
        public static double GetHitedX(this List<double> xList, double mouse_x)
        {
            if (xList.Count == 0) return -1;
            if (xList.Count == 1) return xList[0];

            int hitedRange = xList.GetHitedRange(mouse_x);
            double hitedLeft = xList[hitedRange];
            double hitedRight = xList[hitedRange + 1];
            double center = hitedLeft + (hitedRight - hitedLeft) / 2;
            double hitedx = mouse_x < center ? hitedLeft : hitedRight;
            return hitedx;
        }

        /// <summary>
        /// 随机获取列表中的一个元素
        /// </summary>
        public static T RandomElement<T>(this List<T> source)
        {
            if (source.Count == 0) throw new Exception("列表不能为空");
            int index = new Random().Next(source.Count);
            return source[index];
        }
    }
}