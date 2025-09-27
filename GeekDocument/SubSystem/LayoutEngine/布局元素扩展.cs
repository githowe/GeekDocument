namespace GeekDocument.SubSystem.LayoutEngine
{
    public static class 布局元素扩展
    {
        /// <summary>
        /// 获取根段落
        /// </summary>
        public static 段落 获取根段落(this 布局元素 element)
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
    }
}