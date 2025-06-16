#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */
#endregion
using System;

namespace Atom
{
    public enum EasingType
    {
        Linear,
        Clerp,
        Spring,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInQuart,
        EaseOutQuart,
        EaseInOutQuart,
        EaseInQuint,
        EaseOutQuint,
        EaseInOutQuint,
        EaseInSine,
        EaseOutSine,
        EaseInOutSine,
        EaseInExpo,
        EaseOutExpo,
        EaseInOutExpo,
        EaseInCirc,
        EaseOutCirc,
        EaseInOutCirc,
        EaseInBounce,
        EaseOutBounce,
        EaseInOutBounce,
        EaseInBack,
        EaseOutBack,
        EaseInOutBack,
        EaseInElastic,
        EaseOutElastic,
        EaseInOutElastic
    }

    public static class Easing
    {
        public static Func<float, float, float, float>[] Tweens;

        static Easing()
        {
            Tweens = new System.Func<float, float, float, float>[]{
                Linear,
                Clerp,
                Spring,
                EaseInQuad,
                EaseOutQuad,
                EaseInOutQuad,
                EaseInCubic,
                EaseOutCubic,
                EaseInOutCubic,
                EaseInQuart,
                EaseOutQuart,
                EaseInOutQuart,
                EaseInQuint,
                EaseOutQuint,
                EaseInOutQuint,
                EaseInSine,
                EaseOutSine,
                EaseInOutSine,
                EaseInExpo,
                EaseOutExpo,
                EaseInOutExpo,
                EaseInCirc,
                EaseOutCirc,
                EaseInOutCirc,
                EaseInBounce,
                EaseOutBounce,
                EaseInOutBounce,
                EaseInBack,
                EaseOutBack,
                EaseInOutBack,
                EaseInElastic,
                EaseOutElastic,
                EaseInOutElastic
            };
        }

        public static float Tween(float start, float end, float t, EasingType easingType)
        {
            return Tweens[(int)easingType](start, end, t);
        }

        public static float Linear(float start, float end, float t)
        {
            end -= start;
            return end * t + start;
        }

        public static float Clerp(float start, float end, float t)
        {
            float min = 0.0f;
            float max = 360.0f;
            float half = Math.Abs((max - min) * 0.5f);
            float retval = 0.0f;
            float diff = 0.0f;
            if ((end - start) < -half)
            {
                diff = ((max - start) + end) * t;
                retval = start + diff;
            }
            else if ((end - start) > half)
            {
                diff = -((max - end) + start) * t;
                retval = start + diff;
            }
            else retval = start + (end - start) * t;
            return retval;
        }

        public static float Spring(float start, float end, float t)
        {
            t = Math.Clamp(t, 0, 1);
            t = (float)(Math.Sin(t * Math.PI * (0.2f + 2.5f * t * t * t)) * Math.Pow(1f - t, 2.2f) + t) * (1f + (1.2f * (1f - t)));
            return start + (end - start) * t;
        }

        public static float EaseInQuad(float start, float end, float t)
        {
            end -= start;
            return end * t * t + start;
        }

        public static float EaseOutQuad(float start, float end, float t)
        {
            end -= start;
            return -end * t * (t - 2) + start;
        }

        public static float EaseInOutQuad(float start, float end, float t)
        {
            t /= .5f;
            end -= start;
            if (t < 1) return end * 0.5f * t * t + start;
            t--;
            return -end * 0.5f * (t * (t - 2) - 1) + start;
        }

        public static float EaseInCubic(float start, float end, float t)
        {
            end -= start;
            return end * t * t * t + start;
        }

        public static float EaseOutCubic(float start, float end, float t)
        {
            t--;
            end -= start;
            return end * (t * t * t + 1) + start;
        }

        public static float EaseInOutCubic(float start, float end, float t)
        {
            t /= .5f;
            end -= start;
            if (t < 1) return end * 0.5f * t * t * t + start;
            t -= 2;
            return end * 0.5f * (t * t * t + 2) + start;
        }

        public static float EaseInQuart(float start, float end, float t)
        {
            end -= start;
            return end * t * t * t * t + start;
        }

        public static float EaseOutQuart(float start, float end, float t)
        {
            t--;
            end -= start;
            return -end * (t * t * t * t - 1) + start;
        }

        public static float EaseInOutQuart(float start, float end, float t)
        {
            t /= .5f;
            end -= start;
            if (t < 1) return end * 0.5f * t * t * t * t + start;
            t -= 2;
            return -end * 0.5f * (t * t * t * t - 2) + start;
        }

        public static float EaseInQuint(float start, float end, float t)
        {
            end -= start;
            return end * t * t * t * t * t + start;
        }

        public static float EaseOutQuint(float start, float end, float t)
        {
            t--;
            end -= start;
            return end * (t * t * t * t * t + 1) + start;
        }

        public static float EaseInOutQuint(float start, float end, float t)
        {
            t /= .5f;
            end -= start;
            if (t < 1) return end * 0.5f * t * t * t * t * t + start;
            t -= 2;
            return end * 0.5f * (t * t * t * t * t + 2) + start;
        }

        public static float EaseInSine(float start, float end, float t)
        {
            end -= start;
            return (float)(-end * Math.Cos(t * (Math.PI * 0.5f)) + end + start);
        }

        public static float EaseOutSine(float start, float end, float t)
        {
            end -= start;
            return (float)(end * Math.Sin(t * (Math.PI * 0.5f)) + start);
        }

        public static float EaseInOutSine(float start, float end, float t)
        {
            end -= start;
            return (float)(-end * 0.5f * (Math.Cos(Math.PI * t) - 1) + start);
        }

        public static float EaseInExpo(float start, float end, float t)
        {
            end -= start;
            return (float)(end * Math.Pow(2, 10 * (t - 1)) + start);
        }

        public static float EaseOutExpo(float start, float end, float t)
        {
            end -= start;
            return (float)(end * (-Math.Pow(2, -10 * t) + 1) + start);
        }

        public static float EaseInOutExpo(float start, float end, float t)
        {
            t /= .5f;
            end -= start;
            if (t < 1) return (float)(end * 0.5f * Math.Pow(2, 10 * (t - 1)) + start);
            t--;
            return (float)(end * 0.5f * (-Math.Pow(2, -10 * t) + 2) + start);
        }

        public static float EaseInCirc(float start, float end, float t)
        {
            end -= start;
            return (float)(-end * (Math.Sqrt(1 - t * t) - 1) + start);
        }

        public static float EaseOutCirc(float start, float end, float t)
        {
            t--;
            end -= start;
            return (float)(end * Math.Sqrt(1 - t * t) + start);
        }

        public static float EaseInOutCirc(float start, float end, float t)
        {
            t /= .5f;
            end -= start;
            if (t < 1) return (float)(-end * 0.5f * (Math.Sqrt(1 - t * t) - 1) + start);
            t -= 2;
            return (float)(end * 0.5f * (Math.Sqrt(1 - t * t) + 1) + start);
        }

        /* GFX47 MOD START */
        public static float EaseInBounce(float start, float end, float t)
        {
            end -= start;
            float d = 1f;
            return end - EaseOutBounce(0, end, d - t) + start;
        }
        /* GFX47 MOD END */

        /* GFX47 MOD START */
        //public static float bounce(float start, float end, float t){
        public static float EaseOutBounce(float start, float end, float t)
        {
            t /= 1f;
            end -= start;
            if (t < (1 / 2.75f))
            {
                return end * (7.5625f * t * t) + start;
            }
            else if (t < (2 / 2.75f))
            {
                t -= (1.5f / 2.75f);
                return end * (7.5625f * (t) * t + .75f) + start;
            }
            else if (t < (2.5 / 2.75))
            {
                t -= (2.25f / 2.75f);
                return end * (7.5625f * (t) * t + .9375f) + start;
            }
            else
            {
                t -= (2.625f / 2.75f);
                return end * (7.5625f * (t) * t + .984375f) + start;
            }
        }
        /* GFX47 MOD END */

        /* GFX47 MOD START */
        public static float EaseInOutBounce(float start, float end, float t)
        {
            end -= start;
            float d = 1f;
            if (t < d * 0.5f) return EaseInBounce(0, end, t * 2) * 0.5f + start;
            else return EaseOutBounce(0, end, t * 2 - d) * 0.5f + end * 0.5f + start;
        }
        /* GFX47 MOD END */

        public static float EaseInBack(float start, float end, float t)
        {
            end -= start;
            t /= 1;
            float s = 1.70158f;
            return end * (t) * t * ((s + 1) * t - s) + start;
        }

        public static float EaseOutBack(float start, float end, float t)
        {
            float s = 1.70158f;
            end -= start;
            t = (t) - 1;
            return end * ((t) * t * ((s + 1) * t + s) + 1) + start;
        }

        public static float EaseInOutBack(float start, float end, float t)
        {
            float s = 1.70158f;
            end -= start;
            t /= .5f;
            if ((t) < 1)
            {
                s *= (1.525f);
                return end * 0.5f * (t * t * (((s) + 1) * t - s)) + start;
            }
            t -= 2;
            s *= (1.525f);
            return end * 0.5f * ((t) * t * (((s) + 1) * t + s) + 2) + start;
        }

        /* GFX47 MOD START */
        public static float EaseInElastic(float start, float end, float t)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (t == 0) return start;

            if ((t /= d) == 1) return start + end;

            if (a == 0f || a < Math.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = (float)(p / (2 * Math.PI) * Math.Asin(end / a));
            }

            return (float)(-(a * Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p)) + start);
        }
        /* GFX47 MOD END */

        /* GFX47 MOD START */
        public static float EaseOutElastic(float start, float end, float t)
        {
            /* GFX47 MOD END */
            //Thank you to rafael.marteleto for fixing this as a port over from Pedro's UnityTween
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (t == 0) return start;

            if ((t /= d) == 1) return start + end;

            if (a == 0f || a < Math.Abs(end))
            {
                a = end;
                s = p * 0.25f;
            }
            else
            {
                s = (float)(p / (2 * Math.PI) * Math.Asin(end / a));
            }

            return (float)(a * Math.Pow(2, -10 * t) * Math.Sin((t * d - s) * (2 * Math.PI) / p) + end + start);
        }

        /* GFX47 MOD START */
        public static float EaseInOutElastic(float start, float end, float t)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (t == 0) return start;

            if ((t /= d * 0.5f) == 2) return start + end;

            if (a == 0f || a < Math.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = (float)(p / (2 * Math.PI) * Math.Asin(end / a));
            }

            if (t < 1) return (float)(-0.5f * (a * Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p)) + start);
            return (float)(a * Math.Pow(2, -10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p) * 0.5f + end + start);
        }
        /* GFX47 MOD END */

        public static float Punch(float amplitude, float t)
        {
            float s = 9;
            if (t == 0)
            {
                return 0;
            }
            else if (t == 1)
            {
                return 0;
            }
            float period = 1 * 0.3f;
            s = (float)(period / (2 * Math.PI) * Math.Asin(0));
            return (float)(amplitude * Math.Pow(2, -10 * t) * Math.Sin((t * 1 - s) * (2 * Math.PI) / period));
        }
    }
}
