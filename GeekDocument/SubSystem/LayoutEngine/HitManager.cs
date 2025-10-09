namespace GeekDocument.SubSystem.LayoutEngine
{
    public class HitManager
    {
        #region 单例

        private HitManager() { }
        public static HitManager Instance { get; } = new HitManager();

        #endregion

        public 布局元素? 直接命中元素 { get; set; } = null;
    }
}