using System;
using System.Linq;
using UnityEngine;

namespace CZToolKit.Core.ObjectPool
{
    [Serializable]
    public class UnityGOPool : UnityPoolBase<GameObject>
    {
        public bool group;
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
        public UnityGOPool(GameObject _template) : base(_template) { }

        public UnityGOPool(GameObject _template, bool _group) : base(_template) { group = _group; }

        public virtual GameObject Spawn(Transform _parent)
        {
            //重写是为了使用被Unity重写过的判空
            GameObject unit = null;
            while (IdleList.Count > 0 && unit == null)
            {
                unit = IdleList[0];
                IdleList.RemoveAt(0);
            }

            if (unit == null)
                unit = CreateNewUnit(_parent);

            WorkList.Add(unit);
            OnSpawn(unit);
            return unit;
        }

        public virtual void InitCount()
        {
            if (IdleList.Count + WorkList.Count > maxCount)
                return;
            for (int i = 0; i < maxCount; i++)
            {
                GameObject unit = CreateNewUnit(group ? GroupParent : null);
                unit.SetActive(false);
                IdleList.Add(unit);
            }
        }

        protected override void OnSpawn(GameObject unit)
        {
            unit.SetActive(true);
        }

        protected override void OnRecycle(GameObject unit)
        {
            unit.SetActive(false);

            if (group)
                unit.transform.SetParent(GroupParent);
            if (IdleList.Count > maxCount)
            {
                GameObject.Destroy(IdleList.First());
                IdleList.Remove(IdleList.First());
            }
        }

        protected virtual GameObject CreateNewUnit(Transform parent)
        {
            GameObject go = GameObject.Instantiate(template, parent, false);
            return go;
        }

        public override void Dispose()
        {

        }
    }
}
