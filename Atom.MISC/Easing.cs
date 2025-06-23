using System;

namespace Atom
{
    public static class Easing
    {
        /// <summary>
        /// 缓动类型枚举，定义各种常用曲线。
        /// </summary>
        public enum EasingType
        {
            /// <summary>线性，匀速变化</summary>
            Linear,
            /// <summary>角度插值（360度环绕）</summary>
            Clerp,
            /// <summary>弹簧效果，带回弹</summary>
            Spring,
            /// <summary>二次缓入，加速</summary>
            EaseInQuad,
            /// <summary>二次缓出，减速</summary>
            EaseOutQuad,
            /// <summary>二次缓入缓出，先加速后减速</summary>
            EaseInOutQuad,
            /// <summary>三次缓入，加速更快</summary>
            EaseInCubic,
            /// <summary>三次缓出，减速更快</summary>
            EaseOutCubic,
            /// <summary>三次缓入缓出，先加速后减速</summary>
            EaseInOutCubic,
            /// <summary>四次缓入，极快加速</summary>
            EaseInQuart,
            /// <summary>四次缓出，极快减速</summary>
            EaseOutQuart,
            /// <summary>四次缓入缓出，先加速后减速</summary>
            EaseInOutQuart,
            /// <summary>五次缓入，超快加速</summary>
            EaseInQuint,
            /// <summary>五次缓出，超快减速</summary>
            EaseOutQuint,
            /// <summary>五次缓入缓出，先加速后减速</summary>
            EaseInOutQuint,
            /// <summary>正弦缓入，柔和加速</summary>
            EaseInSine,
            /// <summary>正弦缓出，柔和减速</summary>
            EaseOutSine,
            /// <summary>正弦缓入缓出，柔和加速减速</summary>
            EaseInOutSine,
            /// <summary>指数缓入，初始极慢后极快</summary>
            EaseInExpo,
            /// <summary>指数缓出，初始极快后极慢</summary>
            EaseOutExpo,
            /// <summary>指数缓入缓出，极端加速减速</summary>
            EaseInOutExpo,
            /// <summary>圆形缓入，圆弧加速</summary>
            EaseInCirc,
            /// <summary>圆形缓出，圆弧减速</summary>
            EaseOutCirc,
            /// <summary>圆形缓入缓出，圆弧加速减速</summary>
            EaseInOutCirc,
            /// <summary>弹跳缓入，先弹后到达</summary>
            EaseInBounce,
            /// <summary>弹跳缓出，弹跳到终点</summary>
            EaseOutBounce,
            /// <summary>弹跳缓入缓出，弹跳起止</summary>
            EaseInOutBounce,
            /// <summary>回退缓入，先回拉再前进</summary>
            EaseInBack,
            /// <summary>回退缓出，超出后回拉</summary>
            EaseOutBack,
            /// <summary>回退缓入缓出，前后回拉</summary>
            EaseInOutBack,
            /// <summary>弹性缓入，带弹性拉伸</summary>
            EaseInElastic,
            /// <summary>弹性缓出，带弹性回弹</summary>
            EaseOutElastic,
            /// <summary>弹性缓入缓出，弹性起止</summary>
            EaseInOutElastic
        }

        // 预先存储常用数学常量
        private static readonly float PI = (float)Math.PI;
        private static readonly float PI_HALF = (float)(Math.PI * 0.5f);
        private static readonly float PI2 = (float)(Math.PI * 2.0f);
        
        public static float Tween(float start, float end, float t, EasingType easingType)
        {
            t = Math.Clamp(t, 0f, 1f);
            switch (easingType)
            {
                case EasingType.Linear: return Linear(start, end, t);
                case EasingType.Clerp: return Clerp(start, end, t);
                case EasingType.Spring: return Spring(start, end, t);
                case EasingType.EaseInQuad: return EaseInQuad(start, end, t);
                case EasingType.EaseOutQuad: return EaseOutQuad(start, end, t);
                case EasingType.EaseInOutQuad: return EaseInOutQuad(start, end, t);
                case EasingType.EaseInCubic: return EaseInCubic(start, end, t);
                case EasingType.EaseOutCubic: return EaseOutCubic(start, end, t);
                case EasingType.EaseInOutCubic: return EaseInOutCubic(start, end, t);
                case EasingType.EaseInQuart: return EaseInQuart(start, end, t);
                case EasingType.EaseOutQuart: return EaseOutQuart(start, end, t);
                case EasingType.EaseInOutQuart: return EaseInOutQuart(start, end, t);
                case EasingType.EaseInQuint: return EaseInQuint(start, end, t);
                case EasingType.EaseOutQuint: return EaseOutQuint(start, end, t);
                case EasingType.EaseInOutQuint: return EaseInOutQuint(start, end, t);
                case EasingType.EaseInSine: return EaseInSine(start, end, t);
                case EasingType.EaseOutSine: return EaseOutSine(start, end, t);
                case EasingType.EaseInOutSine: return EaseInOutSine(start, end, t);
                case EasingType.EaseInExpo: return EaseInExpo(start, end, t);
                case EasingType.EaseOutExpo: return EaseOutExpo(start, end, t);
                case EasingType.EaseInOutExpo: return EaseInOutExpo(start, end, t);
                case EasingType.EaseInCirc: return EaseInCirc(start, end, t);
                case EasingType.EaseOutCirc: return EaseOutCirc(start, end, t);
                case EasingType.EaseInOutCirc: return EaseInOutCirc(start, end, t);
                case EasingType.EaseInBounce: return EaseInBounce(start, end, t);
                case EasingType.EaseOutBounce: return EaseOutBounce(start, end, t);
                case EasingType.EaseInOutBounce: return EaseInOutBounce(start, end, t);
                case EasingType.EaseInBack: return EaseInBack(start, end, t);
                case EasingType.EaseOutBack: return EaseOutBack(start, end, t);
                case EasingType.EaseInOutBack: return EaseInOutBack(start, end, t);
                case EasingType.EaseInElastic: return EaseInElastic(start, end, t);
                case EasingType.EaseOutElastic: return EaseOutElastic(start, end, t);
                case EasingType.EaseInOutElastic: return EaseInOutElastic(start, end, t);
                default: return Linear(start, end, t);
            }
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
            t = (float)(Math.Sin(t * PI * (0.2f + 2.5f * t * t * t)) * Math.Pow(1f - t, 2.2f) + t) * (1f + (1.2f * (1f - t)));
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
            return (float)(-end * Math.Cos(t * PI_HALF) + end + start);
        }

        public static float EaseOutSine(float start, float end, float t)
        {
            end -= start;
            return (float)(end * Math.Sin(t * PI_HALF) + start);
        }

        public static float EaseInOutSine(float start, float end, float t)
        {
            end -= start;
            return (float)(-end * 0.5f * (Math.Cos(PI * t) - 1) + start);
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
                s = (float)(p / PI2 * Math.Asin(end / a));
            }

            return (float)(-(a * Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t * d - s) * PI2 / p)) + start);
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
                s = (float)(p / PI2 * Math.Asin(end / a));
            }

            return (float)(a * Math.Pow(2, -10 * t) * Math.Sin((t * d - s) * PI2 / p) + end + start);
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
                s = (float)(p / PI2 * Math.Asin(end / a));
            }

            if (t < 1) return (float)(-0.5f * (a * Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t * d - s) * PI2 / p)) + start);
            return (float)(a * Math.Pow(2, -10 * (t -= 1)) * Math.Sin((t * d - s) * PI2 / p) * 0.5f + end + start);
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
