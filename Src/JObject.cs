using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sdr.JsonMagic
{
    /// <summary>
    /// 
    /// </summary>
    public class JObject : IJObject
    {
        private IDictionary<string, IJsonObjectRoot> _properties;

        #region PROPERTIES

        public JObjectType Type {
            get
            {
                return JObjectType.Object;
            }
        }

        object IJsonObjectRoot.Object
        {
            get { return Object; }
        }

        public object Object
        {
            get;
            private set;
        }

        public IDictionary<string, IJsonObjectRoot> Properties { get; private set; }

        #endregion

        public JObject() : this(null)
        {
        }

        public JObject(string json)
        {
            _properties = new Dictionary<string, IJsonObjectRoot>();

            if (json.StartsWith("{") && json.EndsWith("}"))
            {
                var props = JsonTokenizer.ExtractProperties(json.Substring(1, json.Length - 2));

                var typeProp = props.FirstOrDefault(p => p.Key == "$type");
                var refIdProp = props.FirstOrDefault(p => p.Key == "$refid");
                var itemsProp = props.FirstOrDefault(p => p.Key == "$items");

                if (refIdProp.Key != null)
                {
                    int id = (int)refIdProp.Value.Object;

                    // Extract the object using the reference id...
                    Object = JsonReferenceHolder.Get(id);
                } 
                else if (typeProp.Key != null)        
                {
                    var type = System.Type.GetType(typeProp.Value.Object as string);

                    var obj = Activator.CreateInstance(type, null);

                    PropertyInfo[] propInfos = obj.GetType().GetProperties();

                    // Loop through the JSON properties and assign them to the POCO object...
                    foreach (var jsonprop in props)
                    {
                        if (jsonprop.Key.Equals("$items"))
                        {

                            var items = jsonprop.Value != null ? jsonprop.Value.Object : null;

                            // If this JSON proptery is an '$items', then it means the outer object
                            // is a collection...
                            if (obj is ICollection)
                            {

                            }
                        }
                        else
                        {

                            // Use reflection to populate the target POCO object...
                            var propInfo = propInfos.FirstOrDefault(p => p.Name == jsonprop.Key);
                            if (propInfo != null)
                            {
                                IJsonObjectRoot value = jsonprop.Value;
                                propInfo.SetValue(obj, value.Object, null);
                            }
                        }
                    }

                    var idProp = props.FirstOrDefault(p => p.Key == "$id");
                    if (idProp.Key != null)
                    {
                        int id = (int)idProp.Value.Object;
                        JsonReferenceHolder.Add(obj, id);
                    }

                    Object = obj;
                }
            }
        }

    }
}