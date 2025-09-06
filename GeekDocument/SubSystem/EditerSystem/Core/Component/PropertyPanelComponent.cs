using GeekDocument.SubSystem.EditerSystem.Control.PropertyPanel;
using GeekDocument.SubSystem.EditerSystem.Define;
using GeekDocument.SubSystem.EditerSystem.Define.BlockDerive;
using System.Windows;
using System.Windows.Controls;
using XLogic.Base.UI;

namespace GeekDocument.SubSystem.EditerSystem.Core.Component
{
    /// <summary>
    /// 属性面板组件
    /// </summary>
    public class PropertyPanelComponent : Component<Editer>
    {
        protected override void Enable()
        {
            _panelBox = _host.PropertyPanelBox;
            // 更新属性面板
            UpdatePropertyPanel(GetComponent<PageComponent>().CurrentBlock);
            // 初始化滚动条
            InitScrollBar();
            // 监听面板控制按钮
            _host.Tool_Open.Click += Tool_Open_Click;
            _host.Tool_Close.Click += Tool_Close_Click;
            // 监听组件
            GetComponent<PageComponent>().CurrentBlockChanged += PageComponent_CurrentBlockChanged;
        }

        private void Tool_Open_Click(object sender, RoutedEventArgs e)
        {
            _host.RightArea.Width = new GridLength(320);
            _host.Tool_Open.Visibility = Visibility.Collapsed;
            _host.Tool_Close.Visibility = Visibility.Visible;
        }

        private void Tool_Close_Click(object sender, RoutedEventArgs e)
        {
            _host.RightArea.Width = _zeroLength;
            _host.Tool_Open.Visibility = Visibility.Visible;
            _host.Tool_Close.Visibility = Visibility.Collapsed;
        }

        private void PageComponent_CurrentBlockChanged(Block block)
        {
            UpdatePropertyPanel(block);
        }

        private void UpdatePropertyPanel(Block block)
        {
            if (_currentBlock == block) return;
            _currentBlock = block;
            _panelBox.Children.Clear();
            PropertyPanel? panel = null;
            switch (_currentBlock.Type)
            {
                case BlockType.Text:
                    panel = new TextPropertyPanel { Block = (BlockText)_currentBlock };
                    break;
                case BlockType.SplitLine:
                    break;
                case BlockType.Code:
                    panel = new CodePropertyPanel { Block = (BlockCode)_currentBlock };
                    break;
                case BlockType.List:
                    break;
                case BlockType.Image:
                    break;
                case BlockType.Table:
                    break;
                case BlockType.Formula:
                    break;
                case BlockType.Chart:
                    break;
                case BlockType.Model:
                    break;
                case BlockType.Audio:
                    break;
            }
            if (panel != null)
            {
                _panelBox.Children.Add(panel);
                panel.Init();
                panel.PropertyChanged = PropertyChanged;
            }
        }

        private void InitScrollBar()
        {

        }

        private void PropertyChanged() => GetComponent<PageComponent>().HandlePropertyChanged();

        private Grid _panelBox;
        private Block? _currentBlock = null;
        private GridLength _zeroLength = new GridLength(0);
    }
}