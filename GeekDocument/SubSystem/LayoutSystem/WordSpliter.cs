using System.Net.WebSockets;
using System.Text.RegularExpressions;

namespace GeekDocument.SubSystem.LayoutSystem;

public class WordSpliter
{
    private WordSpliter() { }
    public static WordSpliter Instance { get; } = new WordSpliter();

    public List<Word> ToWordList(string text)
    {
        List<Word> result = new List<Word>();

        foreach (var segment in StringSpliter.Instance.Split(text))
        {
            if (segment.Text == "") throw new Exception("片段为空");

            // 中文
            if (segment.Chinese)
            {
                // 获取字符索引
                int charIndex = segment.FirstCharIndex;
                // 遍历中文字符
                foreach (var c in segment.Text)
                {
                    Word word = new Word
                    {
                        Text = c.ToString(),
                        CharIndexList = new List<int> { charIndex },
                    };
                    result.Add(word);
                    charIndex++;
                }
            }
            // 英文
            else
            {
                // 获取字符索引
                int charIndex = segment.FirstCharIndex;
                // 遍历单词
                foreach (var item in SplitEnglish(segment.Text))
                {
                    List<int> charIndexList = new List<int>();
                    // 遍历字符
                    foreach (var c in item)
                    {
                        charIndexList.Add(charIndex);
                        charIndex++;
                    }
                    Word word = new Word
                    {
                        Text = item,
                        WordType = item != " " ? WordType.English : WordType.Space,
                        CharIndexList = charIndexList,
                    };
                    result.Add(word);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 将英文文本分割为单词
    /// </summary>
    private List<string> SplitEnglish(string text)
    {
        MatchCollection matcheResult = Regex.Matches(text, @"\.{3}|!!!|\w+|[^\w\s]| ");
        List<string> tokenList = new List<string>();
        foreach (Match match in matcheResult) tokenList.Add(match.Value);
        return tokenList;
    }
}