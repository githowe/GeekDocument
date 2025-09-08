using GeekDocument.SubSystem.WindowSystem.ColorPick.Tool;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XLogic.Base.Ex;

namespace GeekDocument.SubSystem.WindowSystem.ColorPick;

public partial class ColorBar : UserControl
{
    #region 构造方法

    public ColorBar() => InitializeComponent();

    #endregion

    #region 属性

    /// <summary>颜色列表</summary>
    public List<Color> ColorList
    {
        get => _colorList;
        set
        {
            _colorList = value;
            UpdateColor();
        }
    }

    /// <summary>当前值：0 - 255</summary>
    public int Value
    {
        get => _value;
        set
        {
            _value = value;
            Grid_Mark.Margin = new Thickness(_value, 0, 0, 0);
        }
    }

    #endregion

    #region 事件

    public Action<int>? ValueChanged { get; set; } = null;

    #endregion

    #region 公开方法

    public void Init()
    {
        Image_Color.ImageSource = _imageSource;
        _tool = new BarTool(this);
        _tool.Init();
    }

    #endregion

    #region 工具方法

    public void CaptureBar() => CaptureMouse();

    public void ReleaseBar() => ReleaseMouseCapture();

    public void MoveToMouse()
    {
        Value = GetMouseX();
        ValueChanged?.Invoke(_value);
    }

    public void Drag()
    {
        Value = GetMouseX();
        ValueChanged?.Invoke(_value);
    }

    #endregion

    #region 控件事件

    private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _tool.OnMouseDown(e.ChangedButton);
        Focus();
    }

    private void UserControl_MouseMove(object sender, MouseEventArgs e)
    {
        _tool.OnMouseMove();
    }

    private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
    {
        _tool.OnMouseUp(e.ChangedButton);
    }

    private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        int newValue = Value + e.Delta / 120;
        Value = newValue.Limit(0, 255);
        ValueChanged?.Invoke(_value);
    }

    private void UserControl_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key is Key.Left or Key.Down)
        {
            if (Value > 0) Value--;
            e.Handled = true;
        }
        if (e.Key is Key.Right or Key.Up)
        {
            if (Value < 255) Value++;
            e.Handled = true;
        }
        ValueChanged?.Invoke(_value);
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 更新颜色
    /// </summary>
    private void UpdateColor()
    {
        // 计算像素颜色值
        int bitsPerPixel = _imageSource.Format.BitsPerPixel;
        Int32Rect rect = new Int32Rect(0, 0, _imageWidth, _imageHeight);
        byte[] pixels = new byte[_imageWidth * _imageHeight * bitsPerPixel / 8];
        int stride = _imageWidth * bitsPerPixel / 8;
        int pixelOffset;
        for (int y = 0; y < _imageHeight; y++)
        {
            for (int x = 0; x < _imageWidth; x++)
            {
                pixelOffset = (y * _imageWidth + x) * bitsPerPixel / 8;
                pixels[pixelOffset + 0] = ColorList[x].B;
                pixels[pixelOffset + 1] = ColorList[x].G;
                pixels[pixelOffset + 2] = ColorList[x].R;
                pixels[pixelOffset + 3] = 255;
            }
        }
        // 写入像素
        _imageSource.WritePixels(rect, pixels, stride, 0);
    }

    private int GetMouseX() => (int)Mouse.GetPosition(this).X.Limit(0, 255);

    #endregion

    #region 字段

    private readonly int _imageWidth = 256;
    private readonly int _imageHeight = 1;
    private readonly WriteableBitmap _imageSource = new WriteableBitmap(256, 1, 96, 96, PixelFormats.Bgr32, null);

    private List<Color> _colorList = new List<Color>();
    private int _value = 0;

    private BarTool _tool;

    #endregion
}