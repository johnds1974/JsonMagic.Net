using System.Collections.Generic;

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

        object IJsonObjectRoot.Value
        {
            get { return Value; }
        }

        public object Value {
            get { return this; }
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
            }
        }

    }
}