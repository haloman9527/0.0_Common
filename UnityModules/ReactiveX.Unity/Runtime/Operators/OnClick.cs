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
 *  Blog: https://www.mindgear.net/
 *
 */
#endregion
using System;
using UnityEngine.UI;

namespace CZToolKit.RX
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
            Next(button);
        }
    }
    public static partial class ReactiveExtension
    {
        public static IObservable<Button> OnClick(this IObservable<Button> src)
        {
            return new OnClick(src);
        }
    }
}
