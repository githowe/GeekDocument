using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekDocument.SubSystem.LayoutEngine.Tool;

public class 行内元素集
{
    public string Text { get; set; } = "";

    public List<行内元素> 行内元素列表 { get; set; } = new List<行内元素>();

    public bool InnerElement { get; set; } = false;

    public int Length => 行内元素列表.Count;

    public void 生成中英文间距()
    {

    }
}