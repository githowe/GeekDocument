using XLogic.Base.Ex;
using XLogic.Wpf;
using XLogic.Wpf.Window;

namespace GeekDocument.SubSystem.WindowSystem
{
    /// <summary>
    /// 加载器接口
    /// </summary>
    public interface ILoader
    {
        /// <summary>获取进度最大值</summary>
        public int GetProgressMax();

        /// <summary>获取阶段列表</summary>
        public List<LoadStage> GetStageList();

        /// <summary>准备加载</summary>
        public void ReadyLoad();

        /// <summary>开始加载</summary>
        public void StartLoad();

        /// <summary>获取阶段</summary>
        public int GetStage();

        /// <summary>获取进度</summary>
        public int GetProgress();

        /// <summary>停止加载</summary>
        public void StopLoad();

        /// <summary>加载完成</summary>
        public bool LoadFinish();
    }

    /// <summary>
    /// 加载阶段
    /// </summary>
    public class LoadStage
    {
        /// <summary>阶段</summary>
        public int Stage { get; set; } = 0;

        /// <summary>标题</summary>
        public string Title { get; set; } = "正在加载...";
    }

    public partial class LoadingDialog : XDialog, ITimerHandler
    {
        #region 构造方法

        public LoadingDialog() => InitializeComponent();

        #endregion

        #region 属性

        /// <summary>加载器：用于获取进度</summary>
        public ILoader Loader { get; set; }

        #endregion

        #region 接口实现

        public void Tick()
        {
            // 获取当前阶段
            _stage = Loader.GetStage();
            if (_stage >= _stageMax)
            {
                AppTimer.Instance.RemoveTimerHandler(this);
                Close();
                return;
            }
            // 获取当前进度
            _progress = Loader.GetProgress();
            // 获取最大进度
            _progressMax = Loader.GetProgressMax();
            // 更新标题
            Title = $"{_stageList[_stage].Title}[{_progress}/{_progressMax}]";
            // 更新进度条
            Grid_Progress.Width = ((double)_progress / _progressMax * 500).RoundInt();
        }

        #endregion

        #region 窗口事件

        protected override void XWindowLoaded()
        {
            if (Loader == null) throw new Exception("加载器为空");

            // 准备加载
            Title = "准备加载...";
            Loader.ReadyLoad();
            // 获取阶段列表
            _stageList = Loader.GetStageList();
            _stageMax = _stageList.Count;
            // 开始加载
            Task.Run(Loader.StartLoad);
            // 启动定时刷新
            AppTimer.Instance.AddTimerHandler(this);
        }

        #endregion

        #region 字段

        /// <summary>当前阶段</summary>
        private int _stage = 0;
        /// <summary>最大阶段</summary>
        private int _stageMax = 0;
        /// <summary>阶段列表</summary>
        private List<LoadStage> _stageList = new List<LoadStage>();

        /// <summary>当前进度</summary>
        private int _progress = 0;
        /// <summary>最大进度</summary>
        private int _progressMax = 0;

        #endregion
    }
}