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
using UnityEngine;

namespace Jiange.SharedVariable
{
    [Serializable]
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.HideReferenceObjectPicker]
#endif
    public abstract class SharedVariable
    {
        [SerializeField, HideInInspector] protected long id = Snowflake.BaseUTC2020.NextId();

        public long Id
        {
            get => this.id;
            protected set => this.id = value;
        }

        public IVariableOwner Owner { get; protected set; }

        public void SetVariableOwner(IVariableOwner owner)
        {
            this.Owner = owner;
        }
    }

    [Serializable]
    public class SharedVariable<T> : SharedVariable
    {
        [SerializeField] protected T value;

        public T Value
        {
            get { return (this.Owner != null && this.Owner.VariableSource != null && this.Owner.VariableSource.TryGetValue(this.Id, out T v)) ? v : value; }
            set
            {
                if (this.Owner != null && this.Owner.VariableSource != null)
                {
                    this.Owner.VariableSource.SetValue(this.id, value);
                }
                else
                {
                    this.value = value;
                }
            }
        }

        public SharedVariable()
        {
            
        }

        public SharedVariable(T v)
        {
            this.value = v;
        }
    }
}