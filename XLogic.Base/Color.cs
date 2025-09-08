using System.Diagnostics;

namespace XLogic.Base
{
    /// <summary>
    /// 颜色
    /// </summary>
    public class Color
    {
        #region 构造方法

        public Color() { }

        public Color(byte red, byte green, byte blue)
        {
            _red = red;
            _green = green;
            _blue = blue;
            UpdateHSB();
        }

        public Color(float hue, float saturation, float brightness)
        {
            _hue = hue;
            _saturation = saturation;
            _brightness = brightness;
            UpdateRGB();
        }

        public Color(string hexCode)
        {
            try
            {
                _red = byte.Parse(hexCode.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                _green = byte.Parse(hexCode.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                _blue = byte.Parse(hexCode.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                _red = 0;
                _green = 0;
                _blue = 0;
            }
            UpdateHSB();
        }

        #endregion

        #region 属性

        /// <summary>红：0 - 255</summary>
        public byte Red
        {
            get => _red;
            set
            {
                _red = value;
                UpdateHSB();
            }
        }

        /// <summary>绿：0 - 255</summary>
        public byte Green
        {
            get => _green;
            set
            {
                _green = value;
                UpdateHSB();
            }
        }

        /// <summary>蓝：0 - 255</summary>
        public byte Blue
        {
            get => _blue;
            set
            {
                _blue = value;
                UpdateHSB();
            }
        }

        /// <summary>色相：0 - 359</summary>
        public float Hue
        {
            get => _hue;
            set
            {
                _hue = value;
                UpdateRGB();
            }
        }

        /// <summary>饱和度：0 - 100</summary>
        public float Saturation
        {
            get => _saturation;
            set
            {
                _saturation = value;
                UpdateRGB();
            }
        }

        /// <summary>亮度：0 - 100</summary>
        public float Brightness
        {
            get => _brightness;
            set
            {
                if (_brightness != value)
                {
                    _brightness = value;
                    BrightnessUpdated?.Invoke();
                    UpdateRGB();
                }
            }
        }

        #endregion

        #region 事件

        /// <summary>亮度已更新</summary>
        public Action? BrightnessUpdated { get; set; } = null;

        #endregion

        #region 公开方法

        public override string ToString() => $"{_red},{_green},{_blue}";

        public void PrintHSB() => Trace.WriteLine($"{_hue},{_saturation},{_brightness}");

        /// <summary>
        /// 获取十六进制代码
        /// </summary>
        public string GetHexCode() => $"{_red:X2}{_green:X2}{_blue:X2}";

        public void UpdateTo(byte red, byte green, byte blue)
        {
            _red = red;
            _green = green;
            _blue = blue;
            UpdateHSB();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 更新红、绿、蓝
        /// </summary>
        private void UpdateRGB()
        {
            float Hue = _hue / 359f;
            float Saturation = _saturation / 100.0f;
            float Brightness = _brightness / 100.0f;

            byte red = 0, green = 0, blue = 0;
            if (Saturation == 0)
                red = green = blue = (byte)(Brightness * 255.0f + 0.5f);
            else
            {
                float h = (Hue - (float)System.Math.Floor(Hue)) * 6.0f;
                float f = h - (float)System.Math.Floor(h);
                float p = Brightness * (1.0f - Saturation);
                float q = Brightness * (1.0f - Saturation * f);
                float t = Brightness * (1.0f - (Saturation * (1.0f - f)));
                switch ((int)h)
                {
                    case 0:
                        red = (byte)(Brightness * 255.0f + 0.5f);
                        green = (byte)(t * 255.0f + 0.5f);
                        blue = (byte)(p * 255.0f + 0.5f);
                        break;
                    case 1:
                        red = (byte)(q * 255.0f + 0.5f);
                        green = (byte)(Brightness * 255.0f + 0.5f);
                        blue = (byte)(p * 255.0f + 0.5f);
                        break;
                    case 2:
                        red = (byte)(p * 255.0f + 0.5f);
                        green = (byte)(Brightness * 255.0f + 0.5f);
                        blue = (byte)(t * 255.0f + 0.5f);
                        break;
                    case 3:
                        red = (byte)(p * 255.0f + 0.5f);
                        green = (byte)(q * 255.0f + 0.5f);
                        blue = (byte)(Brightness * 255.0f + 0.5f);
                        break;
                    case 4:
                        red = (byte)(t * 255.0f + 0.5f);
                        green = (byte)(p * 255.0f + 0.5f);
                        blue = (byte)(Brightness * 255.0f + 0.5f);
                        break;
                    case 5:
                        red = (byte)(Brightness * 255.0f + 0.5f);
                        green = (byte)(p * 255.0f + 0.5f);
                        blue = (byte)(q * 255.0f + 0.5f);
                        break;
                }
            }

            _red = red;
            _green = green;
            _blue = blue;
        }

        /// <summary>
        /// 更新色相、饱和度、亮度
        /// </summary>
        private void UpdateHSB()
        {
            List<int> rgb = new List<int> { _red, _green, _blue };
            rgb.Sort();
            int max = rgb[2];
            int min = rgb[0];
            int dif = max - min;

            // 色相
            if (max > min)
            {
                if (_green == max) _hue = (_blue - _red) / (float)dif * 60f + 120f;
                else if (_blue == max) _hue = (_red - _green) / (float)dif * 60f + 240f;
                else if (_blue > _green) _hue = (_green - _blue) / (float)dif * 60f + 360f;
                else _hue = (_green - _blue) / (float)dif * 60f;
                if (_hue < 0) _hue += 360f;
            }
            else _hue = 0;
            // 饱和度
            _saturation = max == 0 ? 0 : dif / (float)max * 100f;
            // 亮度
            if (_brightness != max / 255f * 100f)
            {
                _brightness = max / 255f * 100f;
                BrightnessUpdated?.Invoke();
            }
        }

        #endregion

        #region 字段

        private byte _red = 0, _green = 0, _blue = 0;
        private float _hue = 0, _saturation = 0, _brightness = 0;

        #endregion
    }
}