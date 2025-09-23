namespace GeekDocument.SubSystem.LayoutEngine.Element;

public class 元素集
{
    public string Text { get; set; } = "";

    public List<布局元素> ElementList { get; set; } = new List<布局元素>();

    public bool InnerElement { get; set; } = false;

    public int Length => ElementList.Count;

    public void 生成中英文间距()
    {
        布局元素? 当前;
        布局元素? 下一个;
        for (int index = 0; index < ElementList.Count - 1; index++)
        {
            当前 = ElementList[index];
            下一个 = ElementList[index + 1];
            // 两个元素都为字时，需要添加字间距
            if (当前 is 字 当前字 && 下一个 is 字 下一个字)
            {
                if (当前字.字类型 == 字类型.Chinese && 下一个字.字类型 == 字类型.English ||
                    当前字.字类型 == 字类型.English && 下一个字.字类型 == 字类型.Chinese)
                    当前字.RightExtend = 当前字.最后一个字宽() * 0.25;
            }
        }
    }
}