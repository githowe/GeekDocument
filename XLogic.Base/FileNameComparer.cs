using System.Collections;
using XLogic.Base.Ex;
using XLogic.Windows.shlwapi;

namespace XLogic.Base
{
    /// <summary>
    /// 文件名比较器：保证文件按字母顺序排列
    /// </summary>
    public class FileNameComparer : IComparer
    {
        public int Compare(object? x, object? y)
        {
            if (x == null || y == null) return 0;
            string? xstr = x.ToString();
            if (xstr == null) return 0;
            return shlwapiInterop.NaturalCompare(x.ToString(), y.ToString());
        }
    }

    /// <summary>
    /// 自然排序比较器。支持中文数值
    /// </summary>
    public class NaturalComparer : IComparer
    {
        public int Compare(object? x, object? y)
        {
            if (x == null || y == null) return 0;
            string? xStr = x.ToString();
            string? yStr = y.ToString();
            if (xStr == null || yStr == null) return 0;

            // 拆分数字与非数字部分
            List<string> xParts = xStr.SplitNumber();
            List<string> yParts = yStr.SplitNumber();
            int xIndex = 0, yIndex = 0;

            int count = Math.Min(xParts.Count, yParts.Count);
            for (int index = 0; index < count; index++)
            {
                string xPart = xParts[index];
                string yPart = yParts[index];
                // 两个都是整数，则比较数值
                if (xPart.TryToInt(out int xInt) && yPart.TryToInt(out int yInt))
                {
                    int result = xInt.CompareTo(yInt);
                    if (result != 0) return result;
                }
                // 否则，比较字符串
                else
                {
                    int result = string.Compare(xPart, yPart, StringComparison.Ordinal);
                    if (result != 0) return result;
                }
            }
            // 各部分比较相同，则比较长度
            return xStr.Length.CompareTo(yStr.Length);
        }
    }
}