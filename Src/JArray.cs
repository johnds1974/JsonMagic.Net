using System.Collections.Generic;
using System.Text.RegularExpressions;

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

        public object[] Object 
        { 
            get { return null; } 
        }

        object IJsonObjectRoot.Object
        {
            get { return Object; }
        }

        public IList<IJsonObjectRoot> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// 
        /// </summary>
        public JArray() : this(null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        public JArray(string json)
        {
            _items = new List<IJsonObjectRoot>();

            if (!string.IsNullOrEmpty(json))
            {
                json = json.Trim();
                if (json.StartsWith("[") && json.EndsWith("]"))
                {
                    json = json.Remove(0, 1);
                    json = json.Remove(json.Length - 1, 1);

                    while (json.Length > 0)
                    {

                        int end = JsonTokenizer.FindMatchingEndToken(json);

                        IJsonObjectRoot item = JsonTokenizer.Extract(json.Substring(0, end));

                        _items.Add(item);

                        json = json.Substring(end);

//                        json = Regex.Replace(json, @"^[\[\s]\s*", "");
                    }

                }
            }
        }
    }
}