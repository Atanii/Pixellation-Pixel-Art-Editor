using System;
using System.Windows.Media;

namespace Pixellation.Utils
{
    public static class ColorUtils
    {
        public struct HSLColor
        {
            public float H { get; set; }
            public float S { get; set; }
            public float L { get; set; }
        }

        public static HSLColor ToHSL(int red, int green, int blue)
        {
            float R = red / 255f;
            float G = green / 255f;
            float B = blue / 255f;

            float min = Math.Min(Math.Min(R, G), B);
            float max = Math.Max(Math.Max(R, G), B);

            // Luminance
            float L = (max + min) / 2f;

            // Saturation
            float S;
            float H;

            if (min == max)
            {
                S = H = 0f;
                return new HSLColor
                {
                    H = H,
                    S = S,
                    L = L
                };
            }

            // L < .5
            if (L < 0.5f)
            {
                S = (max - min) / (max + min);
            }
            else
            {
                S = (max - min) / (2f - max - min);
            }

            // Hue
            if (max == R)
            {
                H = (G - B) / (max - min);
            }
            else if (max == G)
            {
                H = 2f + (B - R) / (max - min);
            }
            else
            {
                H = 4f + (R - G) / (max - min);
            }

            H *= 60;
            if (H < 0)
            {
                H += 360;
            }

            return new HSLColor
            {
                H = H,
                S = S,
                L = L
            };
        }

        public static Color ToRGB(HSLColor color)
        {
            float H = color.H;
            float S = color.S;
            float L = color.L;

            if (S == 0f)
            {
                var val = (byte)(L * 255);
                return Color.FromArgb(255, val, val, val);
            }

            float temp1;
            if (L < 0.5)
            {
                temp1 = L * (1 + S);
            }
            else
            {
                temp1 = (L + S) - (L * S);
            }

            float temp2 = 2 * L - temp1;

            H /= 360f;

            float tmpR = H + 0.333f;
            float tmpG = H;
            float tmpB = H - 0.333f;

            tmpR += tmpR < 0f ? 1 : 0;
            tmpR -= tmpR > 1f ? 1 : 0;

            tmpG += tmpG < 0f ? 1 : 0;
            tmpG -= tmpG > 1f ? 1 : 0;

            tmpB += tmpB < 0f ? 1 : 0;
            tmpB -= tmpB > 1f ? 1 : 0;

            float R, G, B;

            // Red
            if (6f * tmpR < 1)
            {
                R = temp2 + (temp1 - temp2) * 6 * tmpR;
            }
            else if (2f * tmpR < 1)
            {
                R = temp1;
            }
            else if (3f * tmpR < 2)
            {
                R = temp2 + (temp1 - temp2) * (0.666f - tmpR) * 6;
            }
            else
            {
                R = temp2;
            }

            // Green
            if (6f * tmpG < 1)
            {
                G = temp2 + (temp1 - temp2) * 6 * tmpG;
            }
            else if (2f * tmpG < 1)
            {
                G = temp1;
            }
            else if (3f * tmpG < 2)
            {
                G = temp2 + (temp1 - temp2) * (0.666f - tmpG) * 6;
            }
            else
            {
                G = temp2;
            }

            // Blue
            if (6f * tmpB < 1)
            {
                B = temp2 + (temp1 - temp2) * 6 * tmpB;
            }
            else if (2f * tmpB < 1)
            {
                B = temp1;
            }
            else if (3f * tmpB < 2)
            {
                B = temp2 + (temp1 - temp2) * (0.666f - tmpB) * 6;
            }
            else
            {
                B = temp2;
            }

            // Final R, G, B values
            byte _R = (byte)Math.Round(R * 255);
            byte _G = (byte)Math.Round(G * 255);
            byte _B = (byte)Math.Round(B * 255);

            return Color.FromRgb(_R, _G, _B);
        }
    }
}