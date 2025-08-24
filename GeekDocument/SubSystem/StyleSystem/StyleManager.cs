namespace GeekDocument.SubSystem.StyleSystem
{
    /// <summary>
    /// 样式管理器
    /// </summary>
    public class StyleManager
    {
        #region 单例

        private StyleManager() { }
        public static StyleManager Instance { get; } = new StyleManager();

        #endregion

        public List<StyleSheet> SheetList { get; set; } = new List<StyleSheet>();

        public void Init()
        {
            LoadSystemStyle();
        }

        /// <summary>
        /// 查找样式表
        /// </summary>
        public StyleSheet? FindStyleSheet(string name)
        {
            foreach (var sheet in SheetList)
                if (sheet.Name == name) return sheet;
            return null;
        }

        /// <summary>
        /// 加载系统样式
        /// </summary>
        private void LoadSystemStyle()
        {
            StyleSheet 标题 = new StyleSheet("Title");
            标题.AddItem("字号", StyleID.FontSize, "32");
            标题.AddItem("首行缩进", StyleID.FirstLineIndent, "0");
            StyleSheet 一级标头 = new StyleSheet("H1");
            一级标头.AddItem("字号", StyleID.FontSize, "24");
            一级标头.AddItem("首行缩进", StyleID.FirstLineIndent, "0");
            一级标头.AddItem("加粗", StyleID.Bold, "true");
            StyleSheet 二级标头 = new StyleSheet("H2");
            二级标头.AddItem("字号", StyleID.FontSize, "22");
            二级标头.AddItem("首行缩进", StyleID.FirstLineIndent, "0");
            二级标头.AddItem("加粗", StyleID.Bold, "true");
            StyleSheet 三级标头 = new StyleSheet("H3");
            三级标头.AddItem("字号", StyleID.FontSize, "20");
            三级标头.AddItem("首行缩进", StyleID.FirstLineIndent, "0");
            三级标头.AddItem("加粗", StyleID.Bold, "true");
            StyleSheet 四级标头 = new StyleSheet("H4");
            四级标头.AddItem("字号", StyleID.FontSize, "18");
            四级标头.AddItem("首行缩进", StyleID.FirstLineIndent, "0");
            四级标头.AddItem("加粗", StyleID.Bold, "true");

            SheetList.Add(标题);
            SheetList.Add(一级标头);
            SheetList.Add(二级标头);
            SheetList.Add(三级标头);
            SheetList.Add(四级标头);
        }
    }
}