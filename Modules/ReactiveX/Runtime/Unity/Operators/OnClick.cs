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
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
using System;
using UnityEngine.UI;

namespace CZToolKit.Core.ReactiveX
{
    public class OnClick : Operator<Button>
    {
        Button button;

        public OnClick(IObservable<Button> src) : base(src) { }

        public override void OnNext(Button button)
        {
            this.button = button;

            if (this.button == null)
                base.OnError(new ArgumentNullException("button", "Button 参数为空"));
            else
                this.button.onClick.AddListener(this.OnBtnClick);
        }


        public override void OnDispose()
        {
            button.onClick.RemoveListener(OnBtnClick);
        }

        private void OnBtnClick()
        {
            base.OnNext(button);
        }
    }
    public static partial class Extension
    {
        public static IObservable<Button> OnClick(this IObservable<Button> src)
        {
            return new OnClick(src);
        }
    }
}
