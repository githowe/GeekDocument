using GeekDocument.SubSystem.EditerSystem.Define.BlockDerive;
using GeekDocument.SubSystem.ResourceSystem;
using GeekDocument.SubSystem.WindowSystem;
using System.Drawing.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace GeekDocument.SubSystem.EditerSystem.Control.PropertyPanel
{
    public partial class ImagePropertyPanel : PropertyPanel
    {
        public ImagePropertyPanel() => InitializeComponent();

        public BlockImage Block { get; set; }

        public override void Init()
        {
            // 源宽度、源高度
            Bar_SourceWidth.LoadProperty(Block.SourceWidth.ToString());
            Bar_SourceHeight.LoadProperty(Block.SourceHeight.ToString());
            // 目标渲染宽度、实际渲染宽度、实际渲染高度
            Bar_Width.LoadProperty(Block.RenderWidth.ToString());
            Bar_ActualWidth.LoadProperty(Block.RealRenderWidth.ToString());
            Bar_ActualHeight.LoadProperty(Block.RenderHeight.ToString());
            // 像素画、对齐方式
            Bar_PixelArt.LoadProperty(Block.PixelArt);

            // 字体、字号
            List<string> fontNameList = new List<string>();
            InstalledFontCollection fonts = new InstalledFontCollection();
            foreach (var font in fonts.Families) fontNameList.Add(font.Name);
            Bar_Font.LoadProperty(fontNameList, Block.FontFamily);
            Bar_Size.LoadProperty(Block.FontSize.ToString());
            // 帧数、帧率、播放控制
            Bar_FrameCount.LoadProperty(Block.FrameList.Count.ToString());
            Bar_Fps.LoadProperty(Block.Fps.ToString("0.00"));
            Bar_PlayBar.LoadProperty();
            Bar_PlayBar.AddTool(GetIcon("PlayControl/First"), "First", "第一帧");
            Bar_PlayBar.AddTool(GetIcon("PlayControl/Prev"), "Prev", "上一帧");
            _playButton = Bar_PlayBar.AddTool(GetIcon("PlayControl/Play"), "Play", "播放");
            _pauseButton = Bar_PlayBar.AddTool(GetIcon("PlayControl/Pause"), "Pause", "暂停");
            _pauseButton.Visibility = System.Windows.Visibility.Collapsed;
            Bar_PlayBar.AddTool(GetIcon("PlayControl/Next"), "Next", "下一帧");
            Bar_PlayBar.AddTool(GetIcon("PlayControl/Last"), "Last", "最后一帧");
            Bar_PlayBar.IsEnabled = false;
            // 布局
            Bar_Align.LoadProperty();
            Bar_Align.AddRadioButton(GetIcon("AlignLeft"), "Left");
            Bar_Align.AddRadioButton(GetIcon("AlignCenter"), "Center");
            Bar_Align.AddRadioButton(GetIcon("AlignRight"), "Right");
            Bar_Align.SetChecked(Block.Align.ToString());
            Bar_MarginTop.LoadProperty(Block.MarginTop.ToString());
            Bar_MarginBottom.LoadProperty(Block.MarginBottom.ToString());
            Bar_MarginLeft.LoadProperty(Block.MarginLeft.ToString());
            Bar_MarginRight.LoadProperty(Block.MarginRight.ToString());

            // 监听目标渲染宽度
            Bar_Width.TextChanged += Width_TextChanged;
            // 监听像素画
            Bar_PixelArt.Opened += PixelArt_Opened;
            Bar_PixelArt.Closed += PixelArt_Closed;
            // 监听字体、字号
            Bar_Font.SelectionChanged += Font_SelectionChanged;
            Bar_Size.TextChanged += Size_TextChanged;
            // 监听布局
            Bar_Align.ButtonChecked += Align_ButtonChecked;
            Bar_MarginTop.TextChanged += MarginTop_TextChanged;
            Bar_MarginBottom.TextChanged += MarginBottom_TextChanged;
            Bar_MarginLeft.TextChanged += MarginLeft_TextChanged;
            Bar_MarginRight.TextChanged += MarginRight_TextChanged;
        }

        private void Width_TextChanged(string text)
        {
            if (int.TryParse(text, out int width) && width >= 0)
            {
                // 设置目标渲染宽度
                Block.RenderWidth = width;
                PropertyChanged?.Invoke();
                // 更新实际渲染宽度
                Bar_ActualWidth.LoadProperty(Block.RealRenderWidth.ToString());
            }
        }

        private void PixelArt_Opened()
        {
            Block.PixelArt = true;
            PropertyChanged?.Invoke();
        }

        private void PixelArt_Closed()
        {
            Block.PixelArt = false;
            PropertyChanged?.Invoke();
        }

        private void Align_ButtonChecked(string name)
        {
            switch (name)
            {
                case "Left":
                    Block.Align = Define.LineAlignType.Left;
                    break;
                case "Center":
                    Block.Align = Define.LineAlignType.Center;
                    break;
                case "Right":
                    Block.Align = Define.LineAlignType.Right;
                    break;
            }
            PropertyChanged?.Invoke();
        }

        private void Font_SelectionChanged(string font)
        {
            Block.FontFamily = font;
            PropertyChanged?.Invoke();
        }

        private void Size_TextChanged(string text)
        {
            if (int.TryParse(text, out int size))
            {
                if (size < 1 || size > 32)
                {
                    WM.ShowErrorTip("有效字号范围：1 - 32");
                    return;
                }
                Block.FontSize = size;
                PropertyChanged?.Invoke();
            }
        }

        private void MarginTop_TextChanged(string text)
        {
            if (int.TryParse(text, out int marginTop))
            {
                Block.MarginTop = marginTop;
                PropertyChanged?.Invoke();
            }
        }

        private void MarginBottom_TextChanged(string text)
        {
            if (int.TryParse(text, out int marginBottom))
            {
                Block.MarginBottom = marginBottom;
                PropertyChanged?.Invoke();
            }
        }

        private void MarginLeft_TextChanged(string text)
        {
            if (int.TryParse(text, out int marginLeft))
            {
                Block.MarginLeft = marginLeft;
                PropertyChanged?.Invoke();
            }
        }

        private void MarginRight_TextChanged(string text)
        {
            if (int.TryParse(text, out int marginRight))
            {
                Block.MarginRight = marginRight;
                PropertyChanged?.Invoke();
            }
        }

        private ImageSource GetIcon(string name) => ImageResManager.Instance.GetIcon15($"{name}.png");

        private Button _playButton;
        private Button _pauseButton;
    }
}