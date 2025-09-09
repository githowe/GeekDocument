namespace GeekDocument.SubSystem.StyleSystem
{
    public class AppendStyleManager
    {
        #region 单例

        private AppendStyleManager() { }
        public static AppendStyleManager Instance { get; } = new AppendStyleManager();

        #endregion

        public void AddStyle(AppendStyle style)
        {
            style.GetHashCode();
        }

        private Dictionary<string, AppendStyle> _styleDict = new Dictionary<string, AppendStyle>();
    }
}