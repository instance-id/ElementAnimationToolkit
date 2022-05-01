using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace instance.id.EATK.Extensions
{
    [Serializable]
    public class FieldData<T> : FieldData where T : Attribute, new()
    {
        [SerializeField] public T attributeData;

        public FieldData(FieldInfo fieldInfo) : base(fieldInfo)
        {
            CheckForAttributes(fieldInfo);
        }

        public FieldData() : base()
        {
            CheckForAttributes(fieldInfo);
        }

        private void CheckForAttributes(FieldInfo fieldInfo)
        {
            var catAttrib = (T)Attribute.GetCustomAttribute(fieldInfo ?? throw new ArgumentNullException(nameof(fieldInfo)), typeof(T));
            attributeData = catAttrib ?? new T();
        }
    }

    [Serializable]
    public class FieldData
    {
        public Type fieldType;
        public FieldInfo fieldInfo;
        [SerializeField] public string name;
        [SerializeField] public string fieldTypeString;
        [SerializeField] public List<string> fieldTypeParametersString;
        [SerializeField] public List<Type> fieldTypeParameters;

        public FieldData() { }

        public FieldData(FieldInfo fieldInfo)
        {
            this.fieldInfo = fieldInfo;
            name = fieldInfo.Name;
            fieldType = fieldInfo.FieldType;
            fieldTypeString = fieldInfo.FieldType.GetRealTypeName();
            fieldTypeParameters = fieldInfo.FieldType
                .GetGenericArguments()
                .Select(x => x)
                .ToList();

            fieldTypeParametersString = fieldInfo.FieldType
                .GetGenericArguments()
                .Select(x => x.Name.ToString())
                .ToList();
        }
    }
}
