using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekDocument.SubSystem.EditerSystem3
{
    public class 属性
    {
        public string 名称 { get; set; } = "";

        public string 值 { get; set; } = "";
    }

    public class 行内元素数据
    {
        public int 元素类型 { get; set; } = 0;

        public byte[] 元素数据 { get; set; } = Array.Empty<byte>();
    }

    public class 段落数据
    {
        public string 文本 { get; set; } = "";

        public List<属性> 属性列表 { get; set; } = new List<属性>();

        public List<行内元素数据> 行内元素列表 { get; set; } = new List<行内元素数据>();
    }

    public class 文档
    {
        public string 作者 { get; set; } = "";

        public string 简介 { get; set; } = "";

        public DateTime 创建日期 { get; set; } = DateTime.Now;

        public string 备注 { get; set; } = "";

        public List<string> 标签列表 { get; set; } = new List<string>();

        public List<属性> 页面属性列表 { get; set; } = new List<属性>();

        public List<段落数据> 段落数据列表 { get; set; } = new List<段落数据>();
    }
}