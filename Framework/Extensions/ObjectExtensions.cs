using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sepidar.Framework.Extensions
{
    public static class ObjectExtensions
    {
        private static Random random = new Random();

        public static bool IsNumeric(this object obj)
        {
            return decimal.TryParse(obj.ToString(), out decimal temp);
        }

        public static object Clone(this object @object)
        {
            var serialized = @object.JsonSerialize();
            var copy = serialized.JsonDeserialize();
            return copy;
        }

        public static T Clone<T>(this T t)
        {
            var serialized = t.JsonSerialize();
            var copy = serialized.JsonDeserialize<T>();
            return copy;
        }

        public static T CastTo<T>(this object @object)
        {
            var serialized = @object.JsonSerialize();
            var copy = serialized.JsonDeserialize<T>();
            return copy;
        }

        public static T GetNewValues<T>(this T target, T source, params string[] exludedProperties)
        {
            var properties = target.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (exludedProperties.Contains(property.Name))
                {
                    continue;
                }
                var targetValue = property.GetValue(target);
                var soureValue = property.GetValue(source);
                property.SetValue(target, soureValue);
            }
            return target;
        }

        public static bool IsNull(this object obj)
        {
            return obj == null;
        }

        public static bool IsNotNull(this object obj)
        {
            return !obj.IsNull();
        }

        public static bool Has(this object obj, string propertyName)
        {
            var dynamicObject = obj as DynamicObject;
            if (dynamicObject == null) return false;
            return dynamicObject.GetDynamicMemberNames().Contains(propertyName);
        }

        public static bool ToBoolean(this object @object)
        {
            if (@object.IsNull())
            {
                return false;
            }
            Boolean.TryParse(@object.ToString(), out bool result);
            return result;
        }

        public static ExpandoObject ToExpando(this object @object)
        {
            var properties = @object.GetType().GetProperties();
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var property in properties)
            {
                expando.Add(property.Name, property.GetValue(@object));
            }
            return (ExpandoObject)expando;
        }

        public static string JsonSerialize(this object @object)
        {
            return JsonConvert.SerializeObject(@object);
        }

        public static object Random(object[] objects)
        {
            var index = random.Next(objects.Length);
            var value = objects[index];
            return value;
        }
    }
}
