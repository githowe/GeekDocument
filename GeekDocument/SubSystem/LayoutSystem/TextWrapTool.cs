using GeekDocument.SubSystem.EditerSystem.Define;

namespace GeekDocument.SubSystem.LayoutSystem
{
    /// <summary>
    /// 文本换行工具
    /// </summary>
    public class TextWrapTool
    {
        private TextWrapTool() { }
        public static TextWrapTool Instance { get; } = new TextWrapTool();

        public void WrapText(List<Word> wordList, List<TextLine> lineList, int lineWidth, LineAlignType alignType = LineAlignType.Justify, bool singleLine = false)
        {
            // 创建字队列
            WordQueue wordQueue = new WordQueue { WordList = wordList };
            // 生成单行
            if (singleLine)
            {
                TextLine line = wordQueue.GenerateSingleLine(lineWidth);
                lineList.Add(line);
            }
            // 生成多行
            else
            {
                TextLine? prevLine = null;
                while (true)
                {
                    // 只有两端对齐模式允许压缩字间距
                    bool allowCompress = alignType == LineAlignType.Justify;
                    TextLine? line = wordQueue.GenerateLine(lineWidth, lineList.Count == 0, allowCompress, prevLine);
                    if (line != null && !line.EmptyLine)
                    {
                        lineList.Add(line);
                        prevLine = line;
                    }
                    else break;
                }
            }
            // 没有行
            if (lineList.Count == 0) return;
            // 单行
            if (lineList.Count == 1)
            {
                TextLine firstLIne = lineList[0];
                // 设置对齐方式，两端对齐需要改成左对齐
                firstLIne.Align = alignType;
                if (firstLIne.Align == LineAlignType.Justify) firstLIne.Align = LineAlignType.Left;
                // 应用对齐
                firstLIne.ApplyAlign();
                return;
            }
            // 多行。先应用除最后一行外的所有行的对齐
            for (int index = 0; index < lineList.Count - 1; index++)
            {
                lineList[index].Align = alignType;
                lineList[index].ApplyAlign();
            }
            TextLine lastLine = lineList[lineList.Count - 1];
            lastLine.Align = alignType;
            // 如果是两端对齐，且最后一行的实际宽度小于等于行宽，则最后一行需要改左对齐
            if (lastLine.Align == LineAlignType.Justify && lastLine.RealWidth <= lineWidth)
                lastLine.Align = LineAlignType.Left;
            // 应用最后一行的对齐
            lastLine.ApplyAlign();
        }
    }
}