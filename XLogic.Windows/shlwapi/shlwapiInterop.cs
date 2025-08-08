using System.Runtime.InteropServices;

namespace XLogic.Windows.shlwapi
{
    public class shlwapiInterop
    {
        /// <summary>
        /// 操作系统自带的文件名排序函数
        /// </summary>
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int StrCmpLogicalW(string? x, string? y);

        public static int NaturalCompare(string? x, string? y) => StrCmpLogicalW(x, y);
    }
}