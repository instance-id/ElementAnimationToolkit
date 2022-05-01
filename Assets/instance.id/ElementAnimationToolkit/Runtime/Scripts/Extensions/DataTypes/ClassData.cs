using System;
using System.Collections.Generic;
using UnityEngine;

namespace instance.id.EATK.Extensions
{
    [Serializable]
    public class ClassData<T> : ClassData where T : Attribute, new()
    {
        [SerializeField] public List<T> fieldList = new List<T>();
        public Dictionary<string, FieldData<T>> fieldDatas = new Dictionary<string, FieldData<T>>();

        public ClassData(Type type) : base(type) { }
    }

    [Serializable]
    public class ClassData
    {
        [SerializeField] public string typeName;
        public Dictionary<string, FieldData> fieldDatas = new Dictionary<string, FieldData>();
        public ClassData(Type type) => typeName = type.Name;
    }
}
