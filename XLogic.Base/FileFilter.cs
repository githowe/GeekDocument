using XLogic.Base.Ex;

namespace XLogic.Base
{
    /// <summary>
    /// 类型信息
    /// </summary>
    public class TypeInfo
    {
        public TypeInfo(string name, string extension)
        {
            Name = name;
            ExtensionList = extension.Split(",").ToList();
        }

        /// <summary>类型名</summary>
        public string Name { get; set; } = "文件";

        public List<string> ExtensionList { get; set; } = new List<string>();

        public override string ToString()
        {
            // 文件筛选器格式：图片|*.png;*.jpg;*.jpeg
            string extension = "";
            foreach (var item in ExtensionList) extension += $"*.{item};";
            extension = extension.TrimEnd(';');
            return $"{Name}|{extension}";
        }
    }

    /// <summary>
    /// 文件过滤器
    /// </summary>
    public class FileFilter
    {
        public List<TypeInfo> TypeList { get; set; } = new List<TypeInfo>();

        public override string ToString() => TypeList.ToListString("|");
    }
}