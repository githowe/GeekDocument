using System.Text.RegularExpressions;

namespace XLogic.Base.Ex
{
    public static class StringExtension
    {
        /// <summary>
        /// 拆分数值
        ///     将字符串拆分成数值与非数值部分。
        ///     例如“a1”拆分成“a”与“1”；“第一”拆分成“第”与“一”
        /// </summary>
        public static List<string> SplitNumber(this string source)
        {
            string pattern = @"([零一二三四五六七八九十百千万]+)|(\d+)|([^\d零一二三四五六七八九十百千万]+)";
            MatchCollection matcheResult = Regex.Matches(source, pattern);
            List<string> result = new List<string>();
            foreach (Match match in matcheResult) result.Add(match.Value);
            return result;
        }

        /// <summary>
        /// 将中文数字转换为整数。只能处理十万以下的数字
        /// </summary>
        public static int ChineseNumberToInt(this string source)
        {
            Dictionary<char, int> 数字表 = new()
            {
                ['零'] = 0,
                ['一'] = 1,
                ['二'] = 2,
                ['三'] = 3,
                ['四'] = 4,
                ['五'] = 5,
                ['六'] = 6,
                ['七'] = 7,
                ['八'] = 8,
                ['九'] = 9
            };
            Dictionary<char, int> 单位表 = new()
            {
                ['十'] = 10,
                ['百'] = 100,
                ['千'] = 1000,
                ['万'] = 10000
            };

            int 结果 = 0;
            // 临时存储当前数字
            int temp = 0;

            foreach (char c in source)
            {
                if (c == '零') continue;

                // 读取数字
                if (数字表.TryGetValue(c, out int 数字)) temp = 数字;
                // 读取单位
                else if (单位表.TryGetValue(c, out int 单位))
                {
                    结果 += temp == 0 ? 单位 : temp * 单位;
                    // 重置临时数字
                    temp = 0;
                }
            }
            // 加上最后的个位数
            结果 += temp;
            // 返回结果
            return 结果;
        }

        /// <summary>
        /// 尝试转换为整数。支持阿拉伯数字和中文数字的字符串
        /// </summary>
        public static bool TryToInt(this string source, out int result)
        {
            result = 0;
            if (string.IsNullOrEmpty(source)) return false;

            // 尝试匹配阿拉伯数值
            Match match = Regex.Match(source, @"\d+");
            if (match.Success)
            {
                result = int.Parse(match.Value);
                return true;
            }
            // 尝试匹配中文数值
            match = Regex.Match(source, @"[零一二三四五六七八九十百千万]+");
            if (match.Success)
            {
                result = source.ChineseNumberToInt();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 解析路径
        /// </summary>
        public static (string, string) ParsePath(this string source, string split = "\\")
        {
            string path;
            string name;

            TreeNodePath nodePath = new TreeNodePath(source, split);
            name = nodePath.NodeList[^1];
            nodePath.RemoveLast();
            path = nodePath.ToString();

            return (path, name);
        }

        /// <summary>
        /// 移除路径的结尾分割符
        /// </summary>
        public static string RemoveTailSplit(this string path)
        {
            if (path.EndsWith("\\")) return path[..^1];
            return path;
        }
    }
}