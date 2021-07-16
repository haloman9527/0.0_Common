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
using System.Linq;
using UnityEngine;

namespace CZToolKit.Core.ObjectPool
{
    [Serializable]
    public class UnityGOPool : UnityPoolBase<GameObject>
    {
        public bool group;
        [SerializeField]
        private Transform groupParent;

        public Transform GroupParent
        {
            get
            {
                if (groupParent == null)
                    groupParent = new GameObject(template.name + " - " + template.GetInstanceID().ToString()).transform;
                return groupParent;
            }
        }

        public UnityGOPool() { }

        public UnityGOPool(GameObject _template, bool _group) : base(_template) { group = _group; }

        public virtual void InitCount()
        {
            if (IdleList.Count + WorkList.Count > maxCount)
                return;
            for (int i = 0; i < maxCount; i++)
            {
                GameObject unit = CreateNewUnit();
                unit.SetActive(false);
                unit.transform.SetParent(groupParent);
                IdleList.Add(unit);
            }
        }

        protected override void OnBeforeSpawn(GameObject _unit)
        {
            base.OnBeforeSpawn(_unit);
            _unit.SetActive(true);
            _unit.transform.SetParent(groupParent, true);
        }

        protected override void OnAfterRecycle(GameObject _unit)
        {
            base.OnAfterRecycle(_unit);
            _unit.SetActive(false);
            if (group)
                _unit.transform.SetParent(GroupParent);
            if (IdleList.Count > maxCount)
            {
                GameObject.Destroy(IdleList.First());
                IdleList.Remove(IdleList.First());
            }
        }

        public override void Dispose()
        {
            foreach (var unit in IdleList)
            {
                GameObject.Destroy(unit);
            }
            IdleList.Clear();
        }
    }
}
