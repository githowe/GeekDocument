using System.IO.Compression;

namespace XLogic.Base.Ex
{
    public static class ByteArrayExtension
    {
        /// <summary>
        /// 压缩字节数组
        /// </summary>
        public static byte[] Compresse(this byte[] source)
        {
            byte[] result;
            // 创建内存流
            using (MemoryStream output = new MemoryStream())
            {
                // 创建压缩流，压缩后的数据将写入内存流
                using (DeflateStream compress = new DeflateStream(output, CompressionMode.Compress))
                {
                    // 写入数据进行压缩
                    compress.Write(source, 0, source.Length);
                }
                // 获取压缩后的数据
                result = output.ToArray();
            }
            return result;
        }

        /// <summary>
        /// 解压缩字节数组
        /// </summary>
        public static byte[] Uncompress(this byte[] source)
        {
            byte[] result;
            // 创建输入流，读取压缩数据
            using (MemoryStream input = new MemoryStream(source))
            {
                // 创建输出流，存储解压后的数据
                using (MemoryStream output = new MemoryStream())
                {
                    // 创建解压缩流，从输入流中读取压缩数据进行解压，并写入输出流
                    using (DeflateStream decompress = new DeflateStream(input, CompressionMode.Decompress))
                    {
                        // 解压数据到内存流
                        decompress.CopyTo(output);
                    }
                    // 获取解压后的数据
                    result = output.ToArray();
                }
            }
            return result;
        }
    }
}