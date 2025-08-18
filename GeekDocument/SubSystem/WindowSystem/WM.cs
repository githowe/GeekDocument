using System.Media;
using System.Windows;

namespace GeekDocument.SubSystem.WindowSystem
{
    public class WM
    {
        public static MainWindow? Main { get; set; } = null;

        /// <summary>
        /// 显示提示
        /// </summary>
        public static void ShowTip(string tip, Window? owner = null, TipLevel level = TipLevel.Info)
        {
            AskDialog dialog = new AskDialog
            {
                Message = tip,
                Level = level,
                YesText = "确定",
                Group = ButtonGroup.Yes,
            };
            try
            {
                // 设置所属窗口可能会失败：例如程序退出时发生异常，需要显示提示窗口，但主窗口已关闭
                dialog.Owner = owner ?? Main;
            }
            catch (Exception)
            {
                dialog.Owner = null;
            }
            if (level is TipLevel.Info or TipLevel.Warning) SystemSounds.Asterisk.Play();
            else SystemSounds.Hand.Play();
            dialog.Show();
        }

        /// <summary>
        /// 显示错误提示
        /// </summary>
        public static void ShowErrorTip(string tip, Window? owner = null)
        {
            ShowTip(tip, owner, TipLevel.Error);
        }

        /// <summary>
        /// 显示询问窗口
        /// </summary>
        public static void ShowAskWindow(string message, Action<AskDialog, object?> action, object? obj,
            string yesText = "是", bool useCancel = true, TipLevel level = TipLevel.Info)
        {
            AskDialog dialog = new AskDialog()
            {
                Message = message,
                Owner = Main,
                YesText = yesText,
                Level = level,
                HandleResult = action,
                OperateObject = obj,
            };
            if (!useCancel) dialog.Group = ButtonGroup.YesNo;
            dialog?.Show();
        }

        /// <summary>
        /// 显示加载框
        /// </summary>
        public static void ShowLoadingDialog(ILoader loader)
        {
            LoadingDialog dialog = new LoadingDialog
            {
                Owner = Main,
                Loader = loader,
            };
            dialog.ShowDialog();
        }
    }
}