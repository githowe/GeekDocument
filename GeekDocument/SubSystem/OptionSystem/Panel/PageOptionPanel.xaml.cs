using GeekDocument.SubSystem.CacheSystem;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XLogic.Base.Ex;

namespace GeekDocument.SubSystem.OptionSystem.Panel;

public partial class PageOptionPanel : UserControl
{
    public PageOptionPanel() => InitializeComponent();

    public int PageWidth => double.Parse(Input_PageWidth.Text).RoundInt();

    public int Top => _paddingArray[0].RoundInt();

    public int Bottom => _paddingArray[1].RoundInt();

    public int Left => _paddingArray[2].RoundInt();

    public int Right => _paddingArray[3].RoundInt();

    public void Init(PageOption pageOption)
    {
        Input_PageWidth.Text = pageOption.PageWidth.ToString();
        Input_Padding_Top.Text = pageOption.PagePadding.Top.ToString();
        Input_Padding_Bottom.Text = pageOption.PagePadding.Bottom.ToString();
        Input_Padding_Left.Text = pageOption.PagePadding.Left.ToString();
        Input_Padding_Right.Text = pageOption.PagePadding.Right.ToString();
        Toggle_Link.IsChecked = CacheManager.Instance.Cache.Application.PagePaddingLink;

        _inputArray[0] = Input_Padding_Top;
        _inputArray[1] = Input_Padding_Bottom;
        _inputArray[2] = Input_Padding_Left;
        _inputArray[3] = Input_Padding_Right;
        _paddingArray[0] = pageOption.PagePadding.Top;
        _paddingArray[1] = pageOption.PagePadding.Bottom;
        _paddingArray[2] = pageOption.PagePadding.Left;
        _paddingArray[3] = pageOption.PagePadding.Right;

        Input_Padding_Top.GotFocus += Input_Padding_Top_GotFocus;
        Input_Padding_Top.KeyDown += Input_Padding_Top_KeyDown;
        Input_Padding_Top.LostFocus += Input_Padding_Top_LostFocus;

        Input_Padding_Bottom.GotFocus += Input_Padding_Bottom_GotFocus;
        Input_Padding_Bottom.KeyDown += Input_Padding_Bottom_KeyDown;
        Input_Padding_Bottom.LostFocus += Input_Padding_Bottom_LostFocus;

        Input_Padding_Left.GotFocus += Input_Padding_Left_GotFocus;
        Input_Padding_Left.KeyDown += Input_Padding_Left_KeyDown;
        Input_Padding_Left.LostFocus += Input_Padding_Left_LostFocus;

        Input_Padding_Right.GotFocus += Input_Padding_Right_GotFocus;
        Input_Padding_Right.KeyDown += Input_Padding_Right_KeyDown;
        Input_Padding_Right.LostFocus += Input_Padding_Right_LostFocus;
    }

    #region 上边距

    private void Input_Padding_Top_GotFocus(object sender, RoutedEventArgs e) => _current = Input_Padding_Top.Text;

    private void Input_Padding_Top_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        // 获取新输入值，没有变化则忽略
        string _newInput = Input_Padding_Top.Text;
        if (_newInput == _current) return;
        _current = _newInput;
        // 解析新值并通知更新
        OnPaddingChanged(0, double.Parse(Input_Padding_Top.Text));
    }

    private void Input_Padding_Top_LostFocus(object sender, RoutedEventArgs e)
    {
        string _newInput = Input_Padding_Top.Text;
        if (_newInput == _current) return;
        OnPaddingChanged(0, double.Parse(Input_Padding_Top.Text));
    }

    #endregion

    #region 下边距

    private void Input_Padding_Bottom_GotFocus(object sender, RoutedEventArgs e) => _current = Input_Padding_Bottom.Text;

    private void Input_Padding_Bottom_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        string _newInput = Input_Padding_Bottom.Text;
        if (_newInput == _current) return;
        _current = _newInput;
        OnPaddingChanged(1, double.Parse(Input_Padding_Bottom.Text));
    }

    private void Input_Padding_Bottom_LostFocus(object sender, RoutedEventArgs e)
    {
        string _newInput = Input_Padding_Bottom.Text;
        if (_newInput == _current) return;
        OnPaddingChanged(1, double.Parse(Input_Padding_Bottom.Text));
    }

    #endregion

    #region 左边距

    private void Input_Padding_Left_GotFocus(object sender, RoutedEventArgs e)
    {
        _current = Input_Padding_Left.Text;
    }

    private void Input_Padding_Left_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        string _newInput = Input_Padding_Left.Text;
        if (_newInput == _current) return;
        _current = _newInput;
        OnPaddingChanged(2, double.Parse(Input_Padding_Left.Text));
    }

    private void Input_Padding_Left_LostFocus(object sender, RoutedEventArgs e)
    {
        string _newInput = Input_Padding_Left.Text;
        if (_newInput == _current) return;
        OnPaddingChanged(2, double.Parse(Input_Padding_Left.Text));
    }

    #endregion

    #region 右边距

    private void Input_Padding_Right_GotFocus(object sender, RoutedEventArgs e)
    {
        _current = Input_Padding_Right.Text;
    }

    private void Input_Padding_Right_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        string _newInput = Input_Padding_Right.Text;
        if (_newInput == _current) return;
        _current = _newInput;
        OnPaddingChanged(3, double.Parse(Input_Padding_Right.Text));
    }

    private void Input_Padding_Right_LostFocus(object sender, RoutedEventArgs e)
    {
        string _newInput = Input_Padding_Right.Text;
        if (_newInput == _current) return;
        OnPaddingChanged(3, double.Parse(Input_Padding_Right.Text));
    }

    #endregion

    private void Toggle_Link_Click(object sender, RoutedEventArgs e)
    {
        CacheManager.Instance.Cache.Application.PagePaddingLink = Toggle_Link.IsChecked == true;
        CacheManager.Instance.SaveCache();
    }

    private void OnPaddingChanged(int changedIndex, double newValue)
    {
        // 计算比例并更新边距
        double ratio = newValue / _paddingArray[changedIndex];
        _paddingArray[changedIndex] = newValue;

        if (Toggle_Link.IsChecked == false) return;
        // 同步更新其他边距
        for (int index = 0; index < _paddingArray.Length; index++)
        {
            if (index == changedIndex) continue;
            _paddingArray[index] *= ratio;
            _inputArray[index].Text = GetString(_paddingArray[index]);
        }
    }

    private string GetString(double value)
    {
        string result = value.ToString("0.00");
        // 小数部分为零，则去掉小数部分
        if (result.EndsWith(".00")) return result.Substring(0, result.Length - 3);
        return result;
    }

    private readonly TextBox[] _inputArray = new TextBox[4];
    private readonly double[] _paddingArray = new double[4];
    private string _current = "";
}