using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace instance.id.EATK.Extensions
{
    public static class DistinctColors
    {
        public static Color Red = new Color(230, 25, 75);
        public static Color Green = new Color(60, 180, 75);
        public static Color Yellow = new Color(255, 225, 25);
        public static Color Blue = new Color(0, 130, 200);
        public static Color Orange = new Color(245, 130, 48);
        public static Color Purple = new Color(145, 30, 180);
        public static Color Cyan = new Color(70, 240, 240);
        public static Color Magenta = new Color(240, 50, 230);
        public static Color Lime = new Color(210, 245, 60);
        public static Color Pink = new Color(250, 190, 190);
        public static Color Teal = new Color(0, 128, 128);
        public static Color Lavender = new Color(230, 190, 255);
        public static Color Brown = new Color(170, 110, 40);
        public static Color Beige = new Color(255, 250, 200);
        public static Color Maroon = new Color(128, 0, 0);
        public static Color Mint = new Color(170, 255, 195);
        public static Color Olive = new Color(128, 128, 0);
        public static Color Coral = new Color(255, 215, 180);
        public static Color Navy = new Color(0, 0, 128);
        public static Color Grey = new Color(128, 128, 128);
        public static Color White = new Color(255, 255, 255);
        public static Color Black = new Color(0, 0, 0);
    }

    public static class ColorHelper
    {
        public static Color GetRandomColor()
        {
            var bright = RandomBrightColor();
            return bright.ToRGBA();
        }

        public static string ToHex(this Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }

        public enum ColorType
        {
            Bright,
            Pastel,
            Dark
        }

        public static HSLColor RandomBrightColor()
        {
            HSLColor randomColor = new HSLColor();
            randomColor.h = Random.Range(0, 360);
            randomColor.s = Random.Range(0.7f, 1f);
            randomColor.l = Random.Range(0.45f, 0.6f);
            randomColor.a = 1;
            return randomColor;
        }

        public static HSLColor RandomPastelColor()
        {
            HSLColor randomColor = new HSLColor();
            randomColor.h = Random.Range(0, 360);
            randomColor.s = Random.Range(0.3f, 0.4f);
            randomColor.l = Random.Range(0.45f, 0.6f);
            randomColor.a = 1;

            return randomColor;
        }

        public static HSLColor RandomDarkColor()
        {
            HSLColor randomColor = new HSLColor();
            randomColor.h = Random.Range(0, 360);
            randomColor.s = 0.8f;
            randomColor.l = 0.1f;
            randomColor.a = 1;

            return randomColor;
        }

        // private static float Random.Range(float min, float max)
        // {
        //     return min + ((float) System.Random.NextDouble() * (max - min));
        // }

        public static bool IsBlack(this Color32 color)
        {
            if (color.r == 0 && color.g == 0 && color.b == 0)
            {
                return true;
            }

            return false;
        }

        public const float ColorOffset = 40;

        public static Color[] GetGradients(int darkness)
        {
            Color[] colorGradient = new Color[2];
            var newColor = RandomBrightColor();
            bool increase = newColor.h < 100;
            if (increase)
            {
                newColor.h += ColorOffset;
            }
            else
            {
                newColor.h -= ColorOffset;
            }

            if (darkness == 2)
            {
                newColor.l -= 0.35f;
                newColor.s -= 0.2f;
            }

            if (darkness == 3)
            {
                if (increase)
                {
                    newColor.h -= ColorOffset * 2;
                }
                else
                {
                    newColor.h += ColorOffset * 2;
                }

                newColor.l -= 0.5f;
                newColor.s -= 0.4f;
            }

            HSLColor complimentColor = newColor;
            if (newColor.h < 30 && darkness != 2)
            {
                complimentColor.h += ColorOffset * 0.5f;
            }
            else
            {
                complimentColor.h -= ColorOffset * 0.5f;
            }

            complimentColor.s += Random.Range(-0.15f, 0.15f);
            colorGradient[0] = complimentColor;
            colorGradient[1] = newColor;
            return colorGradient;
        }
    }


    public struct HSLColor
    {
        public float h;
        public float s;
        public float l;
        public float a;


        public HSLColor(float h, float s, float l, float a)
        {
            this.h = h;
            this.s = s;
            this.l = l;
            this.a = a;
        }

        public HSLColor(float h, float s, float l)
        {
            this.h = h;
            this.s = s;
            this.l = l;
            this.a = 1f;
        }

        public HSLColor(Color c)
        {
            HSLColor temp = FromRGBA(c);
            h = temp.h;
            s = temp.s;
            l = temp.l;
            a = temp.a;
        }

        public static HSLColor FromRGBA(Color c)
        {
            float h, s, l, a;
            a = c.a;

            float cmin = MathEx.Min(MathEx.Min(c.r, c.g), c.b);
            float cmax = MathEx.Max(MathEx.Max(c.r, c.g), c.b);

            l = (cmin + cmax) / 2f;

            if (cmin == cmax)
            {
                s = 0;
                h = 0;
            }
            else
            {
                float delta = cmax - cmin;

                s = (l <= .5f) ? (delta / (cmax + cmin)) : (delta / (2f - (cmax + cmin)));

                h = 0;

                if (c.r == cmax)
                {
                    h = (c.g - c.b) / delta;
                }
                else if (c.g == cmax)
                {
                    h = 2f + (c.b - c.r) / delta;
                }
                else if (c.b == cmax)
                {
                    h = 4f + (c.r - c.g) / delta;
                }

                h = Mathf.Repeat(h * 60f, 360f);
            }

            return new HSLColor(h, s, l, a);
        }


        public Color ToRGBA()
        {
            float r, g, b, a;
            a = this.a;
            float m1, m2;
            m2 = (l <= .5f) ? (l * (1f + s)) : (l + s - l * s);
            m1 = 2f * l - m2;
            if (s == 0f)
            {
                r = g = b = l;
            }
            else
            {
                r = Value(m1, m2, h + 120f);
                g = Value(m1, m2, h);
                b = Value(m1, m2, h - 120f);
            }

            return new Color(r, g, b, a);
        }

        static float Value(float n1, float n2, float hue)
        {
            hue = Mathf.Repeat(hue, 360f);

            if (hue < 60f)
            {
                return n1 + (n2 - n1) * hue / 60f;
            }

            if (hue < 180f)
            {
                return n2;
            }

            if (hue < 240f)
            {
                return n1 + (n2 - n1) * (240f - hue) / 60f;
            }

            return n1;
        }

        public static implicit operator HSLColor(Color src)
        {
            return FromRGBA(src);
        }

        public static implicit operator Color(HSLColor src)
        {
            return src.ToRGBA();
        }

        public override string ToString()
        {
            return string.Format("H: {0} S: {1} L: {2} A: {3}", h, s, l, a);
        }

        public static uint ToUint(HSLColor hsl)
        {
            int num1 = (int) Math.Round(hsl.h * 360.0);
            int num2 = (int) Math.Round(hsl.s * 100.0 + 100.0);
            int num3 = (int) Math.Round(hsl.l * 100.0 + 100.0);
            int num4 = num2 << 16;
            int num5 = num3 << 24;
            return (uint) (num1 | num4 | num5);
        }

        public static HSLColor FromUint(uint packed)
        {
            return new HSLColor((float) (ushort) packed / 360f,
                (float) ((int) (byte) (packed >> 16) - 100) / 100f,
                (float) ((int) (byte) (packed >> 24) - 100) / 100f);
        }
    }
}
