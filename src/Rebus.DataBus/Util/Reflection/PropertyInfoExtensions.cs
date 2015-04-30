using System;
using System.Linq.Expressions;
using System.Reflection;
using Rebus.DataBus.Messaging;

namespace Rebus.DataBus.Util.Reflection
{
    public static class PropertyInfoExtensions
    {
        /// <summary>
        /// Taken from SO: http://stackoverflow.com/a/28869743/234417
        /// </summary>
        public static Func<object, object> GenerateGetterFunc(this PropertyInfo pi)
        {
            var expParamPo = Expression.Parameter(typeof (object), "p");
            var expParamPc = Expression.Convert(expParamPo, pi.DeclaringType);

            var expMma = Expression.MakeMemberAccess(expParamPc, pi);

            var expMmac = Expression.Convert(expMma, typeof (object));

            var exp = Expression.Lambda<Func<object, object>>(expMmac, expParamPo);

            return exp.Compile();
        }
        
        public static Func<object> GenerateGetterFunc(this PropertyInfo pi, object instance)
        {
            var getter = pi.GenerateGetterFunc();

            return () => getter(instance);
        }

        /// <summary>
        /// Taken from SO: http://stackoverflow.com/a/28869743/234417
        /// </summary>
        public static Action<object, object> GenerateSetterAction(this PropertyInfo pi)
        {
            var expParamPo = Expression.Parameter(typeof (object), "p");
            var expParamPc = Expression.Convert(expParamPo, pi.DeclaringType);

            var expParamV = Expression.Parameter(typeof (object), "v");
            var expParamVc = Expression.Convert(expParamV, pi.PropertyType);

            var expMma = Expression.Call(expParamPc, pi.GetSetMethod(), expParamVc);

            var exp = Expression.Lambda<Action<object, object>>(expMma, expParamPo, expParamV);

            return exp.Compile();
        }

        public static bool IsDataBusProperty(this PropertyInfo propertyInfo)
        {
            if (typeof (IDataBusProperty).IsAssignableFrom(propertyInfo.PropertyType))
                return typeof (IDataBusProperty) != propertyInfo.PropertyType;

            return false;
        }

        public static bool IsDataBusCompressedProperty(this PropertyInfo propertyInfo)
        {
            if (typeof (IDataBusCompressedProperty).IsAssignableFrom(propertyInfo.PropertyType))
                return typeof (IDataBusCompressedProperty) != propertyInfo.PropertyType;

            return false;
        }

        //public static DataBusPropertyInfo PromoteToDataBusPropertyInfo(this PropertyInfo propertyInfo)
        //{
        //    if (!propertyInfo.IsDataBusProperty())
        //        throw new ArgumentException("Can't promote this property to a DataBusPropertyInfo object.");

        //    return new DataBusPropertyInfo(
        //        propertyInfo.Name,
        //        propertyInfo.IsDataBusCompressedProperty(),
        //        propertyInfo.GenerateGetterFunc(), null);
        //}

        //public static DataBusPropertyInfo PromoteToDataBusPropertyInfo(this PropertyInfo propertyInfo, string propertyPath)
        //{
        //    if (!propertyInfo.IsDataBusProperty())
        //        throw new ArgumentException("Can't promote this property to a DataBusPropertyInfo object.");

        //    return new DataBusPropertyInfo(
        //        propertyInfo.Name,
        //        propertyInfo.IsDataBusCompressedProperty(),
        //        propertyInfo.GenerateGetterFunc(), 
        //        propertyPath == null ? propertyInfo.Name : propertyPath + "." + propertyInfo.Name);
        //}

        //public static DataBusPropertyInfo PromoteToDataBusPropertyInfo(this PropertyInfo propertyInfo, object instance)
        //{
        //    if (!propertyInfo.IsDataBusProperty())
        //        throw new ArgumentException("Can't promote this property to a DataBusPropertyInfo object.");

        //    return new DataBusPropertyInfo(
        //        propertyInfo.Name,
        //        propertyInfo.IsDataBusCompressedProperty(),
        //        propertyInfo.GenerateGetterFunc(instance));
        //}

        public static DataBusPropertyInfo PromoteToDataBusPropertyInfo(this PropertyInfo propertyInfo, Func<object> getterFunc)
        {
            if (!propertyInfo.IsDataBusProperty())
                throw new ArgumentException("Can't promote this property to a DataBusPropertyInfo object.");

            return new DataBusPropertyInfo(
                propertyInfo.Name,
                propertyInfo.IsDataBusCompressedProperty(),
                getterFunc);
        }
    }
}