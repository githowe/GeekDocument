using GeekDocument.SubSystem.EditerSystem.Define;

namespace GeekDocument.SubSystem.LayoutSystem
{
    /// <summary>
    /// 文本行
    /// </summary>
    public class TextLine
    {
        #region 构造方法

        public TextLine() { }

        #endregion

        #region 属性

        /// <summary>行宽</summary>
        public double LineWidth { get; set; } = 0;

        /// <summary>缩进</summary>
        public double Indent { get; set; } = 0;

        /// <summary>当前行宽</summary>
        public double CurrentWidth => _currentWidth;

        /// <summary>实际宽度 = 当前宽度 - 右端连续空格宽度</summary>
        public double RealWidth => _currentWidth - GetRightSpaceWidth();

        public LineAlignType Align { get; set; } = LineAlignType.Justify;

        public List<Word> WordList { get; set; } = new List<Word>();

        /// <summary>横坐标列表</summary>
        public List<double> XList { get; set; } = new List<double>();

        /// <summary>首行</summary>
        public bool FirstLine { get; set; } = false;

        /// <summary>空行</summary>
        public bool EmptyLine => WordList.Count == 0;

        #endregion

        #region 公开方法

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            LineWidth -= Indent;
        }

        /// <summary>
        /// 添加字
        /// </summary>
        /// <returns>剩余空间无法容纳新字时，返回False</returns>
        public bool AddWord(Word word, bool allowCompress)
        {
            // 无法容纳新字
            if (_currentWidth + word.Width > LineWidth)
            {
                // 因为尾部的空格在两端对齐时会被忽略掉，所以为了保证正确拉伸或压缩
                // 尾部空格无需判断能否通过压缩来容纳，直接返回False
                if (word.WordType == WordType.Space) return false;
                // 允许压缩
                if (allowCompress)
                {
                    double 超宽 = _currentWidth + word.Width - LineWidth;
                    int 空格数量 = GetSpaceCount();
                    // 没有空格，无法压缩
                    if (空格数量 == 0) return false;
                    double 空格宽度 = GetSpaceWidth();
                    double 可压缩宽度 = 空格数量 * (空格宽度 * 0.2);
                    // 压缩空格后可以容纳新字
                    if (可压缩宽度 >= 超宽)
                    {
                        WordList.Add(word);
                        XList.Add(_currentWidth + Indent);
                        _currentWidth += word.Width + word.Interval;
                        return true;
                    }
                }
                return false;
            }
            // 添加新字
            WordList.Add(word);
            // 记录横坐标
            XList.Add(_currentWidth + Indent);
            // 更新当前宽度
            _currentWidth += word.Width + word.Interval;
            // 返回成功
            return true;
        }

        /// <summary>
        /// 直接添加字，不检查当前行宽是否足够。单行模式时调用此方法
        /// </summary>
        public void DirectAddWord(Word word)
        {
            // 添加新元素
            WordList.Add(word);
            // 记录横坐标
            XList.Add(_currentWidth + Indent);
            // 更新当前宽度
            _currentWidth += word.Width + word.Interval;
        }

        /// <summary>
        /// 应用对齐
        ///     根据对齐方式调整字的横坐标
        /// </summary>
        public void ApplyAlign()
        {
            // 如果是左对齐，直接返回，因为默认就是左对齐
            if (Align == LineAlignType.Left) return;

            // 两端对齐
            if (Align == LineAlignType.Justify)
            {
                // 计算实际行宽
                double realLineWidth = _currentWidth - GetRightSpaceWidth();
                // 拉伸宽度 = 行宽 - 实际行宽
                double stretchWidth = LineWidth - realLineWidth;

                // 无需拉伸
                if (stretchWidth == 0) return;

                // 重置坐标列表
                XList.Clear();
                double x = Indent;
                XList.Add(x);
                Word lastWord = WordList.Last();
                // 需要压缩
                if (stretchWidth < 0)
                {
                    int 空格数量 = GetSpaceCount() - GetRightSpaceCount();
                    double 压缩量 = -stretchWidth / 空格数量;
                    // 调整空格的宽度
                    foreach (var item in WordList)
                        if (item.WordType == WordType.Space) item.Width -= 压缩量;
                    // 遍历字，调整横坐标
                    foreach (var word in WordList)
                    {
                        if (word == lastWord) break;
                        x += word.Width + word.Interval;
                        XList.Add(x);
                    }
                }
                // 需要拉伸
                else
                {
                    // 计算实际字数
                    int realWordCount = WordList.Count - GetRightSpaceCount();
                    // 计算字间距
                    double wordInterval = stretchWidth / (realWordCount - 1);
                    // 遍历字，调整横坐标
                    foreach (var word in WordList)
                    {
                        if (word == lastWord) break;
                        x += word.Width + word.Interval + wordInterval;
                        XList.Add(x);
                    }
                }
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 获取空格数量
        /// </summary>
        private int GetSpaceCount()
        {
            int spaceCount = 0;
            foreach (var word in WordList)
                if (word.WordType == WordType.Space) spaceCount++;
            return spaceCount;
        }

        /// <summary>
        /// 获取空格宽度
        /// </summary>
        private double GetSpaceWidth()
        {
            foreach (var word in WordList)
                if (word.WordType == WordType.Space) return word.Width;
            return -1;
        }

        /// <summary>
        /// 获取右端空格宽度
        /// </summary>
        private double GetRightSpaceWidth()
        {
            double width = 0;
            // 反向遍历字
            for (int index = WordList.Count - 1; index >= 0; index--)
            {
                Word word = WordList[index];
                // 如果是空格，累加宽度
                if (word.WordType == WordType.Space) width += word.Width;
                // 否则，退出循环
                else break;
            }
            return width;
        }

        /// <summary>
        /// 获取右端空格数量
        /// </summary>
        private int GetRightSpaceCount()
        {
            int count = 0;
            // 反向遍历字
            for (int index = WordList.Count - 1; index >= 0; index--)
            {
                Word word = WordList[index];
                // 如果是空格，累加数量
                if (word.WordType == WordType.Space) count++;
                // 否则，退出循环
                else break;
            }
            return count;
        }

        #endregion

        #region 字段

        /// <summary>当前宽度。所有元素累加，且未拉伸的宽度</summary>
        private double _currentWidth = 0;

        #endregion
    }
}