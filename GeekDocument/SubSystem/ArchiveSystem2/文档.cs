using Newtonsoft.Json;
using XLogic.Base.Ex;

namespace GeekDocument.SubSystem.ArchiveSystem2
{
    public class 边线
    {
        public double Left { get; set; } = 0;

        public double Top { get; set; } = 0;

        public double Right { get; set; } = 0;

        public double Bottom { get; set; } = 0;
    }

    public class 文档数据
    {
        public string 作者 { get; set; } = "";

        public string 简介 { get; set; } = "";

        public DateTime 创建日期 { get; set; } = DateTime.Now;

        public string 备注 { get; set; } = "";

        public List<string> 标签列表 { get; set; } = new List<string>();

        public 页面数据 页面 { get; set; } = new 页面数据();
    }

    public class 页面数据
    {
        public double 页面宽度 { get; set; } = 0;

        public 边线 内边距 { get; set; } = new 边线();

        public double 首行缩进 { get; set; } = 0;

        public double 块间距 { get; set; } = 0;

        public List<元素信息> 元素列表 { get; set; } = new List<元素信息>();
    }

    public class 元素信息
    {
        public string Type { get; set; } = "";

        public string Version { get; set; } = "1.0";

        public string Data { get; set; } = "";
    }

    public class 文本样式
    {
        public string 名称 { get; set; } = "";

        public string 值 { get; set; } = "";

        public int 起始索引 { get; set; } = 0;

        public int 结束索引 { get; set; } = 0;
    }

    public class 段落元素属性
    {
        #region 默认值

        public string 默认文本 { get; set; } = "";

        public int 默认水平对齐方式 { get; set; } = 3;

        public int 默认垂直对齐方式 { get; set; } = 2;

        public double 默认段前距 { get; set; } = 0;

        public double 默认段后距 { get; set; } = 0;

        public double 默认左缩进 { get; set; } = 0;

        public double 默认右缩进 { get; set; } = 0;

        public double 默认首行缩进 { get; set; } = 32;

        public double 默认行间距 { get; set; } = 4;

        #endregion

        public string 文本 { get; set; } = "";

        public int 水平对齐方式 { get; set; } = 0;

        public int 垂直对齐方式 { get; set; } = 0;

        public double 段前距 { get; set; } = 0;

        public double 段后距 { get; set; } = 0;

        public double 左缩进 { get; set; } = 0;

        public double 右缩进 { get; set; } = 0;

        public double 首行缩进 { get; set; } = 0;

        public double 行间距 { get; set; } = 0;

        public override string ToString()
        {
            List<string> list = new List<string>();
            if (文本 != 默认文本) list.Add(文本);
            else list.Add("");
            if (水平对齐方式 != 默认水平对齐方式) list.Add(水平对齐方式.ToString());
            else list.Add("");
            if (垂直对齐方式 != 默认垂直对齐方式) list.Add(垂直对齐方式.ToString());
            else list.Add("");
            if (段前距 != 默认段前距) list.Add(段前距.ToString());
            else list.Add("");
            if (段后距 != 默认段后距) list.Add(段后距.ToString());
            else list.Add("");
            if (左缩进 != 默认左缩进) list.Add(左缩进.ToString());
            else list.Add("");
            if (右缩进 != 默认右缩进) list.Add(右缩进.ToString());
            else list.Add("");
            if (首行缩进 != 默认首行缩进) list.Add(首行缩进.ToString());
            else list.Add("");
            if (行间距 != 默认行间距) list.Add(行间距.ToString());
            else list.Add("");
            // 序列化并转换为字节数组
            string jsonData = JsonConvert.SerializeObject(list);
            byte[] byteData = System.Text.Encoding.UTF8.GetBytes(jsonData);
            // 压缩字节数组并返回 Base64 字符串
            byte[] compressedData = byteData.Compresse();
            return Convert.ToBase64String(compressedData);
        }
    }

    public class 段落元素
    {
        public string 属性 { get; set; } = "";

        public List<元素信息> 内嵌元素列表 { get; set; } = new List<元素信息>();

        public List<文本样式> 样式列表 { get; set; } = new List<文本样式>();
    }
}