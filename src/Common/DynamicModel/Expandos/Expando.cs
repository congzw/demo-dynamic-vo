﻿#define SupportXmlSerialization
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Common.DynamicModel.Expandos
{
    /// <summary>
    /// Class that provides extensible properties and methods to an
    /// existing object when cast to dynamic. This
    /// dynamic object stores 'extra' properties in a dictionary or
    /// checks the actual properties of the instance passed via 
    /// constructor.
    /// 
    /// This class can be subclass to extend an existing type or 
    /// you can pass in an instance to extend. Properties (both
    /// dynamic and strongly typed) can be accessed through an 
    /// indexer.
    /// 
    /// This type allows you three ways to access its properties:
    /// 
    /// Directly: any explicitly declared properties are accessible
    /// Dynamic: dynamic cast allows access to dictionary and native properties/methods
    /// Dictionary: Any of the extended properties are accessible via IDictionary interface
    /// </summary>
    [Serializable]
    public class Expando : DynamicObject, IDynamicMetaObjectProvider
    {
        /// <summary>
        /// Instance of object passed in
        /// </summary>
        object Instance;

        /// <summary>
        /// Cached type of the instance
        /// </summary>
        Type InstanceType;

        protected PropertyInfo[] InstancePropertyInfo
        {
            get
            {
                if (_InstancePropertyInfo == null && Instance != null)
                {
                    //fix indexer "item" bug!
                    //instance props should not include the indexer: this[""] => "item"
                    InstanceType = Instance.GetType();
                    var propertyInfos = InstanceType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.GetProperty);
                    _InstancePropertyInfo = propertyInfos.Where(x => x.GetIndexParameters().Length == 0).ToArray();
                }
                return _InstancePropertyInfo;
            }
        }
        PropertyInfo[] _InstancePropertyInfo;


        /// <summary>
        /// String Dictionary that contains the extra dynamic values
        /// stored on this object/instance
        /// </summary>        
        /// <remarks>Using PropertyBag to support XML Serialization of the dictionary</remarks>
#if SupportXmlSerialization
        public PropertyBag Properties = new PropertyBag();
#else
        public Dictionary<string,object> Properties = new Dictionary<string,object>();  
#endif        

        //public Dictionary<string,object> Properties = new Dictionary<string, object>();

        /// <summary>
        /// This constructor just works off the internal dictionary and any 
        /// public properties of this object.
        /// 
        /// Note you can subclass Expando.
        /// </summary>
        public Expando()
        {
            Initialize(this);
        }

        /// <summary>
        /// Allows passing in an existing instance variable to 'extend'.        
        /// </summary>
        /// <remarks>
        /// You can pass in null here if you don't want to 
        /// check native properties and only check the Dictionary!
        /// </remarks>
        /// <param name="instance"></param>
        public Expando(object instance)
        {
            Initialize(instance);
        }

        /// <summary>
        /// Create an Expando from a dictionary
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="expando">Expando instance</param>
        public Expando(IDictionary<string, object> dict)
        {
            var expando = this;

            Initialize(expando);

            Properties = new PropertyBag();

            foreach (var kvp in dict)
            {
                var kvpValue = kvp.Value;
                if (kvpValue is IDictionary<string, object>)
                {
                    var expandoVal = new Expando(kvpValue);
                    expando[kvp.Key] = expandoVal;
                }
                else if (kvp.Value is ICollection)
                {
                    // iterate through the collection and convert any string-object dictionaries
                    // along the way into expando objects
                    var objList = new List<object>();
                    foreach (var item in (ICollection)kvp.Value)
                    {
                        var itemVals = item as IDictionary<string, object>;
                        if (itemVals != null)
                        {
                            var expandoItem = new Expando(itemVals);
                            objList.Add(expandoItem);
                        }
                        else
                        {
                            objList.Add(item);
                        }
                    }
                    expando[kvp.Key] = objList;
                }
                else
                {
                    expando[kvp.Key] = kvpValue;
                }
            }
        }
        
        protected void Initialize(object instance)
        {
            Instance = instance;
            if (instance != null)
                InstanceType = instance.GetType();
        }

        /// <summary>
        /// Return both instance and dynamic names.
        /// 
        /// Important to return both so JSON serialization with 
        /// Json.NET works.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            var memberNames = new List<string>();

            //按需触发Lazy属性
            var dynamicMembers = GetReturnDynamicMembers();
            foreach (var dynamicMember in dynamicMembers)
            {
                memberNames.Add(dynamicMember);
                if (LazyBag.CachedLazyProperties.ContainsKey(dynamicMember))
                {
                    LazyBag.TryGetLazyProp(dynamicMember, out var result);
                }
            }

            var instanceMembers = GetReturnInstancePropertyInfos();
            foreach (var instanceMember in instanceMembers)
            {
                memberNames.Add(instanceMember.Name);
            }
            return memberNames;
        }

        /// <summary>
        /// Try to retrieve a member by name first from instance properties
        /// followed by the collection entries.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            // first check the Properties collection for member
            if (Properties.Keys.Contains(binder.Name))
            {
                result = Properties[binder.Name];
                return true;
            }

            // Next check for Public properties via Reflection
            if (Instance != null)
            {
                try
                {
                    return GetProperty(Instance, binder.Name, out result);
                }
                catch { }
            }

            // failed to retrieve a property
            result = null;
            return false;
        }
        
        /// <summary>
        /// Property setter implementation tries to retrieve value from instance 
        /// first then into this object
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {

            // first check to see if there's a native property to set
            if (Instance != null)
            {
                try
                {
                    bool result = SetProperty(Instance, binder.Name, value);
                    if (result)
                        return true;
                }
                catch
                {
                    return false;
                }
            }

            // no match - set or add to dictionary
            Properties[binder.Name] = value;
            return true;
        }

        /// <summary>
        /// Dynamic invocation method. Currently allows only for Reflection based
        /// operation (no ability to add methods dynamically).
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (Instance != null)
            {
                try
                {
                    // check instance passed in for methods to invoke
                    if (InvokeMethod(Instance, binder.Name, args, out result))
                        return true;
                }
                catch { }
            }

            result = null;
            return false;
        }
        
        /// <summary>
        /// Reflection Helper method to retrieve a property
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected bool GetProperty(object instance, string name, out object result)
        {
            if (instance == null)
                instance = this;

            var miArray = InstanceType.GetMember(name, BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
            if (miArray != null && miArray.Length > 0)
            {
                var mi = miArray[0];
                if (mi.MemberType == MemberTypes.Property)
                {
                    result = ((PropertyInfo)mi).GetValue(instance, null);
                    return true;
                }
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Reflection helper method to set a property value
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool SetProperty(object instance, string name, object value)
        {
            if (instance == null)
                instance = this;

            var miArray = InstanceType.GetMember(name, BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);
            if (miArray != null && miArray.Length > 0)
            {
                var mi = miArray[0];
                if (mi.MemberType == MemberTypes.Property)
                {
                    ((PropertyInfo)mi).SetValue(Instance, value, null);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Reflection helper method to invoke a method
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected bool InvokeMethod(object instance, string name, object[] args, out object result)
        {
            if (instance == null)
                instance = this;

            // Look at the instanceType
            var miArray = InstanceType.GetMember(name,
                                    BindingFlags.InvokeMethod |
                                    BindingFlags.Public | BindingFlags.Instance);

            if (miArray != null && miArray.Length > 0)
            {
                var mi = miArray[0] as MethodInfo;
                result = mi.Invoke(Instance, args);
                return true;
            }

            result = null;
            return false;
        }
        
        /// <summary>
        /// Convenience method that provides a string Indexer 
        /// to the Properties collection AND the strongly typed
        /// properties of the object by name.
        /// 
        /// // dynamic
        /// exp["Address"] = "112 nowhere lane"; 
        /// // strong
        /// var name = exp["StronglyTypedProperty"] as string; 
        /// </summary>
        /// <remarks>
        /// The getter checks the Properties dictionary first
        /// then looks in PropertyInfo for properties.
        /// The setter checks the instance properties before
        /// checking the Properties dictionary.
        /// </remarks>
        /// <param name="key"></param>
        /// 
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                // try reflection on instanceType
                if (GetProperty(Instance, key, out var result))
                    return result;

                //wrap with lazy
                return LazyBag.TryGetLazyProp(key, out var theValue);
            }
            set
            {
                // check instance for existance of type first
                var miArray = InstanceType.GetMember(key, BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
                if (miArray.Length > 0)
                {
                    SetProperty(Instance, key, value);
                }
                else
                {
                    //wrap with lazy
                    LazyBag.TrySetLazyProp(key, value);
                }
            }
        }

        /// <summary>
        /// Checks whether a property exists in the Property collection
        /// or as a property on the instance
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<string, object> item, bool includeInstanceProperties = false)
        {
            bool res = Properties.ContainsKey(item.Key);
            if (res)
                return true;

            if (includeInstanceProperties && Instance != null)
            {
                foreach (var prop in this.InstancePropertyInfo)
                {
                    if (prop.Name == item.Key)
                        return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// Converts an <see cref="IDictionary&lt;string, object&gt;"/> into an <see cref="Expando"/>
        /// </summary>
        /// <returns><see cref="Expando"/></returns>
        public static Expando ToIndexableExpando(IDictionary<string, object> dict)
        {
            var expando = new Expando();


            foreach (var kvp in dict)
            {
                var kvpValue = kvp.Value as IDictionary<string, object>;
                if (kvpValue != null)
                {
                    var expandoVal = ToIndexableExpando(kvpValue);
                    expando[kvp.Key] = expandoVal;
                }
                else if (kvp.Value is ICollection)
                {
                    // iterate through the collection and convert any string-object dictionaries
                    // along the way into expando objects
                    var objList = new List<object>();
                    foreach (var item in (ICollection)kvp.Value)
                    {
                        var itemVals = item as IDictionary<string, object>;
                        if (itemVals != null)
                        {
                            var expandoItem = ToIndexableExpando(itemVals);
                            objList.Add(expandoItem);
                        }
                        else
                        {
                            objList.Add(item);
                        }
                    }
                    expando[kvp.Key] = objList;
                }
                else
                {
                    expando[kvp.Key] = kvp.Value;
                }
            }
            return expando;
        }

        #region support lazy props
        
        private PropertyLazyBag _lazyBag;
        protected PropertyLazyBag LazyBag
        {
            get { return _lazyBag ??= new PropertyLazyBag(Properties); }
        }

        /// <summary>
        /// 最终返回的实例属性
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<PropertyInfo> GetReturnInstancePropertyInfos()
        {
            return Instance != null ? this.InstancePropertyInfo : Enumerable.Empty<PropertyInfo>();
        }

        /// <summary>
        /// 最终返回的动态属性
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<string> GetReturnDynamicMembers()
        {
            return LazyBag.CachedLazyProperties.Keys;
        }

        #endregion
    }
}
