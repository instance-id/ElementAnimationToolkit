using System;

namespace instance.id.EATK
{
    public enum Easing
    {
        Step, Linear, InSine, OutSine, InOutSine, InQuad, OutQuad, OutExpo, InOutQuad, InCubic,
        OutCubic, InOutCubic, InPower, OutPower, InOutPower, InBounce, OutBounce, InOutBounce,
        InElastic, OutElastic, InOutElastic, InBack, OutBack, InOutBack, InCirc, OutCirc,
        InOutCirc, EaseInOutQuint
    }

    [Serializable] public abstract class GetEasing
    {
        public static Func<float, float> Get(Easing easingType)
        {
            switch (easingType)
            {
                case Easing.Step:           return Easy.Step;
                case Easing.Linear:         return Easy.Linear;
                case Easing.InSine:         return Easy.InSine;
                case Easing.OutSine:        return Easy.OutSine;
                case Easing.InOutSine:      return Easy.InOutSine;
                case Easing.InQuad:         return Easy.InQuad;
                case Easing.OutQuad:        return Easy.OutQuad;
                case Easing.OutExpo:        return Easy.OutExpo;
                case Easing.InOutQuad:      return Easy.InOutQuad;
                case Easing.InCubic:        return Easy.InCubic;
                case Easing.OutCubic:       return Easy.OutCubic;
                case Easing.InOutCubic:     return Easy.InOutCubic;
                case Easing.InBounce:       return Easy.InBounce;
                case Easing.OutBounce:      return Easy.OutBounce;
                case Easing.InOutBounce:    return Easy.InOutBounce;
                case Easing.InElastic:      return Easy.InElastic;
                case Easing.OutElastic:     return Easy.OutElastic;
                case Easing.InOutElastic:   return Easy.InOutElastic;
                case Easing.InBack:         return Easy.InBack;
                case Easing.OutBack:        return Easy.OutBack;
                case Easing.InOutBack:      return Easy.InOutBack;
                case Easing.InCirc:         return Easy.InCirc;
                case Easing.OutCirc:        return Easy.OutCirc;
                case Easing.InOutCirc:      return Easy.InOutCirc;
                case Easing.EaseInOutQuint: return Easy.EaseInOutQuint;
                // case Easing.InPower:       return e = floatVal => Easy.InPower(floatVal); break;
                // case Easing.OutPower:      return e = floatVal => Easy.OutPower(floatVal); break;
                // case Easing.InOutPower:    return e = floatVal => Easy.InOutPower(floatVal); break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}
