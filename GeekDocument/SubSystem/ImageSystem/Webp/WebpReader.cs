using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GeekDocument.SubSystem.ImageSystem.Webp
{
    public class WebpReader : IImageLoader
    {
        #region 构造方法

        public WebpReader() => _reader = Interop.CreateWebpReader();

        #endregion

        #region 属性

        /// <summary>宽度</summary>
        public int Width { get; private set; } = 0;

        /// <summary>高度</summary>
        public int Height { get; private set; } = 0;

        /// <summary>帧数</summary>
        public int FrameCount { get; private set; } = 0;

        /// <summary>动画持续时间。单位：毫秒</summary>
        public int Duration { get; set; } = 0;

        /// <summary>帧列表</summary>
        public List<ImageFrame> FrameList => _frameList;

        #endregion

        #region 公开方法

        /// <summary>
        /// 加载文件
        /// </summary>
        public void LoadImageFile(string path)
        {
            // 加载文件。加载时会进行解码
            int loadResult = Interop.LoadImageFile(_reader, path);
            if (loadResult != 0)
            {
                Console.WriteLine("加载图片文件失败。错误码：" + loadResult);
                return;
            }

            // 设置图片基本信息
            Width = Interop.GetImageWidth(_reader);
            Height = Interop.GetImageHeight(_reader);
            FrameCount = Interop.GetFrameCount(_reader);

            // 加载帧
            int frameSize = Width * Height * 4;
            while (true)
            {
                // 获取帧指针
                nint framePtr = Interop.GetFrame(_reader);
                if (framePtr == nint.Zero) break;
                // 获取帧数据指针
                nint dataPtr = Interop.GetFrameData(framePtr);
                // 创建帧数据
                ImageFrame frameData = new ImageFrame
                {
                    PixelData = new byte[frameSize],
                    Timestamp = Interop.GetFrameTimestamp(framePtr),
                };
                // 复制帧数据
                Marshal.Copy(dataPtr, frameData.PixelData, 0, frameSize);
                // 转换像素格式
                ConvertPixelFormat(frameData);
                // 添加到帧列表
                _frameList.Add(frameData);
            }
            // 清空帧
            Interop.ClearFrame(_reader);

            // 计算帧的持续时间
            for (int index = 0; index < _frameList.Count; index++)
            {
                ImageFrame frame = _frameList[index];
                // 第一帧的持续时间为当前帧的时间戳
                if (index == 0) frame.Duration = frame.Timestamp;
                // 后续帧的持续时间为当前帧的时间戳减去上一帧的时间戳
                else frame.Duration = frame.Timestamp - _frameList[index - 1].Timestamp;
            }

            // 修正持续时间为0ms的帧：提升至20ms
            foreach (var frame in _frameList)
                if (frame.Duration == 0) frame.Duration = 20;
            // 修正时间戳
            int time = 0;
            int totalDuration = 0;
            foreach (var frame in _frameList)
            {
                frame.Timestamp = time;
                time += frame.Duration;
                totalDuration += frame.Duration;
            }
            Duration = totalDuration;
        }

        public void Reset()
        {
            Width = 0;
            Height = 0;
            FrameCount = 0;
            Duration = 0;
            _frameList.Clear();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 转换帧的像素格式：RGBA -> BGRA
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ConvertPixelFormat(ImageFrame frame)
        {
            byte[] rgbaData = frame.PixelData;
            for (int index = 0; index < rgbaData.Length; index += 4)
            {
                byte r = rgbaData[index];
                byte g = rgbaData[index + 1];
                byte b = rgbaData[index + 2];
                byte a = rgbaData[index + 3];
                rgbaData[index] = b;
                rgbaData[index + 1] = g;
                rgbaData[index + 2] = r;
                rgbaData[index + 3] = a;
            }
        }

        #endregion

        #region 字段

        private readonly nint _reader;
        private readonly List<ImageFrame> _frameList = new List<ImageFrame>();

        #endregion
    }
}