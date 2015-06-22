using System.Collections.Generic;

namespace Sdr.JsonMagic
{
    public class JArray : IJArray
    {
        private IList<IJsonObjectRoot> _items;

        public JObjectType Type
        {
            get
            {
                return JObjectType.Array;
            }
        }

        public object[] Value 
        { 
            get { return null; } 
        }

        object IJsonObjectRoot.Value
        {
            get { return Value; }
        }

        public IList<IJsonObjectRoot> Items
        {
            get { return _items; }
        }

        public JArray() : this(null)
        {
        }

        public JArray(string json)
        {
            _items = new List<IJsonObjectRoot>();
        }
    }
}