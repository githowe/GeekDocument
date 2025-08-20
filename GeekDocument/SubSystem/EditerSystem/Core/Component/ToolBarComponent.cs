using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using XLogic.Base.UI;

namespace GeekDocument.SubSystem.EditerSystem.Core.Component
{
    /// <summary>
    /// 工具栏组件
    /// </summary>
    public class ToolBarComponent : Component<Editer>
    {
        #region 事件

        public event Action<string> ToolClick;

        public event Action<string> ToggleClick;

        public event Action<string> RadioClick;

        #endregion

        #region 生命周期

        protected override void Init()
        {
            foreach (var item in _host.MainToolBar.Children)
            {
                if (item is Button button) button.Click += Tool_Click;
                else if (item is ToggleButton toggleButton) toggleButton.Click += Toggle_Click;
                else if (item is RadioButton radioButton) radioButton.Click += Radio_Click;
            }
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 更新文本样式按钮（加粗、斜体）
        /// </summary>
        public void UpdateTextStyleButton()
        {

        }

        /// <summary>
        /// 更新对齐方式按钮（左对齐、居中、右对齐、两端对齐）
        /// </summary>
        public void UpdateAlignButton()
        {

        }

        #endregion

        #region 控件事件

        private void Tool_Click(object sender, RoutedEventArgs e) => ToolClick?.Invoke(((Button)sender).Name);

        private void Toggle_Click(object sender, RoutedEventArgs e) => ToggleClick?.Invoke(((ToggleButton)sender).Name);

        private void Radio_Click(object sender, RoutedEventArgs e) => RadioClick?.Invoke(((RadioButton)sender).Name);

        #endregion
    }
}