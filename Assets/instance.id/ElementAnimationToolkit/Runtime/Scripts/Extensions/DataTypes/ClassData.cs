using System;
using System.Collections.Generic;
using UnityEngine;

namespace instance.id.EATK.Extensions
{
    [Serializable]
    public class ClassData<T> where T : Attribute , new()
    {
        [SerializeField] public string typeName;
        [SerializeField] public List<T> fieldList = new List<T>();
        public Dictionary<string, FieldData<T>> fieldDatas = new Dictionary<string, FieldData<T>>();

        public ClassData(Type type) => typeName = type.Name;
    }
}
