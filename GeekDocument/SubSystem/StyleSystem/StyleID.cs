using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekDocument.SubSystem.StyleSystem
{
    /// <summary>
    /// 定义各种样式的唯一编号
    /// </summary>
    public class StyleID
    {
        #region 基础样式

        /// <summary>背景色</summary>
        public const int BackgroundColor = 0001;
        /// <summary>前景色</summary>
        public const int Color = 0002;

        #endregion

        #region 文本样式

        /// <summary>字体</summary>
        public const int FontFamily = 1001;
        /// <summary>字号。单位：像素</summary>
        public const int FontSize = 1002;
        /// <summary>加粗</summary>
        public const int Bold = 1003;
        /// <summary>倾斜</summary>
        public const int Italic = 1004;

        #endregion

        #region 布局样式

        /// <summary>左缩进</summary>
        public const int IndentLeft = 2001;
        /// <summary>右缩进</summary>
        public const int IndentRight = 2002;
        /// <summary>首行缩进</summary>
        public const int FirstLineIndent = 2003;
        /// <summary>对齐方式：左(0)、中(1)、右(2)、两端(3)</summary>
        public const int Align = 2004;
        /// <summary>行间距</summary>
        public const int LineSpace = 2005;
        /// <summary>上边距</summary>
        public const int TopSpace = 2006;
        /// <summary>下边距</summary>
        public const int BottomSpace = 2007;

        #endregion
    }
}