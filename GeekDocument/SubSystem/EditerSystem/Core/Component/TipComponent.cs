using GeekDocument.SubSystem.EditerSystem.Control.PopupBar;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XLogic.Base.UI;

namespace GeekDocument.SubSystem.EditerSystem.Core.Component
{
    public class TipComponent : Component<Editer>
    {
        public void ShowTip(string tip)
        {
            if (_currentTip == null)
            {
                _currentTip = new OperateTip();
                _currentTip.Block_Tip.Text = tip;
                _host.TipBox.Children.Add(_currentTip);
            }
            // 获取鼠标相对于“TipBox”的坐标
            Point mousePoint = Mouse.GetPosition(_host.TipBox);
            Canvas.SetLeft(_currentTip, mousePoint.X + 16);
            Canvas.SetTop(_currentTip, mousePoint.Y + 16);
        }

        public void HideTip()
        {
            if (_currentTip != null)
            {
                _host.TipBox.Children.Remove(_currentTip);
                _currentTip = null;
            }
        }

        private OperateTip? _currentTip = null;
    }
}