using GeekDocument.AppTool.Ex;
using GeekDocument.SubSystem.LayoutEngine;

namespace GeekDocument.SubSystem.ArchiveSystem2;

public class 存档生成器
{
    private 存档生成器() { }
    public static 存档生成器 Instance { get; } = new 存档生成器();

    public 文档数据 生成存档(页面 page)
    {
        文档数据 result = new 文档数据();

        页面数据 pageData = new 页面数据
        {
            页面宽度 = page.页宽,
            内边距 = new 边线
            {
                Left = page.内边距.Left,
                Top = page.内边距.Top,
                Right = page.内边距.Right,
                Bottom = page.内边距.Bottom,
            },
            首行缩进 = page.首行缩进,
            段落间距 = page.段落间距,
        };
        foreach (var item in page.段落列表)
            pageData.元素列表.Add(生成段落元素信息(item));
        result.页面 = pageData;

        return result;
    }

    private 元素信息 生成段落元素信息(段落 段落)
    {
        元素信息 result = new 元素信息
        {
            Type = "段落",
            Version = "1.0",
        };

        段落元素属性 元素属性 = new 段落元素属性
        {
            文本 = 段落.获取文本(),
            水平对齐方式 = (int)段落.水平对齐,
            垂直对齐方式 = (int)段落.垂直对齐,
            段前距 = 段落.段前距,
            段后距 = 段落.段后距,
            左缩进 = 段落.左缩进,
            右缩进 = 段落.右缩进,
            首行缩进 = 段落.首行缩进,
            行间距 = 段落.行间距,
        };
        段落元素 段落元素 = new 段落元素 { 属性 = 元素属性.ToString() };
        foreach (var item in 段落.获取内嵌元素())
        {
            元素信息 行内元素信息 = 生成行内元素信息(item);
            段落元素.内嵌元素列表.Add(行内元素信息);
        }
        result.Data = 段落元素.序列化并压缩();

        return result;
    }

    private 元素信息 生成行内元素信息(行内元素 元素)
    {
        if (元素 is 图片 图片) return 生成图片元素信息(图片);
        else if (元素 is 表格 表格) return 生成表格元素信息(表格);
        throw new Exception("生成行内元素信息失败");
    }

    private 元素信息 生成图片元素信息(图片 图片)
    {
        元素信息 result = new 元素信息
        {
            Type = "图片",
            Version = "1.0",
        };

        图片元素属性 属性 = new 图片元素属性
        {
            图片源 = 图片.SourceHash,
            宽度 = 图片.ImageWidth,
            高度 = 图片.ImageHeight,
            像素画 = 图片.PixelArt,
            图注宽度模式 = (int)图片.CaptionWidthMode,
            图注最大宽度 = 图片.CaptionMaxWidth,
            图注固定宽度 = 图片.CaptionWidth,
            图注顶边距 = 图片.CaptionTopMargin,
        };
        图片元素 图片元素 = new 图片元素
        {
            属性 = 属性.ToString()
        };
        if (图片.图注段落 != null) 图片元素.图注信息 = 生成段落元素信息(图片.图注段落);
        result.Data = 图片元素.序列化并压缩();

        return result;
    }

    private 元素信息 生成表格元素信息(表格 表格)
    {
        元素信息 result = new 元素信息
        {
            Type = "表格",
            Version = "1.0",
        };

        表格元素属性 属性 = new 表格元素属性
        {
            行数 = 表格.行数,
            列数 = 表格.列数,
            边框粗细 = 表格.边框粗细,
        };
        表格元素 表格元素 = new 表格元素 { 属性 = 属性.ToString() };
        foreach (var item in 表格.单元格列表)
            表格元素.单元格列表.Add(生成单元格元素信息(item));
        result.Data = 表格元素.序列化并压缩();

        return result;
    }

    private 元素信息 生成单元格元素信息(单元格 单元格)
    {
        元素信息 result = new 元素信息
        {
            Type = "单元格",
            Version = "1.0",
        };

        单元格元素属性 属性 = new 单元格元素属性
        {
            行号 = 单元格.行号,
            列号 = 单元格.列号,
            宽度 = 单元格.Width,
            最小高度 = 单元格.MinHeight,
            内边距 = new 边线
            {
                Left = 单元格.Padding.Left,
                Top = 单元格.Padding.Top,
                Right = 单元格.Padding.Right,
                Bottom = 单元格.Padding.Bottom,
            },
            水平对齐方式 = (int)单元格.水平对齐,
            垂直对齐方式 = (int)单元格.垂直对齐,
            段间距 = 单元格.段间距,
        };
        单元格元素 单元格元素 = new 单元格元素 { 属性 = 属性.ToString() };
        foreach (var item in 单元格.段落列表)
            单元格元素.段落列表.Add(生成段落元素信息(item));
        result.Data = 单元格元素.序列化并压缩();

        return result;
    }
}