using GeekDocument.SubSystem.WindowSystem;
using System.Windows;
using XLogic.Wpf.Window;

namespace GeekDocument.SubSystem.EditerSystem3;

public partial class InsertTableDialog : XDialog
{
    public InsertTableDialog()
    {
        InitializeComponent();
        Loaded += InsertTableDialog_Loaded;
    }

    public int 行数 { get; set; } = 4;

    public int 列数 { get; set; } = 4;

    public double 单元格宽度 { get; set; } = 136;

    public double 单元格高度 { get; set; } = 24;

    private void InsertTableDialog_Loaded(object sender, RoutedEventArgs e)
    {
        Input_RowCount.Text = 行数.ToString();
        Input_ColCount.Text = 列数.ToString();
        Input_CellWidth.Text = 单元格宽度.ToString();
        Input_CellHeight.Text = 单元格高度.ToString();
        OK.Click += OK_Click;
    }

    private void OK_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            行数 = int.Parse(Input_RowCount.Text);
            列数 = int.Parse(Input_ColCount.Text);
            单元格宽度 = double.Parse(Input_CellWidth.Text);
            单元格高度 = double.Parse(Input_CellHeight.Text);
            DialogResult = true;
        }
        catch (Exception ex)
        {
            WM.ShowErrorTip(ex.Message);
        }
    }
}