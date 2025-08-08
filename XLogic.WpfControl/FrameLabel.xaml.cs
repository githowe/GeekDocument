using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace XLogic.WpfControl
{
    /// <summary>
    /// 线框方向
    /// </summary>
    public enum FrameDirection
    {
        None,
        UpToDown,
        DownToUp,
    }

    public partial class FrameLabel : UserControl
    {
        public FrameLabel()
        {
            InitializeComponent();
            SizeChanged += FrameLabel_SizeChanged;
        }

        private void FrameLabel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateFrame();
        }

        static FrameLabel()
        {
            // 注册依赖项属性
            IconProperty = DependencyProperty.Register("Icon", typeof(string), typeof(FrameLabel), new FrameworkPropertyMetadata(OnIconChanged));
            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(FrameLabel), new FrameworkPropertyMetadata(OnTextChanged));
            TextColorProperty = DependencyProperty.Register("TextColor", typeof(Brush), typeof(FrameLabel), new FrameworkPropertyMetadata(OnTextColorChanged));
            FrameLineColorProperty = DependencyProperty.Register("FrameLineColor", typeof(Brush), typeof(FrameLabel), new FrameworkPropertyMetadata(OnFrameLineColorChanged));
            DirectionProperty = DependencyProperty.Register("Direction", typeof(FrameDirection), typeof(FrameLabel), new FrameworkPropertyMetadata(OnDirectionChanged));
        }

        #region 依赖项属性

        // 定义依赖项属性
        public static DependencyProperty IconProperty;
        public static DependencyProperty TextProperty;
        public static DependencyProperty TextColorProperty;
        public static DependencyProperty FrameLineColorProperty;
        public static DependencyProperty DirectionProperty;

        // 封装依赖项属性
        public string Icon
        {
            set => SetValue(IconProperty, value);
            get => (string)GetValue(IconProperty);
        }
        public string Text
        {
            set => SetValue(TextProperty, value);
            get => (string)GetValue(TextProperty);
        }
        public Brush TextColor
        {
            set => SetValue(TextColorProperty, value);
            get => (Brush)GetValue(TextColorProperty);
        }
        public Brush FrameLineColor
        {
            set => SetValue(FrameLineColorProperty, value);
            get => (Brush)GetValue(FrameLineColorProperty);
        }

        public FrameDirection Direction
        {
            get => (FrameDirection)GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }

        // 属性更改回调
        private static void OnIconChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FrameLabel control = sender as FrameLabel;
            control.UpdateIcon();
        }
        private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FrameLabel control = sender as FrameLabel;
            control.TheText.Text = control.Text;
        }
        private static void OnTextColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FrameLabel control = sender as FrameLabel;
            control.TheText.Foreground = control.TextColor;
        }
        private static void OnFrameLineColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FrameLabel control = sender as FrameLabel;
            control.Border_Left.BorderBrush = control.FrameLineColor;
            control.Border_Right.BorderBrush = control.FrameLineColor;
        }

        private static void OnDirectionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FrameLabel control = sender as FrameLabel;
            control.UpdateFrame();
        }

        #endregion

        public void UpdateIcon()
        {
            return;
            try
            {
                // 获取图标
                Image_Icon.Source = new BitmapImage(new Uri($"pack://application:,,,/Assets/Icon15/{Icon}.png"));
                IconBox.Visibility = Visibility.Visible;
            }
            catch (Exception)
            {
                IconBox.Visibility = Visibility.Collapsed;
                Image_Icon.Source = null;
            }
        }

        public void UpdateFrame()
        {
            if (ActualHeight == 0) return;

            int space = (int)(ActualHeight / 2) + 1;
            Border_Left.Height = space;
            Border_Right.Height = space;
            switch (Direction)
            {
                case FrameDirection.None:
                    Border_Left.BorderThickness = new Thickness(0, _borderThickness, 0, 0);
                    Border_Right.BorderThickness = new Thickness(0, _borderThickness, 0, 0);
                    Border_Left.VerticalAlignment = VerticalAlignment.Bottom;
                    Border_Right.VerticalAlignment = VerticalAlignment.Bottom;
                    break;
                case FrameDirection.UpToDown:
                    Border_Left.BorderThickness = new Thickness(_borderThickness, _borderThickness, 0, 0);
                    Border_Right.BorderThickness = new Thickness(0, _borderThickness, _borderThickness, 0);
                    Border_Left.VerticalAlignment = VerticalAlignment.Bottom;
                    Border_Right.VerticalAlignment = VerticalAlignment.Bottom;
                    break;
                case FrameDirection.DownToUp:
                    Border_Left.BorderThickness = new Thickness(_borderThickness, 0, 0, _borderThickness);
                    Border_Right.BorderThickness = new Thickness(0, 0, _borderThickness, _borderThickness);
                    Border_Left.VerticalAlignment = VerticalAlignment.Top;
                    Border_Right.VerticalAlignment = VerticalAlignment.Top;
                    break;
            }
        }

        private readonly int _borderThickness = 1;
    }
}