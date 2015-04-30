using System;

namespace Rebus.DataBus
{
    public class DataBusPropertyInfo
    {        
        public DataBusPropertyInfo(string name, bool isCompressedProperty, Func<object> getPropertyInstance)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (getPropertyInstance == null) throw new ArgumentNullException("getPropertyInstance");
            
            Name = name;
            IsCompressedProperty = isCompressedProperty;
            GetPropertyInstance = getPropertyInstance;            
        }

        //public DataBusPropertyInfo(string name, bool isCompressedProperty, Func<object, object> getPropertyInstance, string propertyPath)
        //{
            
        //    if (name == null) throw new ArgumentNullException("name");
        //    if (getPropertyInstance == null) throw new ArgumentNullException("getPropertyInstance");
        //    if (propertyPath == null) throw new ArgumentNullException("propertyPath");

        //    PopertyPath = propertyPath;
        //    Name = name;
        //    IsCompressedProperty = isCompressedProperty;
        //    GetPropertyInstance2 = getPropertyInstance;
        //}

        public string PopertyPath { get; set; }

        public string Name { get; private set; }
        public bool IsCompressedProperty { get; set; }
        //public Func<object, object> GetPropertyInstance2 { get; private set; }
        public Func<object> GetPropertyInstance { get; private set; } 
    }
}
