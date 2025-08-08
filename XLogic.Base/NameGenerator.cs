namespace XLogic.Base
{
    /// <summary>
    /// 名称生成器
    /// </summary>
    public class NameGenerator
    {
        /// <summary>当前名称集</summary>
        public HashSet<string> NameSet { get; set; } = new HashSet<string>();

        /// <summary>名称构建器</summary>
        public Func<int, string>? NameBuilder { get; set; } = null;

        /// <summary>
        /// 生成名称
        /// </summary>
        public string GenerateName()
        {
            if (NameBuilder == null) throw new Exception("未设置“名称构建器”");
            int nameID = 1;
            while (true)
            {
                string name = NameBuilder(nameID);
                if (NameSet.Contains(name))
                {
                    nameID++;
                    continue;
                }
                return name;
            }
        }
    }
}