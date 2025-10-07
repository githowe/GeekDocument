using GeekDocument.SubSystem.LayoutEngine.Element;

namespace GeekDocument.SubSystem.LayoutEngine
{
    public class 文档
    {
        public string 作者 { get; set; } = "";

        public string 简介 { get; set; } = "";

        public DateTime 创建日期 { get; set; } = DateTime.Now;

        public string 备注 { get; set; } = "";

        public List<string> 标签列表 { get; set; } = new List<string>();

        public 页面 页面 { get; set; } = new 页面();

        public void 添加段落(段落 段落)
        {
            段落.OwnerPage = 页面;
            页面.段落列表.Add(段落);
            页面.更新绘图对象("添加段落");
        }
    }
}