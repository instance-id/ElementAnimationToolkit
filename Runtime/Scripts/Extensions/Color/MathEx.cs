using System;

namespace instance.id.EATK.Extensions
{
	public static class MathEx
	{
		public static int Clamp(int v, int min, int max)
		{
			return (v < min) ? min : ((v > max) ? max : v);
		}

		public static float Clamp01(float v)
		{
			return (v < 0f) ? 0f : ((v > 1f) ? 1f : v);
		}

		public static double Clamp01(double v)
		{
			return (v < 0.0) ? 0.0 : ((v > 1.0) ? 1.0 : v);
		}

		public static float Clamp(float v, float min, float max)
		{
			return (v < min) ? min : ((v > max) ? max : v);
		}

		public static double Clamp(double v, double min, double max)
		{
			return (v < min) ? min : ((v > max) ? max : v);
		}

		public static float Lerp(float from, float to, float t)
		{
			return from + (to - from) * MathEx.Clamp01(t);
		}

		public static double Lerp(double from, double to, double t)
		{
			return from + (to - from) * MathEx.Clamp01(t);
		}

		public static float Repeat(float t, float length)
		{
			return t - (float)Math.Floor((double)(t / length)) * length;
		}

		public static double Repeat(double t, double length)
		{
			return t - Math.Floor(t / length) * length;
		}

		public static float LerpAngle(float a, float b, float t)
		{
			float num = MathEx.Repeat(b - a, 360f);
			bool flag = num <= 180f;
			if (!flag)
			{
				num -= 360f;
			}
			return a + num * MathEx.Clamp01(t);
		}

		public static double LerpAngle(double a, double b, double t)
		{
			double num = MathEx.Repeat(b - a, 360.0);
			bool flag = num <= 180.0;
			if (!flag)
			{
				num -= 360.0;
			}
			return a + num * MathEx.Clamp01(t);
		}

		public static float Distance(float a, float b)
		{
			return Math.Abs(a - b);
		}

		public static double Distance(double a, double b)
		{
			return Math.Abs(a - b);
		}

		public static int IsRange(float value, float min, float max)
		{
			return (value < min) ? -1 : ((value > max) ? 1 : 0);
		}

		public static int IsRange(double value, double min, double max)
		{
			return (value < min) ? -1 : ((value > max) ? 1 : 0);
		}

		public static float Max(float a, float b)
		{
			return (a <= b) ? b : a;
		}

		public static double Max(double a, double b)
		{
			return (a <= b) ? b : a;
		}

		public static float Min(float a, float b)
		{
			return (a <= b) ? a : b;
		}

		public static double Min(double a, double b)
		{
			return (a <= b) ? a : b;
		}

		public static bool Approximately(float a, float b)
		{
			return Math.Abs(b - a) < MathEx.Max(1E-06f * MathEx.Max(Math.Abs(a), Math.Abs(b)), 1.1E-44f);
		}

		public static bool Approximately(double a, double b)
		{
			return Math.Abs(b - a) < MathEx.Max(1E-06 * MathEx.Max(Math.Abs(a), Math.Abs(b)), 1.121039E-44);
		}
	}
}
