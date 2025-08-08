using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SmartLib.WPFStyle
{
    class MenuWidthConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 控件宽度
            double width = (double)value;
            if (width == 0) return 0;
            else return width - 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class TagToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? value.ToString() : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    class TagToImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 获取标签内容
            string tagString = (string)value;
            if (tagString == null) return null;

            Uri uri = new Uri(tagString, UriKind.Relative);
            BitmapImage image = new BitmapImage(uri);

            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    class GetTreeViewItemMargin : IValueConverter
    {
        public double Length { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as TreeViewItem;
            if (item == null) return new Thickness(0);

            return new Thickness(Length * item.GetDepth(), 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    class TagToTrackMargin : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string tagString = (string)value;
            if (tagString == null) return new Thickness();

            Thickness thickness = (Thickness)new ThicknessConverter().ConvertFromString(tagString);
            return thickness;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    class ListBoxItemBgConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 获取项索引
            int itemIndex = (int)value;
            // 根据奇偶设定不同颜色
            Color bgColor = itemIndex % 2 == 1 ? Color.FromArgb(8, 255, 255, 255) : Color.FromArgb(0, 255, 255, 255);
            // 返回画刷
            return new SolidColorBrush(bgColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    class ListBoxItemTextConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 获取项
            ListBoxItem item = (ListBoxItem)value;
            // 获取项文本
            return item.Content.ToString();
            /*string text = (string)item.Content;
            // 获取参数：项索引
            int itemIndex = ItemsControl.GetAlternationIndex(item) + 1;
            // 返回格式化后的文本
            return $"{itemIndex:00} {text}";*/
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}