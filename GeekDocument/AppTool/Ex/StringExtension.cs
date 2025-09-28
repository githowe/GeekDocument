using Newtonsoft.Json;
using System.Text;
using XLogic.Base.Ex;

namespace GeekDocument.AppTool.Ex;

public static class StringExtension
{
    public static T? 解压并反序列化<T>(this string base64Data) where T : class
    {
        byte[] compressedData = Convert.FromBase64String(base64Data);
        byte[] byteData = compressedData.Uncompress();
        string jsonData = Encoding.UTF8.GetString(byteData);
        return JsonConvert.DeserializeObject<T>(jsonData);
    }
}