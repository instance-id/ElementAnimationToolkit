using System;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace instance.id.EATK.Extensions
{
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Get attributes from a class
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sortOutput"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ClassData<T> GetEditorAttributes<T>(this object obj, bool sortOutput = false) where T : Attribute, new()
        {
            var thisType = obj.GetType();
            var classData = new ClassData<T>(thisType);
            try
            {
                foreach (var field in thisType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (!(field.GetCustomAttributes(typeof(T), true).FirstOrDefault() is T att)) continue;
                    classData.fieldDatas.TryAddValue(field.Name, new FieldData<T>(field));
                    classData.fieldList.TryAddValue(classData.fieldDatas[field.Name].attributeData);
                }
            }
            catch (Exception e) { Debug.LogException(e); }
            return classData; // @formatter:on
        }
        
        public static string GetRealTypeName(this Type t, bool parameters = false)
        {
            if (!t.IsGenericType) return t.Name;
            var sb = new StringBuilder();
            sb.Append(t.Name.Substring(0, t.Name.IndexOf('`')));
            if (!parameters) return sb.ToString();
            sb.Append('<');
            var appendComma = false;
            foreach (var arg in t.GetGenericArguments())
            {
                if (appendComma) sb.Append(',');
                sb.Append(GetRealTypeName(arg));
                appendComma = true;
            }

            sb.Append('>');
            return sb.ToString();
        }
    }
}
