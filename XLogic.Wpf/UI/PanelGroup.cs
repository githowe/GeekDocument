using System.Windows.Input;

namespace XLogic.Wpf.UI
{
    /// <summary>
    /// 面板组
    /// </summary>
    public class PanelGroup : IKeyHandler
    {
        /// <summary>
        /// 注册面板
        /// </summary>
        public void RegisterPanel(IPanel panel)
        {
            if (_panelList.Contains(panel)) return;
            _panelList.Add(panel);
        }

        /// <summary>
        /// 清空面板
        /// </summary>
        public void ClearPanel()
        {
            _panelList.Clear();
            _currentPanel = null;
        }

        /// <summary>
        /// 激活面板
        /// </summary>
        public void ActivePanel(IPanel panel)
        {
            if (!_panelList.Contains(panel)) return;
            if (_focus) _currentPanel?.RemoveFocus();
            _currentPanel = panel;
            if (_focus) _currentPanel?.AddFocus();
        }

        /// <summary>
        /// 添加焦点
        /// </summary>
        public void AddFocus()
        {
            _currentPanel?.AddFocus();
            _focus = true;
        }

        /// <summary>
        /// 移除焦点
        /// </summary>
        public void RemoveFocus()
        {
            _currentPanel?.RemoveFocus();
            _focus = false;
        }

        public void HandleKeyDown(KeyEventArgs e) => _currentPanel?.HandleKeyDown(e);

        public void HandleKeyUp(KeyEventArgs e) => _currentPanel?.HandleKeyUp(e);

        /// <summary>面板列表</summary>
        private readonly List<IPanel> _panelList = new List<IPanel>();
        /// <summary>当前面板</summary>
        private IPanel? _currentPanel = null;

        private bool _focus = false;
    }
}