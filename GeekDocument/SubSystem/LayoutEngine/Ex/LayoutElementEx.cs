using GeekDocument.SubSystem.EditerSystemNew.Control;
using GeekDocument.SubSystem.LayoutEngine.Element;

namespace GeekDocument.SubSystem.LayoutEngine.Ex
{
    /// <summary>
    /// 布局元素扩展
    /// </summary>
    public static class LayoutElementEx
    {
        /// <summary>
        /// 获取根段落
        /// </summary>
        public static 段落 GetRootParagraph(this 布局元素 element)
        {
            while (true)
            {
                if (element.Parent != null) element = element.Parent;
                else break;
            }
            段落? root = element as 段落;
            if (root == null) throw new Exception("未找到根段落");
            return root;
        }

        public static Page GetPage(this 布局元素 element)
        {
            段落 root = element.GetRootParagraph();
            return root.OwnerPage;
        }
    }
}