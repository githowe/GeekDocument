using System.Windows;
using System.Windows.Media;

namespace GeekDocument.SubSystem.LayoutEngine
{
    public class 代码 : 行内元素
    {
        #region 构造方法

        public 代码()
        {
            Name = "代码";
            Icon = "Code";
        }

        #endregion

        #region 属性 

        public string 源码 { get; set; } = "public class Program\n{\n    static void Main()\n    {\n        Console.WriteLine(\"Hello World!\");\n    }\n}";

        public string 语言 { get; set; } = "C#";

        public string 字体 { get; set; } = "霞鹜文楷等宽";

        public double 字号 { get; set; } = 16;

        public int 行间距 { get; set; } = 2;

        public bool 自动换行 { get; set; } = false;

        public bool 显示行号 { get; set; } = true;

        public bool 显示语言 { get; set; } = false;

        #endregion

        #region 布局元素方法

        public override void Init()
        {
            CanInput = true;
            源码 = 源码.Replace("\r\n", "\n");
            源码 = 源码.Replace("\t", "    ");
            // 分割源码为行
            _源码行列表 = 源码.Split('\n').ToList();
            // 每行创建一个段落
            foreach (var item in _源码行列表)
            {
                段落 段落 = new 段落
                {
                    文本 = item,
                    字体 = 字体,
                    字号 = 字号,
                    行间距 = 行间距,
                    水平对齐 = 水平对齐方式.Left,
                    禁用缩进 = true,
                    纯文本模式 = true,
                };
                段落.Init();
                _段落列表.Add(段落);
            }
            // 添加子元素
            AddChildList(_段落列表.Cast<布局元素>().ToList());

            _codeBack.Freeze();
            _lineNumberBack.Freeze();
        }

        public override void 测量()
        {
            if (double.IsNaN(MaxWidth)) throw new Exception("代码未设置最大宽度");

            // 测量每一行代码
            foreach (var item in _段落列表)
            {
                // 代码块一般都是占用整个行宽，所以设置固定宽度
                item.Width = MaxWidth - _代码内边距 * 2;
                item.测量();
            }
            // 计算代码块的宽高
            ActualWidth = MaxWidth;
            ActualHeight = 0;
            foreach (var item in _段落列表)
                ActualHeight += item.ActualHeight;
            ActualHeight += 行间距 * (_段落列表.Count - 1);
            ActualHeight += _代码内边距 * 2;
        }

        public override void 重新测量()
        {
            Parent?.重新测量();
        }

        public override void 排列()
        {
            // 设置每个段落的坐标，并排列
            double x = Left + _代码内边距;
            double y = Top + _代码内边距;
            foreach (var item in _段落列表)
            {
                item.Left = x;
                item.Top = y;
                item.排列();
                y += item.ActualHeight + 行间距;
            }
        }

        public override void 渲染(DrawingContext? dc)
        {
            // 绘制代码背景
            DrawingContext self_dc = _绘图对象.RenderOpen();
            self_dc.DrawRectangle(_codeBack, null, new Rect(Left, Top, ActualWidth, ActualHeight));
            self_dc.Close();
            // 绘制代码
            foreach (var item in _段落列表)
                item.渲染(dc);
        }

        public override List<绘图对象> 获取绘图对象()
        {
            List<绘图对象> result = new List<绘图对象>();
            result.Add(_绘图对象);
            foreach (var item in _段落列表)
                result.AddRange(item.获取绘图对象());
            return result;
        }

        public override 元素行 获取最近元素行(Point point)
        {
            段落 段落 = 获取最近段落(point);
            return 段落.获取最近元素行(point);
        }

        public override void 从开头移出光标(布局元素 元素)
        {
            // 代码块的子元素只能是段落
            段落 段落 = (段落)元素;
            int index = _段落列表.IndexOf(段落);
            if (index > 0) _段落列表[index - 1].移动光标至末尾();
            else Parent?.从开头移出光标(this);
        }

        public override void 从末尾移出光标(布局元素 元素)
        {
            段落 段落 = (段落)元素;
            int index = _段落列表.IndexOf(段落);
            if (index < _段落列表.Count - 1) _段落列表[index + 1].移动光标至开头();
            else Parent?.从末尾移出光标(this);
        }

        #endregion

        #region 行内元素方法

        public override void 移入光标至开头()
        {
            _段落列表[0].移动光标至开头();
        }

        public override void 移入光标至末尾()
        {
            _段落列表.Last().移动光标至末尾();
        }

        #endregion

        #region 公开方法

        public void 合并段落(段落 sender)
        {
            int 段落索引 = _段落列表.IndexOf(sender);
            if (段落索引 == 0) return;

            段落 前段落 = _段落列表[段落索引 - 1];
            // 前段落为空，直接删除前段落
            if (前段落.全部行内元素.Count == 0)
            {
                _段落列表.Remove(前段落);
                RemoveChild(前段落);
                // 删除段落后需要重新测量与排列
                处理元素更新();
                sender.移动光标至开头();
            }
            // 与前段落合并
            else
            {
                int 光标索引 = 前段落.全部行内元素.Count;
                // 合并段落元素
                List<行内元素> 元素列表 = 前段落.全部行内元素;
                元素列表.AddRange(sender.全部行内元素);
                前段落.更新文本与内嵌元素(元素列表);
                // 重新初始化前段落
                前段落.Init();
                // 删除当前段落
                _段落列表.Remove(sender);
                RemoveChild(sender);
                处理元素更新();
                前段落.移动光标至(光标索引);
            }
        }

        public void 处理回车(段落 sender)
        {
            // 获取当前段落索引
            int 段落索引 = _段落列表.IndexOf(sender);
            // 克隆段落
            段落 新段落 = new 段落
            {
                水平对齐 = sender.水平对齐,
                字体 = sender.字体,
                字号 = sender.字号,
                禁用缩进 = true,
                行间距 = sender.行间距
            };

            if (sender.光标索引 == 0)
            {
                // 移动文本
                新段落.文本 = sender.获取文本();
                sender.文本 = "";
                // 重新初始化
                sender.Init();
                新段落.Init();
                // 插入新段落
                _段落列表.Insert(段落索引 + 1, 新段落);
                AddChild(新段落);
                处理元素更新();
            }
            else if (sender.光标索引 < sender.全部行内元素.Count)
            {
                List<行内元素> 元素列表 = sender.分割元素(sender.光标索引);
                新段落.更新文本与内嵌元素(元素列表);
                sender.Init();
                新段落.Init();
                _段落列表.Insert(段落索引 + 1, 新段落);
                AddChild(新段落);
                处理元素更新();
            }
            else
            {
                新段落.Init();
                _段落列表.Insert(段落索引 + 1, 新段落);
                AddChild(新段落);
                处理元素更新();
            }

            新段落.移动光标至开头();
        }

        #endregion

        #region 私有方法

        private 段落 获取最近段落(Point point)
        {
            段落? 命中段落 = null;
            // 首先通过纵坐标找到命中段落，当段落重叠时，优先命中最上层的段落
            for (int index = _段落列表.Count - 1; index >= 0; index--)
            {
                段落 段落 = _段落列表[index];
                Rect rect = new Rect(段落.Left, 段落.Top, 段落.ActualWidth, 段落.ActualHeight);
                if (rect.Top <= point.Y && point.Y <= rect.Bottom)
                {
                    命中段落 = 段落;
                    break;
                }
            }
            // 段落之间有间距时，会无法命中，此时通过最近距离找到命中段落
            if (命中段落 == null)
            {
                double 当前距离 = double.MaxValue;
                命中段落 = _段落列表[0];
                foreach (var 段落 in _段落列表)
                {
                    Rect rect = new Rect(段落.Left, 段落.Top, 段落.ActualWidth, 段落.ActualHeight);
                    double 距离 = Math.Min(Math.Abs(point.Y - rect.Top), Math.Abs(point.Y - rect.Bottom));
                    if (距离 < 当前距离)
                    {
                        当前距离 = 距离;
                        命中段落 = 段落;
                    }
                }
            }
            return 命中段落;
        }

        private void 处理元素更新()
        {
            // 代码的子元素全是段落，所以添加或删除段落时，必然会更新尺寸，所以直接重新测量然后通知父元素
            测量();
            Parent?.重新测量();
        }

        #endregion

        #region 字段

        private readonly 绘图对象 _绘图对象 = new 绘图对象();
        private readonly Brush _codeBack = new SolidColorBrush(Color.FromArgb(255, 24, 24, 24));
        private readonly Brush _lineNumberBack = new SolidColorBrush(Color.FromArgb(255, 16, 16, 16));

        private List<string> _源码行列表 = new List<string>();
        private readonly List<段落> _段落列表 = new List<段落>();

        private readonly double _代码内边距 = 16;

        #endregion
    }
}