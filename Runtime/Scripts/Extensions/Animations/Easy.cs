// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/ElementAnimationToolkit         --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

using System;
using UnityEngine;

namespace instance.id.EATK.Extensions
{
    /// <summary>
    ///   <para>A collection of easing curves to be used with ValueAnimations.</para>
    /// </summary>
    public static class Easy
    {
        private const float HalfPi = 1.570796f;

        public static float Step(float t) => (double) t < 0.5 ? 0.0f : 1f;

        public static float Linear(float t) => t;

        public static float InSine(float t) => Mathf.Sin((float) (1.57079637050629 * ((double) t - 1.0))) + 1f;

        public static float OutSine(float t) => Mathf.Sin(t * 1.570796f);

        public static float InOutSine(float t) => (float) (((double) Mathf.Sin((float) (3.14159274101257 * ((double) t - 0.5))) + 1.0) * 0.5);

        public static float InQuad(float t) => t * t;

        public static float OutQuad(float t) => t * (2f - t);

        public static float OutExpo(float t) {
            return t.Equals(1f) ? 1 : 1 - Mathf.Pow(2, -10 * t);

        }

        public static float InOutQuad(float t)
        {
            t *= 2f;
            return (double) t < 1.0 ? (float) ((double) t * (double) t * 0.5) : (float) (-0.5 * (((double) t - 1.0) * ((double) t - 3.0) - 1.0));
        }

        public static float InCubic(float t) => Easy.InPower(t, 3);

        public static float OutCubic(float t) => Easy.OutPower(t, 3);

        public static float InOutCubic(float t) => Easy.InOutPower(t, 3);

        public static float InPower(float t, int power) => Mathf.Pow(t, (float) power);

        public static float OutPower(float t, int power)
        {
            int num = power % 2 == 0 ? -1 : 1;
            return (float) num * (Mathf.Pow(t - 1f, (float) power) + (float) num);
        }

        public static float InOutPower(float t, int power)
        {
            t *= 2f;
            if ((double) t < 1.0)
                return Easy.InPower(t, power) * 0.5f;
            int num = power % 2 == 0 ? -1 : 1;
            return (float) ((double) num * 0.5 * ((double) Mathf.Pow(t - 2f, (float) power) + (double) (num * 2)));
        }

        public static float InBounce(float t) => 1f - Easy.OutBounce(1f - t);

        public static float OutBounce(float t)
        {
            if ((double) t < 0.363636374473572)
                return 121f / 16f * t * t;
            if ((double) t < 0.727272748947144)
                return (float) (121.0 / 16.0 * (double) (t -= 0.5454546f) * (double) t + 0.75);
            return (double) t < 0.909090936183929
                ? (float) (121.0 / 16.0 * (double) (t -= 0.8181818f) * (double) t + 15.0 / 16.0)
                : (float) (121.0 / 16.0 * (double) (t -= 0.9545454f) * (double) t + 63.0 / 64.0);
        }

        public static float InOutBounce(float t) =>
            (double) t < 0.5 ? Easy.InBounce(t * 2f) * 0.5f : (float) ((double) Easy.OutBounce((float) (((double) t - 0.5) * 2.0)) * 0.5 + 0.5);

        public static float InElastic(float t)
        {
            if ((double) t == 0.0)
                return 0.0f;
            if ((double) t == 1.0)
                return 1f;
            float num1 = 0.3f;
            float num2 = num1 / 4f;
            return (float) -((double) Mathf.Pow(2f, 10f * --t) * (double) Mathf.Sin((float) (((double) t - (double) num2) * 6.28318548202515) / num1));
        }

        public static float OutElastic(float t)
        {
            if ((double) t == 0.0)
                return 0.0f;
            if ((double) t == 1.0)
                return 1f;
            float num1 = 0.3f;
            float num2 = num1 / 4f;
            return (float) ((double) Mathf.Pow(2f, -10f * t) * (double) Mathf.Sin((float) (((double) t - (double) num2) * 6.28318548202515) / num1) + 1.0);
        }

        public static float InOutElastic(float t) =>
            (double) t < 0.5 ? Easy.InElastic(t * 2f) * 0.5f : (float) ((double) Easy.OutElastic((float) (((double) t - 0.5) * 2.0)) * 0.5 + 0.5);

        public static float InBack(float t)
        {
            float num = 1.70158f;
            return (float) ((double) t * (double) t * (((double) num + 1.0) * (double) t - (double) num));
        }

        public static float OutBack(float t) => 1f - Easy.InBack(1f - t);

        public static float InOutBack(float t) =>
            (double) t < 0.5 ? Easy.InBack(t * 2f) * 0.5f : (float) ((double) Easy.OutBack((float) (((double) t - 0.5) * 2.0)) * 0.5 + 0.5);

        public static float InBack(float t, float s) => (float) ((double) t * (double) t * (((double) s + 1.0) * (double) t - (double) s));

        public static float OutBack(float t, float s) => 1f - Easy.InBack(1f - t, s);

        public static float InOutBack(float t, float s) =>
            (double) t < 0.5 ? Easy.InBack(t * 2f, s) * 0.5f : (float) ((double) Easy.OutBack((float) (((double) t - 0.5) * 2.0), s) * 0.5 + 0.5);

        public static float InCirc(float t) => (float) -((double) Mathf.Sqrt((float) (1.0 - (double) t * (double) t)) - 1.0);

        public static float OutCirc(float t)
        {
            --t;
            return Mathf.Sqrt((float) (1.0 - (double) t * (double) t));
        }

        public static float InOutCirc(float t)
        {
            t *= 2f;
            if ((double) t < 1.0)
                return (float) (-0.5 * ((double) Mathf.Sqrt((float) (1.0 - (double) t * (double) t)) - 1.0));
            t -= 2f;
            return (float) (0.5 * ((double) Mathf.Sqrt((float) (1.0 - (double) t * (double) t)) + 1.0));
        }


        public static float EaseInOutQuint(float t)
        {
            return (float) (t < 0.5f ? 16f * t * t * t * t * t : 1 - Math.Pow(-2 * t + 2, 5) / 2);
        }
    }
}
