using System.Diagnostics;

namespace GeekDocument.SubSystem.TimeSystem
{
    /// <summary>
    /// 应用级计时器
    /// </summary>
    public class AppWatch
    {
        #region 单例

        private AppWatch() { }
        public static AppWatch Instance { get; } = new AppWatch();

        #endregion

        public long Milliseconds => _stopwatch.ElapsedMilliseconds;

        public double DoubleMs => _stopwatch.ElapsedTicks / (double)Stopwatch.Frequency * 1000;

        public void Start()
        {
            _stopwatch.Start();
        }

        private readonly Stopwatch _stopwatch = new Stopwatch();
    }
}