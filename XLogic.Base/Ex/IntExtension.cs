namespace XLogic.Base.Ex
{
    public static class IntExtension
    {
        /// <summary>
        /// 将整数限制在指定范围内
        /// </summary>
        public static int Limit(this int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}