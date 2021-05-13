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

        public override GameObject Spawn()
        {
            GameObject go = base.Spawn();
            go.SetActive(true);
            return go;
        }

        public virtual GameObject Spawn(Transform _parent)
        {
            GameObject go = Spawn();
            go.transform.SetParent(_parent, true);
            return go;
        }

        public override void Recycle(GameObject _unit)
        {
            base.Recycle(_unit);
            _unit.SetActive(false);

            if (group)
                _unit.transform.SetParent(GroupParent);
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
    }
}
