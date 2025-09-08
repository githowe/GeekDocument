using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using XLogic.Base.Ex;

namespace XLogic.WpfControl
{
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_Up", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "PART_Down", Type = typeof(RepeatButton))]
    public class NumericUpDown : Control
    {
        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
        }

        #region 依赖项属性

        #region 当前值

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(NumericUpDown),
            new PropertyMetadata(0, OnValueChanged));

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // 获取控件
            NumericUpDown control = (NumericUpDown)sender;
            // 获取新值
            int newValue = (int)e.NewValue;
            // 防止越界
            newValue = newValue.Limit(control.MinValue, control.MaxValue);
            // 更新值与文本框
            control.Value = newValue;
            if (control._textBox != null) control._textBox.Text = control.Value.ToString();
            control.ValueChanged?.Invoke();
        }

        #endregion

        #region 最大值

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(int), typeof(NumericUpDown),
            new PropertyMetadata(100000, OnMaxValueChanged));

        public int MaxValue
        {
            get => (int)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        private static void OnMaxValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // 获取控件
            var control = (NumericUpDown)sender;
            // 转换值
            var maxValue = (int)e.NewValue;
            // 最大值 < 最小值：更新最小值为最大值
            if (maxValue < control.MinValue) control.MinValue = maxValue;
            // 当前值 > 最大值：更新当前值为最大值
            if (control.Value > maxValue) control.Value = maxValue;
        }

        #endregion

        #region 最小值

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(int), typeof(NumericUpDown),
            new PropertyMetadata(0, OnMinValueChanged));

        public int MinValue
        {
            get => (int)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        private static void OnMinValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // 获取控件
            var control = (NumericUpDown)sender;
            // 转换值
            var minValue = (int)e.NewValue;
            // 最小值 > 最大值：更新最大值为最小值
            if (minValue > control.MaxValue) control.MaxValue = minValue;
            // 当前值 < 最小值：更新当前值为最小值
            if (control.Value < minValue) control.Value = minValue;
        }

        #endregion

        #endregion

        #region 命令

        /// <summary>上调数值</summary>
        private readonly RoutedUICommand _upCommand = new RoutedUICommand("UpValue", "UpValue", typeof(NumericUpDown));
        /// <summary>下调数值</summary>
        private readonly RoutedUICommand _downCommand = new RoutedUICommand("DownValue", "DownValue", typeof(NumericUpDown));
        /// <summary>回车按下</summary>
        private readonly RoutedUICommand _enterDownCommand = new RoutedUICommand("EnterDown", "EnterDown", typeof(NumericUpDown));

        #endregion

        #region 事件

        public Action? ValueChanged { get; set; } = null;

        public Action? OnValueConfirm { get; set; } = null;

        #endregion

        #region 公开方法

        /// <summary>
        /// 应用模板
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InitControl();
            InitCommands();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 初始化控件
        /// </summary>
        private void InitControl()
        {
            // 主网格
            Grid? mainGrid = GetTemplateChild("MainGrid") as Grid;
            mainGrid.MouseWheel += MainGrid_MouseWheel;
            // 文本框
            _textBox = GetTemplateChild("PART_TextBox") as TextBox;
            _textBox.Text = Value.ToString();
            _textBox.LostFocus += (_, _) => Value = ToInt(_textBox.Text);
            // 上调按钮
            var repeatUp = GetTemplateChild("PART_Up") as RepeatButton;
            repeatUp.Command = _upCommand;
            // 下调按钮
            var repeatDown = GetTemplateChild("PART_Down") as RepeatButton;
            repeatDown.Command = _downCommand;
        }

        private void MainGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Value += e.Delta / 120;
        }

        /// <summary>
        /// 初始化命令
        /// </summary>
        private void InitCommands()
        {
            // 绑定每个命令执行的动作
            CommandBindings.Add(new CommandBinding(_upCommand, (a, b) => Value++));
            CommandBindings.Add(new CommandBinding(_downCommand, (a, b) => Value--));
            CommandBindings.Add(new CommandBinding(_enterDownCommand, (_, _) => OnEnterDown()));
            // 绑定快捷键至命令
            _textBox.InputBindings.Add(new KeyBinding(_upCommand, new KeyGesture(Key.Up)));
            _textBox.InputBindings.Add(new KeyBinding(_downCommand, new KeyGesture(Key.Down)));
            _textBox.InputBindings.Add(new KeyBinding(_enterDownCommand, new KeyGesture(Key.Enter)));
        }

        /// <summary>
        /// 字符串转整型
        /// </summary>
        private int ToInt(string source)
        {
            _ = int.TryParse(source, out int value);
            return value;
        }

        /// <summary>
        /// 当回车键按下时
        /// </summary>
        private void OnEnterDown()
        {
            Value = ToInt(_textBox.Text);
            OnValueConfirm?.Invoke();
        }

        #endregion

        #region 字段

        /// <summary>文本框控件</summary>
        private TextBox? _textBox;

        #endregion
    }
}