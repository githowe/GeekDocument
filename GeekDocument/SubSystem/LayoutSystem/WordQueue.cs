using GeekDocument.SubSystem.GlyphSystem;
using GeekDocument.SubSystem.OptionSystem;

namespace GeekDocument.SubSystem.LayoutSystem
{
    /// <summary>
    /// 字队列。用于生成文本行
    /// </summary>
    public class WordQueue
    {
        public List<Word> WordList { get; set; } = new List<Word>();

        /// <summary>
        /// 生成单行
        /// </summary>
        public TextLine GenerateSingleLine(double lineWidth)
        {
            TextLine result = new TextLine { LineWidth = lineWidth };
            while (_wordIndex < WordList.Count)
            {
                Word word = WordList[_wordIndex];
                _wordIndex++;
                result.DirectAddWord(word);
            }
            return result;
        }

        /// <summary>
        /// 生成行
        ///     从字列表中逐个获取字并拼接成行，直到取完
        /// </summary>
        public TextLine? GenerateLine(double lineWidth, bool firstLine, bool allowCompress, TextLine? prevLine, int firstLineIndent)
        {
            // 已取完
            if (_wordIndex >= WordList.Count) return null;

            // 创建文本行
            TextLine line = new TextLine
            {
                LineWidth = lineWidth,
                FirstLine = firstLine
            };
            // 设置首行缩进并初始化
            if (firstLine) line.Indent = firstLineIndent;
            line.Init();
            // 循环取字，拼接成行
            while (_wordIndex < WordList.Count)
            {
                // 取出字
                Word word = WordList[_wordIndex];
                // 字的宽度大于行宽时，需要拆分字
                if (word.Width > lineWidth)
                {
                    List<Word> wordList = SplitWord(word, line.CurrentWidth, lineWidth);
                    WordList.RemoveAt(_wordIndex);
                    WordList.InsertRange(_wordIndex, wordList);
                    continue;
                }

                _wordIndex++;
                // 当前字是空格 && 非首行 && 当前行宽为零
                if (word.WordType == WordType.Space && !firstLine && line.CurrentWidth == 0)
                {
                    // 将空格添加至上一行末尾，此举是为了将行首的所有空格移动至前一行末尾
                    if (prevLine != null)
                    {
                        prevLine.DirectAddWord(word);
                        continue;
                    }
                }
                // 添加字到行
                bool added = line.AddWord(word, allowCompress);
                // 添加失败，表示行生成完成
                if (!added)
                {
                    _wordIndex--;
                    break;
                }
            }
            // 返回行
            return line;
        }

        private List<Word> SplitWord(Word word, double currentWidth, double lineWidth)
        {
            List<Word> result = new List<Word>();

            List<GlyphImage> imageList = new List<GlyphImage>();
            List<int> indexList = new List<int>();

            // 循环取出所有字形
            while (word.GlyphImageList.Count > 0)
            {
                // 取一个字形图片与字符索引
                GlyphImage image = word.GlyphImageList[0];
                word.GlyphImageList.RemoveAt(0);
                int charIndex = word.CharIndexList[0];
                word.CharIndexList.RemoveAt(0);
                // 无法容纳刚取出的字形
                if (currentWidth + image.GlyphWidth > lineWidth)
                {
                    // 生成新字
                    Word newWord = GenerateWord(imageList, indexList, word);
                    result.Add(newWord);
                    // 新建列表并添加刚取出的数据
                    imageList = new List<GlyphImage> { image };
                    indexList = new List<int> { charIndex };
                    // 更新当前宽度
                    currentWidth = image.GlyphWidth;
                }
                // 可以容纳刚取出的字形
                else
                {
                    imageList.Add(image);
                    indexList.Add(charIndex);
                    currentWidth += image.GlyphWidth;
                }
            }
            // 还有未处理的字形
            if (imageList.Count > 0)
            {
                Word newWord = GenerateWord(imageList, indexList, word);
                result.Add(newWord);
            }

            return result;
        }

        private Word GenerateWord(List<GlyphImage> imageList, List<int> indexList, Word oldWord)
        {
            // 生成文本、计算字宽度
            string text = "";
            double width = 0;
            foreach (var item in imageList)
            {
                text += item.C;
                width += item.GlyphWidth;
            }
            // 创建字
            Word newWord = new Word
            {
                Text = text,
                WordType = oldWord.WordType,
                GlyphImageList = imageList,
                CharIndexList = indexList,
                Width = width,
                Interval = oldWord.Interval
            };
            // 返回字
            return newWord;
        }

        private int _wordIndex = 0;
    }
}