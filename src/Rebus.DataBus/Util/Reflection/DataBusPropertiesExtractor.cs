using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rebus.DataBus.Util.Reflection
{
    public static class DataBusPropertiesExtractor
    {
        //private static readonly ConcurrentDictionary<Type, List<DataBusPropertyInfo>> DataBusPropertyCache =
        //    new ConcurrentDictionary<Type, List<DataBusPropertyInfo>>();

        private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> MessagePropertyCache =
            new ConcurrentDictionary<Type, List<PropertyInfo>>();

        //public static List<DataBusPropertyInfo> GetDataBusProperties(Type messageType)
        //{
        //    return GetDataBusProperties(messageType, null);
        //}

        //private static List<DataBusPropertyInfo> GetDataBusProperties(Type type, string propertyPath)
        //{
        //    if (type == null) throw new ArgumentNullException("type");

        //    List<DataBusPropertyInfo> list;

        //    if (!DataBusPropertyCache.TryGetValue(type, out list))
        //    {
        //        list = new List<DataBusPropertyInfo>();

        //        foreach (PropertyInfo propertyInfo in GetRelevantPropertyInfos(type))
        //        {
        //            if (propertyInfo.IsDataBusProperty())
        //            {
        //                list.Add(propertyInfo.PromoteToDataBusPropertyInfo(propertyPath));
        //            }
        //            else
        //            {
        //                if (propertyInfo.PropertyType.IsGenericType &&
        //                    propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof (IEnumerable<>))
        //                {
        //                    Type enumerableType = propertyInfo.PropertyType.GetGenericArguments()[0];

        //                    if (enumerableType != null)
        //                    {
        //                        list.AddRange(GetDataBusProperties(enumerableType, GetPropertyPath(propertyPath, propertyInfo)));
        //                    }
        //                }
                        
        //                list.AddRange(GetDataBusProperties(propertyInfo.PropertyType, GetPropertyPath(propertyPath, propertyInfo)));
        //            }
        //        }

        //        DataBusPropertyCache[type] = list;
        //    }

        //    return list;
        //}

        //private static string GetPropertyPath(string propertyPath, PropertyInfo propertyInfo)
        //{
        //    var basePath = propertyPath ?? String.Empty;

        //    return basePath == String.Empty
        //        ? propertyInfo.Name
        //        : String.Format("{0}.{1}", basePath, propertyInfo.Name);
        //}

        //public static List<PropertyInfo> GetRelevantPropertyInfos(Type type)
        //{
        //    return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
        //                .Where(t => !PrimitiveTypes.Test(t.PropertyType))
        //                .ToList();
        //}


        //public static IEnumerable<DataBusPropertyInfo> GetNonNullDataBusProperties2(object message)
        //{
        //    return GetNonNullDataBusProperties(message.GetType(), message);
        //}

        //private static object GetParentObjectForDataBusProperty(object rootInstance, string propertyPath)
        //{
        //    object parentInstance = rootInstance;
        //    string[] propertyNames = propertyPath.Split('.');

        //    if (propertyNames.Length == 1) return rootInstance;

        //    foreach (string propertyName in propertyNames)
        //    {                                 
        //        Type parentType = parentInstance.GetType();
        //        PropertyInfo propertyOnParent = parentType.GetProperty(propertyName);

        //        if (propertyOnParent != null)
        //        {
        //            parentInstance = propertyOnParent.GetValue(parentInstance, null);


        //        }
        //        else
        //        {
        //           throw new InvalidOperationException(String.Format("Property path '{0}' for root type '{1}' is invalid", propertyPath, rootInstance.GetType().FullName));
        //        }
        //    }

        //    return parentInstance;
        //}

        //private static IEnumerable<DataBusPropertyInfo> GetNonNullDataBusProperties2(Type type, object instance)
        //{
        //    foreach (var dataBusProp in GetDataBusProperties(type))
        //    {
        //        var parentObject = GetParentObjectForDataBusProperty(instance, dataBusProp.PopertyPath);

        //        if (parentObject == null) continue;

        //        IEnumerable parentEnumerableObject = parentObject as IEnumerable;

        //        if (parentEnumerableObject != null)
        //        {
        //            foreach (object parentItemObject in parentEnumerableObject)
        //            {
        //            }
        //        }


        //    }
            
            
            
        //    foreach (PropertyInfo propertyInfo in GetRelevantPropertyInfosCached(type))
        //    {
        //        Func<object> getterFunc = propertyInfo.GenerateGetterFunc(instance);

        //        object propertyValue = getterFunc();

        //        if (propertyValue == null) continue;

        //        if (propertyInfo.IsDataBusProperty())
        //        {
        //            yield return propertyInfo.PromoteToDataBusPropertyInfo(getterFunc);
        //        }
        //        else
        //        {
        //            IEnumerable enumerablePropertyValue = propertyValue as IEnumerable;

        //            if (enumerablePropertyValue != null)
        //            {
        //                foreach (object lo in enumerablePropertyValue)
        //                {
        //                    foreach (
        //                        DataBusPropertyInfo dataBusProperty in GetNonNullDataBusProperties(lo.GetType(), lo))
        //                    {
        //                        yield return dataBusProperty;
        //                    }
        //                }
        //            }

        //            foreach (DataBusPropertyInfo dataBusProperty in
        //                GetNonNullDataBusProperties(propertyInfo.PropertyType, propertyValue))
        //            {
        //                yield return dataBusProperty;
        //            }
        //        }
        //    }
        //}



        public static IEnumerable<DataBusPropertyInfo> GetNonNullDataBusProperties(object message)
        {
            return GetNonNullDataBusProperties(message.GetType(), message);
        }

        private static IEnumerable<DataBusPropertyInfo> GetNonNullDataBusProperties(Type type, object instance)
        {
            foreach (PropertyInfo propertyInfo in GetRelevantPropertyInfosCached(type))
            {
                Func<object> getterFunc = propertyInfo.GenerateGetterFunc(instance);

                object propertyValue = getterFunc();

                if (propertyValue == null) continue;

                if (propertyInfo.IsDataBusProperty())
                {
                    yield return propertyInfo.PromoteToDataBusPropertyInfo(getterFunc);
                }
                else
                {
                    IEnumerable enumerablePropertyValue = propertyValue as IEnumerable;

                    if (enumerablePropertyValue != null)
                    {
                        foreach (object enumerableItemValue in enumerablePropertyValue)
                        {
                            foreach (
                                DataBusPropertyInfo dataBusProperty in GetNonNullDataBusProperties(enumerableItemValue.GetType(), enumerableItemValue))
                            {
                                yield return dataBusProperty;
                            }
                        }
                    }

                    foreach (DataBusPropertyInfo dataBusProperty in
                        GetNonNullDataBusProperties(propertyInfo.PropertyType, propertyValue))
                    {
                        yield return dataBusProperty;
                    }
                }
            }
        }

        public static List<PropertyInfo> GetRelevantPropertyInfosCached(Type type)
        {
            List<PropertyInfo> list;

            if (!MessagePropertyCache.TryGetValue(type, out list))
            {
                List<PropertyInfo> props =
                    type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(t => !PrimitiveTypes.Test(t.PropertyType))
                        .ToList();

                MessagePropertyCache[type] = (list = props);
            }

            return list;
        }
    }
}