using System.Windows.Threading;

namespace XLogic.Wpf
{
    /// <summary>
    /// 定时器处理器
    /// </summary>
    public interface ITimerHandler
    {
        void Tick();
    }

    /// <summary>
    /// 应用程序定时器。运行在界面线程的定时器。
    /// </summary>
    public class AppTimer
    {
        #region 单例

        private AppTimer()
        {
            _timer.Interval = TimeSpan.FromMilliseconds(1000.0 / 50.0);
            _timer.Tick += Timer_Tick;
        }
        public static AppTimer Instance { get; } = new AppTimer();

        #endregion

        public void Start()
        {
            if (_handlerList.Count > 0) _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
            _handlerList = new List<ITimerHandler>();
        }

        /// <summary>
        /// 添加定时器处理
        /// </summary>
        public void AddTimerHandler(ITimerHandler handler)
        {
            List<ITimerHandler> newList = new List<ITimerHandler>(_handlerList);
            if (!newList.Contains(handler)) newList.Add(handler);
            _handlerList = newList;
            _timer.Start();
        }

        /// <summary>
        /// 移除定时器处理器
        /// </summary>
        public void RemoveTimerHandler(ITimerHandler handler)
        {
            List<ITimerHandler> newList = new List<ITimerHandler>(_handlerList);
            newList.Remove(handler);
            _handlerList = newList;
            if (_handlerList.Count == 0) _timer.Stop();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            foreach (var handler in _handlerList) handler.Tick();
        }

        /// <summary>定时器</summary>
        private readonly DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Normal);
        /// <summary>定时器处理器列表</summary>
        private List<ITimerHandler> _handlerList = new List<ITimerHandler>();
    }
}