using System.Windows;
using System.Windows.Media;

namespace GeekDocument.SubSystem.LayoutEngine.Element
{
    public class 项信息
    {
        public 项信息() { }

        public 项信息(int deep, 段落 段落)
        {
            段落.Init();
            Deep = deep;
            this.段落 = 段落;
        }

        public int Deep { get; set; } = 0;

        public 段落 段落 { get; set; } = null!;

        public override string ToString()
        {
            string text = 段落.获取文本().Replace("\u002b", "");
            int length = Math.Min(text.Length, 20);
            return $"{Deep} - {text.Substring(0, length)}";
        }
    }

    public class 列表 : 行内元素
    {
        #region 构造方法

        public 列表()
        {
            Name = "列表";
            Icon = "List";
        }

        #endregion

        #region 属性

        public List<项信息> 项信息列表 { get; set; } = new List<项信息>();

        public double 行间距 { get; set; } = 8;

        public double 缩进 { get; set; } = 32;

        public double MarkSize { get; set; } = 4;

        #endregion

        #region 运行时属性

        public List<列表项> 项列表 { get; set; } = new List<列表项>();

        #endregion

        #region 布局元素方法

        public override void Init()
        {
            CanInput = true;
            生成项列表();
            foreach (var item in 项列表) item.Init();
            AddChildList(项列表.Cast<布局元素>().ToList());
        }

        public override void 测量()
        {
            if (double.IsNaN(MaxWidth)) throw new Exception("列表未设置最大宽度");

            foreach (var item in 项列表)
            {
                item.Width = MaxWidth;
                item.测量();
            }
            ActualWidth = MaxWidth;
            ActualHeight = 0;
            foreach (var item in 项列表)
                ActualHeight += item.ActualHeight;
            ActualHeight += 行间距 * (项列表.Count - 1);
        }

        public override void 重新测量()
        {
            Parent?.重新测量();
        }

        public override void 排列()
        {
            // 设置每个列表项的坐标，并排列
            double x = Left;
            double y = Top;
            foreach (var item in 项列表)
            {
                item.Left = x;
                item.Top = y;
                item.排列();
                y += item.ActualHeight + 行间距;
            }
        }

        public override void 渲染(DrawingContext? dc)
        {
            foreach (var item in 项列表)
                item.渲染(dc);
        }

        public override List<绘图对象> 获取绘图对象()
        {
            List<绘图对象> result = new List<绘图对象>();
            foreach (var item in 项列表)
                result.AddRange(item.获取绘图对象());
            return result;
        }

        public override 元素行 获取最近元素行(Point point)
        {
            段落 段落 = 获取最近列表项(point).段落;
            return 段落.获取最近元素行(point);
        }

        public override void 移入光标至开头()
        {
            项列表[0].移入光标至开头();
        }

        public override void 移入光标至末尾()
        {
            项列表.Last().移入光标至末尾();
        }

        public override void 从开头移出光标(布局元素 元素)
        {
            列表项 项 = (列表项)元素;
            int index = 项列表.IndexOf(项);
            if (index > 0) 项列表[index - 1].移入光标至末尾();
            else Parent?.从开头移出光标(this);
        }

        public override void 从末尾移出光标(布局元素 元素)
        {
            列表项 项 = (列表项)元素;
            int index = 项列表.IndexOf(项);
            if (index < 项列表.Count - 1) 项列表[index + 1].移入光标至开头();
            else Parent?.从末尾移出光标(this);
        }

        #endregion

        #region 行内元素方法

        public override List<图片> 提取图片元素()
        {
            List<图片> result = new List<图片>();
            foreach (var item in 项列表)
                result.AddRange(item.段落.提取图片元素());
            return result;
        }

        #endregion

        #region 公开方法

        public void 更新项信息列表()
        {
            项信息列表.Clear();
            foreach (var item in 项列表)
            {
                项信息列表.Add(new 项信息
                {
                    Deep = item.Deep,
                    段落 = item.段落,
                });
            }
        }

        public void 移动至上一项末尾(列表项 sender)
        {
            int itemIndex = 项列表.IndexOf(sender);
            if (itemIndex > 0)
            {
                列表项 prevItem = 项列表[itemIndex - 1];
                prevItem.移入光标至末尾();
            }
            else Parent?.从开头移出光标(this);
        }

        public void 移动至下一项开头(列表项 sender)
        {
            int itemIndex = 项列表.IndexOf(sender);
            if (itemIndex < 项列表.Count - 1)
            {
                列表项 nextItem = 项列表[itemIndex + 1];
                nextItem.移入光标至开头();
            }
            else Parent?.从末尾移出光标(this);
        }

        public void 处理退格(列表项 sender)
        {
            int itemIndex = 项列表.IndexOf(sender);
            if (itemIndex == 0) return;

            列表项 prevItem = 项列表[itemIndex - 1];
            // 前一项为空
            if (prevItem.IsEmpty)
            {
                sender.Deep = prevItem.Deep;
                项列表.Remove(prevItem);
                RemoveChild(prevItem);
                处理元素更新();
                sender.移动光标至开头();
            }
            else
            {
                int 光标索引 = prevItem.段落.全部行内元素.Count;
                // 合并段落元素
                List<行内元素> 元素列表 = prevItem.段落.全部行内元素;
                元素列表.AddRange(sender.段落.全部行内元素);
                prevItem.段落.更新文本与内嵌元素(元素列表);
                // 重新初始化前项
                prevItem.段落.Init();
                // 删除当前项
                项列表.Remove(sender);
                RemoveChild(sender);
                处理元素更新();
                prevItem.段落.移动光标至(光标索引);
            }
        }

        public void 处理回车(列表项 sender)
        {
            int itemIndex = 项列表.IndexOf(sender);
            段落 新段落 = new 段落
            {
                水平对齐 = sender.段落.水平对齐,
                垂直对齐 = sender.段落.垂直对齐,
                字体 = sender.段落.字体,
                字号 = sender.段落.字号,
                行间距 = sender.段落.行间距,
            };
            if (sender.段落.光标索引 == 0)
            {
                // 移动元素
                新段落.文本列表 = sender.段落.获取文本().Split("\u002b").ToList();
                新段落.内嵌元素列表 = sender.段落.获取内嵌元素();
                新段落.Init();
                // 置空旧项内容
                sender.段落.文本列表 = new List<string> { "" };
                sender.段落.内嵌元素列表.Clear();
                sender.段落.Init();
                // 创建新项并初始化
                列表项 newItem = new 列表项
                {
                    Deep = sender.Deep,
                    段落 = 新段落
                };
                newItem.Init();
                // 插入新项
                项列表.Insert(itemIndex + 1, newItem);
                AddChild(newItem);
                处理元素更新();
            }
            else if (sender.段落.光标索引 < sender.段落.全部行内元素.Count)
            {
                // 分割元素列表
                List<行内元素> 元素列表 = sender.段落.分割元素(sender.段落.光标索引);
                // 加载元素列表至新段落
                新段落.更新文本与内嵌元素(元素列表);
                // 初始化段落
                sender.段落.Init();
                新段落.Init();
                // 创建新项并初始化
                列表项 newItem = new 列表项
                {
                    Deep = sender.Deep,
                    段落 = 新段落
                };
                newItem.Init();
                // 插入新项
                项列表.Insert(itemIndex + 1, newItem);
                AddChild(newItem);
                处理元素更新();
            }
            else
            {
                新段落.Init();
                // 创建新项并初始化
                列表项 newItem = new 列表项
                {
                    Deep = sender.Deep,
                    段落 = 新段落
                };
                newItem.Init();
                // 插入新项
                项列表.Insert(itemIndex + 1, newItem);
                AddChild(newItem);
                处理元素更新();
            }

            新段落.移动光标至开头();
        }

        #endregion

        #region 私有方法

        private void 生成项列表()
        {
            foreach (var item in 项信息列表)
            {
                列表项 listItem = new 列表项
                {
                    行间距 = 行间距,
                    缩进 = 缩进,
                    MarkSize = MarkSize,
                    Deep = item.Deep,
                    段落 = item.段落,
                };
                项列表.Add(listItem);
            }
        }

        private 列表项 获取最近列表项(Point point)
        {
            列表项? 命中列表项 = null;
            for (int index = 项列表.Count - 1; index >= 0; index--)
            {
                列表项 项 = 项列表[index];
                double top = 项.Top;
                double bottom = top + 项.ActualHeight;
                if (top <= point.Y && point.Y <= bottom)
                {
                    命中列表项 = 项;
                    break;
                }
            }
            if (命中列表项 == null)
            {
                double 当前距离 = double.MaxValue;
                命中列表项 = 项列表[0];
                foreach (var 项 in 项列表)
                {
                    double top = 项.Top;
                    double bottom = top + 项.ActualHeight;
                    double 距离 = Math.Min(Math.Abs(point.Y - top), Math.Abs(point.Y - bottom));
                    if (距离 < 当前距离)
                    {
                        当前距离 = 距离;
                        命中列表项 = 项;
                    }
                }
            }
            return 命中列表项;
        }

        private void 处理元素更新()
        {
            测量();
            Parent?.重新测量();
        }

        #endregion

        #region 字段



        #endregion
    }
}