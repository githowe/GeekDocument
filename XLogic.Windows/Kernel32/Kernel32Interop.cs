using System.Runtime.InteropServices;

namespace XLogic.Windows.Kernel32
{
    public class Kernel32Interop
    {
        [DllImport("Kernel32.dll")]
        public static extern void AllocConsole();
    }
}