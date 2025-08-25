using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Btimap = System.Drawing.Bitmap;

namespace GeekDocument.SubSystem.ImageSystem.Gif
{
    public class GifReader : IImageLoader
    {
        public int Width { get; private set; } = 0;

        public int Height { get; private set; } = 0;

        /// <summary>动画持续时间。单位：毫秒</summary>
        public int Duration { get; set; } = 0;

        /// <summary>帧列表</summary>
        public List<ImageFrame> FrameList => _frameList;

        public void LoadImageFile(string path)
        {
            // 加载图片文件
            Image image = Image.FromFile(path);
            // 设置图片大小
            Width = image.Width;
            Height = image.Height;

            // 帧维度可以理解为帧的类型，图片本身并不包含维度信息，只是Image类在解析时会根据图片类型生成维度信息
            // 例如读取Gif图片时，会创建一个Time维度，然后将所有帧关联至该维度
            // 所以要读取帧列表，首先要创建Time维度

            // 创建帧维度
            FrameDimension dimension = new FrameDimension(FrameDimension.Time.Guid);
            // 获取该维度下的帧的数量
            int frameCount = image.GetFrameCount(dimension);

            // 延时数组。全部帧的延时都存储在该数组中，单位为毫秒
            int[] delayArray = new int[frameCount];
            // 设置默认延时
            Array.Fill(delayArray, 20);

            try
            {
                // 获取延时属性
                PropertyItem? delayProperty = image.GetPropertyItem(0x5100);
                if (delayProperty != null && delayProperty.Value != null)
                {
                    byte[] delayData = delayProperty.Value;
                    for (int index = 0; index < frameCount; index++)
                        delayArray[index] = BitConverter.ToInt32(delayData, index * 4) * 10;
                }
            }
            catch (Exception) { throw new Exception("获取延时属性异常"); }

            // 当前时间戳
            int timestamp = 0;
            // 读取帧
            for (int index = 0; index < frameCount; index++)
            {
                // 激活指定帧并生成位图
                image.SelectActiveFrame(dimension, index);
                Btimap bmp = new Btimap(image);

                // 创建需要读取像素数据的区域
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                // 锁定区域
                BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                // 创建像素数据并复制数据
                int frameSize = bmp.Width * bmp.Height * 4;
                byte[] pixelData = new byte[frameSize];
                Marshal.Copy(bmpData.Scan0, pixelData, 0, frameSize);
                // 解锁
                bmp.UnlockBits(bmpData);
                // 释放
                bmp.Dispose();

                // 创建帧数据
                ImageFrame frameData = new ImageFrame
                {
                    PixelData = pixelData,
                    Timestamp = timestamp,
                    Duration = delayArray[index],
                };
                // 添加到帧列表
                _frameList.Add(frameData);
                // 更新时间戳
                timestamp += delayArray[index];
            }
            // 设置总持续时间
            Duration = timestamp;

            image.Dispose();
        }

        public void Reset()
        {
            Width = 0;
            Height = 0;
            Duration = 0;
            _frameList.Clear();
        }

        private readonly List<ImageFrame> _frameList = new List<ImageFrame>();
    }
}