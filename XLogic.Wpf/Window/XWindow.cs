using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace XLogic.Wpf.Window
{
    public abstract class XWindow : System.Windows.Window
    {
        #region 属性

        /// <summary>自定义图标</summary>
        public string CustomIcon { get; set; } = "";

        #endregion

        #region 构造方法

        public XWindow()

        {
            Loaded += XWindow_Loaded;
        }

        #endregion

        #region 内部方法

        /// <summary>
        /// 添加窗口控制
        /// </summary>
        protected abstract void AddWindowControl();

        /// <summary>
        /// 窗口已加载
        /// </summary>
        protected virtual void XWindowLoaded() { }

        #endregion

        #region 窗口事件

        private void XWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 添加窗口控制
            AddWindowControl();
            // 设置左上角图标
            if (CustomIcon != "")
            {
                // 更新图标
                if (GetTemplateChild("WindowIcon") is Image icon)
                {
                    var uri = new Uri($"pack://application:,,,/Assets/{CustomIcon}.png");
                    var bitmap = new BitmapImage(uri);
                    icon.Source = bitmap;
                    icon.Width = bitmap.PixelWidth;
                    icon.Height = bitmap.PixelHeight;
                    // 设置图标区域大小
                    if (GetTemplateChild("IconArea") is ColumnDefinition column)
                    {
                        int margin = 36 - bitmap.PixelHeight;
                        int width = bitmap.PixelWidth + margin;
                        column.Width = new GridLength(width, GridUnitType.Pixel);
                    }
                }
            }
            // 调用已加载
            XWindowLoaded();
        }

        #endregion
    }
}