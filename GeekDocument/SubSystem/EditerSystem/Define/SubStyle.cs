using GeekDocument.SubSystem.StyleSystem;

namespace GeekDocument.SubSystem.EditerSystem.Define;

public class SubStyle
{
    public List<AppendStyle> StyleList { get; set; } = new List<AppendStyle>();

    public void AddStyle(AppendStyle style)
    {
        // 遍历当前样式
        foreach (var item in StyleList)
        {
            // 替换同类型样式
            if (item.Type == style.Type)
            {
                StyleList.Remove(item);
                StyleList.Add(style);
                return;
            }
        }
        StyleList.Add(style);
    }

    public void RemoveStyle(AppendStyleType type)
    {
        // 遍历当前样式
        foreach (var item in StyleList)
        {
            // 移除同类型样式
            if (item.Type == type)
            {
                StyleList.Remove(item);
                return;
            }
        }
    }
}