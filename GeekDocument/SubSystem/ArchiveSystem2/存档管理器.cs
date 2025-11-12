using GeekDocument.AppTool.Ex;
using GeekDocument.SubSystem.ImageSystem;
using GeekDocument.SubSystem.LayoutEngine;
using GeekDocument.SubSystem.LayoutEngine.Element;
using Newtonsoft.Json;
using System.Text;
using System.Windows;
using XLogic.Base.Ex;

namespace GeekDocument.SubSystem.ArchiveSystem2
{
    public class 存档管理器
    {
        #region 单例

        private 存档管理器() { }
        public static 存档管理器 Instance { get; } = new 存档管理器();

        #endregion

        #region 公开方法

        public byte[] 生成存档数据(文档 文档)
        {
            存档文件 存档 = new 存档文件
            {
                文档数据 = 生成文档数据(文档),
                资源列表 = 提取资源文件(文档.页面),
            };
            string jsonData = JsonConvert.SerializeObject(存档);
            byte[] byteData = Encoding.UTF8.GetBytes(jsonData);
            return byteData.Compresse();
        }

        public 文档 加载存档数据(byte[] sourceData)
        {
            byte[] byteData = sourceData.Uncompress();
            string jsonData = Encoding.UTF8.GetString(byteData);
            存档文件? 存档 = JsonConvert.DeserializeObject<存档文件>(jsonData);
            if (存档 == null) throw new Exception("反序列化存档文件失败");

            加载资源文件(存档.资源列表);
            return 加载文档数据(存档.文档数据);
        }

        #endregion

        #region 生成文档数据

        private 文档数据 生成文档数据(文档 文档)
        {
            文档数据 result = new 文档数据
            {
                作者 = 文档.作者,
                简介 = 文档.简介,
                创建日期 = 文档.创建日期,
                备注 = 文档.备注,
                标签列表 = 文档.标签列表,
            };

            页面 page = 文档.页面;
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
                Version = "1.1",
            };

            段落元素属性2 元素属性 = new 段落元素属性2
            {
                文本 = 段落.获取文本(),
                字体 = 段落.字体,
                字号 = 段落.字号,
                水平对齐方式 = (int)段落.水平对齐,
                垂直对齐方式 = (int)段落.垂直对齐,
                段前距 = 段落.段前距,
                段后距 = 段落.段后距,
                左缩进 = 段落.左缩进,
                右缩进 = 段落.右缩进,
                使用自定义首行缩进 = 段落.使用自定义首行缩进,
                自定义首行缩进 = 段落.自定义首行缩进,
                行间距 = 段落.行间距,
                使用自定义段间距 = 段落.使用自定义段间距,
                自定义段间距 = 段落.自定义段间距,
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
            else if (元素 is 公式 公式) return 生成公式元素信息(公式);
            else if (元素 is 代码 代码) return 生成代码元素信息(代码);
            else if (元素 is 列表 列表) return 生成列表元素信息(列表);
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
                行高列表 = 表格.获取全部行高(),
                列宽列表 = 表格.全部列宽,
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
                最小高度 = 0,
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

        private 元素信息 生成公式元素信息(公式 公式)
        {
            元素信息 result = new 元素信息
            {
                Type = "公式",
                Version = "1.0",
            };

            公式元素属性 属性 = new 公式元素属性
            {
                Latex = 公式.Latex,
                Size = 公式.Size,
                Color = 公式.Color,
            };
            公式元素 公式元素 = new 公式元素 { 属性 = 属性.ToString() };
            result.Data = 公式元素.序列化并压缩();

            return result;
        }

        private 元素信息 生成代码元素信息(代码 代码)
        {
            元素信息 result = new 元素信息
            {
                Type = "代码",
                Version = "1.0",
            };

            代码.更新源代码();
            代码元素属性 属性 = new 代码元素属性
            {
                源码 = 代码.源码,
                语言 = 代码.语言,
                字体 = 代码.字体,
                字号 = 代码.字号,
                行间距 = 代码.行间距,
                自动换行 = 代码.自动换行,
                显示行号 = 代码.显示行号,
                显示语言 = 代码.显示语言,
            };
            代码元素 代码元素 = new 代码元素 { 属性 = 属性.ToString() };
            result.Data = 代码元素.序列化并压缩();

            return result;
        }

        private 元素信息 生成列表元素信息(列表 列表)
        {
            元素信息 result = new 元素信息
            {
                Type = "列表",
                Version = "1.0",
            };

            列表元素属性 属性 = new 列表元素属性
            {
                行间距 = 列表.行间距,
                缩进 = 列表.缩进,
                MarkSize = 列表.MarkSize,
            };
            列表元素 列表元素 = new 列表元素 { 属性 = 属性.ToString() };
            列表.更新项信息列表();
            foreach (var item in 列表.项信息列表)
            {
                项数据 info = new 项数据
                {
                    Deep = item.Deep,
                    段落信息 = 生成段落元素信息(item.段落),
                };
                列表元素.项数据列表.Add(info);
            }
            result.Data = 列表元素.序列化并压缩();

            return result;
        }

        #endregion

        #region 提取资源文件

        private List<资源文件> 提取资源文件(页面 page)
        {
            List<资源文件> list = new List<资源文件>();

            // 提取资源文件
            foreach (var 段落 in page.段落列表)
                list.AddRange(提取资源文件(段落));
            // 去重
            List<资源文件> result = new List<资源文件>();
            List<string> hashList = new List<string>();
            foreach (var item in list)
            {
                if (hashList.Contains(item.哈希值)) continue;
                result.Add(item);
                hashList.Add(item.哈希值);
            }

            return result;
        }

        private List<资源文件> 提取资源文件(段落 段落)
        {
            List<资源文件> result = new List<资源文件>();

            foreach (var 图片 in 段落.提取图片元素())
            {
                ImageFileData? fileData = ImageManager.Instance.FindFileData(图片.SourceHash);
                if (fileData == null) throw new Exception("查找图片文件数据失败");
                资源文件 图片资源 = new 资源文件
                {
                    哈希值 = 图片.SourceHash,
                    类型 = fileData.Type,
                    数据 = fileData.Data,
                };
                result.Add(图片资源);
            }

            return result;
        }

        #endregion

        #region 加载文档数据

        private 文档 加载文档数据(文档数据 数据)
        {
            文档 result = new 文档
            {
                作者 = 数据.作者,
                简介 = 数据.简介,
                创建日期 = 数据.创建日期,
                备注 = 数据.备注,
                标签列表 = 数据.标签列表,
                页面 = 加载页面数据(数据.页面),
            };
            return result;
        }

        private 页面 加载页面数据(页面数据 数据)
        {
            页面 result = new 页面
            {
                页宽 = 数据.页面宽度,
                内边距 = new Thickness(数据.内边距.Left, 数据.内边距.Top, 数据.内边距.Right, 数据.内边距.Bottom),
                首行缩进 = 数据.首行缩进,
                段落间距 = 数据.段落间距,
            };
            foreach (var 元素信息 in 数据.元素列表)
            {
                段落 段落 = 加载段落数据(元素信息);
                段落.OwnerPage = result;
                result.段落列表.Add(段落);
            }
            result.更新绘图对象("加载段落");
            return result;
        }

        private 段落 加载段落数据(元素信息 段落元素信息)
        {
            段落元素? 段落元素 = 段落元素信息.Data.解压并反序列化<段落元素>();
            if (段落元素 == null) throw new Exception("加载段落数据失败");

            段落? result = null;
            // 1.0
            if (段落元素信息.Version == "1.0")
            {
                段落元素属性 属性 = new 段落元素属性(段落元素.属性);
                result = new 段落
                {
                    字体 = 属性.字体,
                    字号 = 属性.字号,
                    水平对齐 = (水平对齐方式)属性.水平对齐方式,
                    垂直对齐 = (垂直对齐方式)属性.垂直对齐方式,
                    段前距 = 属性.段前距,
                    段后距 = 属性.段后距,
                    左缩进 = 属性.左缩进,
                    右缩进 = 属性.右缩进,
                    首行缩进 = 属性.首行缩进,
                    行间距 = 属性.行间距,
                };
                if (属性.文本.Contains("\u200b"))
                    result.文本列表 = 属性.文本.Split("\u200b").ToList();
                else
                    result.文本列表 = new List<string> { 属性.文本 };
            }
            // 1.1
            else if (段落元素信息.Version == "1.1")
            {
                段落元素属性2 属性 = new 段落元素属性2(段落元素.属性);
                result = new 段落
                {
                    字体 = 属性.字体,
                    字号 = 属性.字号,
                    水平对齐 = (水平对齐方式)属性.水平对齐方式,
                    垂直对齐 = (垂直对齐方式)属性.垂直对齐方式,
                    段前距 = 属性.段前距,
                    段后距 = 属性.段后距,
                    左缩进 = 属性.左缩进,
                    右缩进 = 属性.右缩进,
                    使用自定义首行缩进 = 属性.使用自定义首行缩进,
                    自定义首行缩进 = 属性.自定义首行缩进,
                    行间距 = 属性.行间距,
                    使用自定义段间距 = 属性.使用自定义段间距,
                    自定义段间距 = 属性.自定义段间距,
                };
                if (属性.文本.Contains("\u200b"))
                    result.文本列表 = 属性.文本.Split("\u200b").ToList();
                else
                    result.文本列表 = new List<string> { 属性.文本 };
            }
            if (result == null) throw new Exception("加载段落数据失败");

            foreach (var 内嵌元素信息 in 段落元素.内嵌元素列表)
            {
                switch (内嵌元素信息.Type)
                {
                    case "图片":
                        result.内嵌元素列表.Add(加载图片数据(内嵌元素信息));
                        break;
                    case "表格":
                        result.内嵌元素列表.Add(加载表格数据(内嵌元素信息));
                        break;
                    case "公式":
                        result.内嵌元素列表.Add(加载公式数据(内嵌元素信息));
                        break;
                    case "代码":
                        result.内嵌元素列表.Add(加载代码数据(内嵌元素信息));
                        break;
                    case "列表":
                        result.内嵌元素列表.Add(加载列表数据(内嵌元素信息));
                        break;
                }
            }
            result.Init();
            return result;
        }

        private 图片 加载图片数据(元素信息 图片元素信息)
        {
            图片元素? 图片元素 = 图片元素信息.Data.解压并反序列化<图片元素>();
            if (图片元素 == null) throw new Exception("加载图片数据失败");

            图片元素属性 属性 = new 图片元素属性(图片元素.属性);
            图片 result = new 图片
            {
                SourceHash = 属性.图片源,
                ImageWidth = 属性.宽度,
                ImageHeight = 属性.高度,
                PixelArt = 属性.像素画,
                CaptionWidthMode = (图注宽度模式)属性.图注宽度模式,
                CaptionMaxWidth = 属性.图注最大宽度,
                CaptionWidth = 属性.图注固定宽度,
                CaptionTopMargin = 属性.图注顶边距,
            };
            if (图片元素.图注信息 != null)
            {
                result.图注段落 = 加载段落数据(图片元素.图注信息);
                result.AddChild(result.图注段落);
            }
            result.Init();
            return result;
        }

        private 表格 加载表格数据(元素信息 表格元素信息)
        {
            表格元素? 表格元素 = 表格元素信息.Data.解压并反序列化<表格元素>();
            if (表格元素 == null) throw new Exception("加载表格数据失败");

            表格元素属性 属性 = new 表格元素属性(表格元素.属性);
            表格 result = new 表格
            {
                行数 = 属性.行数,
                列数 = 属性.列数,
                边框粗细 = 属性.边框粗细,
            };
            result.Init();
            result.ClearCell();
            result.加载行高(属性.行高列表);
            result.加载列宽(属性.列宽列表);
            List<单元格> cellList = new List<单元格>();
            foreach (var 单元格信息 in 表格元素.单元格列表)
                cellList.Add(加载单元格数据(单元格信息));
            result.加载单元格(cellList);
            return result;
        }

        private 单元格 加载单元格数据(元素信息 单元格元素信息)
        {
            单元格元素? 单元格元素 = 单元格元素信息.Data.解压并反序列化<单元格元素>();
            if (单元格元素 == null) throw new Exception("加载单元格数据失败");

            单元格元素属性 属性 = new 单元格元素属性(单元格元素.属性);
            单元格 result = new 单元格
            {
                行号 = 属性.行号,
                列号 = 属性.列号,
                Width = 属性.宽度,
                Padding = new Thickness(属性.内边距.Left, 属性.内边距.Top, 属性.内边距.Right, 属性.内边距.Bottom),
                水平对齐 = (水平对齐方式)属性.水平对齐方式,
                垂直对齐 = (垂直对齐方式)属性.垂直对齐方式,
                段间距 = 属性.段间距,
            };
            foreach (var 段落信息 in 单元格元素.段落列表)
                result.添加段落(加载段落数据(段落信息));
            return result;
        }

        private 公式 加载公式数据(元素信息 公式元素信息)
        {
            公式元素? 公式元素 = 公式元素信息.Data.解压并反序列化<公式元素>();
            if (公式元素 == null) throw new Exception("加载公式数据失败");

            公式元素属性 属性 = new 公式元素属性(公式元素.属性);
            公式 result = new 公式
            {
                Latex = 属性.Latex,
                Size = 属性.Size,
                Color = 属性.Color,
            };
            result.Init();
            return result;
        }

        private 代码 加载代码数据(元素信息 代码元素信息)
        {
            代码元素? 代码元素 = 代码元素信息.Data.解压并反序列化<代码元素>();
            if (代码元素 == null) throw new Exception("加载代码数据失败");
            代码元素属性 属性 = new 代码元素属性(代码元素.属性);
            代码 result = new 代码
            {
                源码 = 属性.源码,
                语言 = 属性.语言,
                字体 = 属性.字体,
                字号 = 属性.字号,
                行间距 = 属性.行间距,
                自动换行 = 属性.自动换行,
                显示行号 = 属性.显示行号,
                显示语言 = 属性.显示语言,
            };
            result.Init();
            return result;
        }

        private 列表 加载列表数据(元素信息 列表元素信息)
        {
            列表元素? 列表元素 = 列表元素信息.Data.解压并反序列化<列表元素>();
            if (列表元素 == null) throw new Exception("加载列表数据失败");

            列表元素属性 属性 = new 列表元素属性(列表元素.属性);
            列表 result = new 列表
            {
                行间距 = 属性.行间距,
                缩进 = 属性.缩进,
                MarkSize = 属性.MarkSize,
            };
            foreach (var data in 列表元素.项数据列表)
            {
                项信息 info = new 项信息
                {
                    Deep = data.Deep,
                    段落 = 加载段落数据(data.段落信息),
                };
                result.项信息列表.Add(info);
            }
            result.Init();
            return result;
        }

        #endregion

        #region 加载资源文件

        private void 加载资源文件(List<资源文件> 资源列表)
        {
            foreach (var item in 资源列表)
            {
                ImageManager.Instance.AddFileData(new ImageFileData
                {
                    Hash = item.哈希值,
                    Type = item.类型,
                    Data = item.数据,
                });
                ImageManager.Instance.DecodeImage(item.哈希值);
            }
        }

        #endregion
    }
}