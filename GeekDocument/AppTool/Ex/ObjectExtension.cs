using Newtonsoft.Json;
using System.Text;
using XLogic.Base.Ex;

namespace GeekDocument.AppTool.Ex;

public static class ObjectExtension
{
    public static string 序列化并压缩(this object obj)
    {
        string jsonData = JsonConvert.SerializeObject(obj);
        byte[] byteData = Encoding.UTF8.GetBytes(jsonData);
        byte[] compressedData = byteData.Compresse();
        return Convert.ToBase64String(compressedData);
    }
}