using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdr.JsonMagic
{
    // Internal class to handle storage of object references during serialization/deserialization...
    internal class JsonReferenceHolder
    {
        private static IDictionary<object, int> _refsDict;

        /// <summary>
        /// 
        /// </summary>
        static JsonReferenceHolder()
        {
            Reset();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Reset()
        {
            _refsDict = new Dictionary<object, int>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public static void Add(object obj) 
        {
            _refsDict.Add(obj, _refsDict.Count + 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="id"></param>
        public static void Add(object obj, int id)
        {
            _refsDict.Add(obj, id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static object Get(int id)
        {
            if (_refsDict.Any(kvp => kvp.Value == id))
            {
                KeyValuePair<object, int> kv = _refsDict.First(kvp => kvp.Value == id);

                return kv.Value;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetId(object obj)
        {
            if (ContainsObject(obj))
            {
                return _refsDict.First(kvp => kvp.Key == obj).Value;
            }

            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool ContainsObject(object obj)
        {
            return _refsDict.Any(kvp => kvp.Key == obj);
        }

    }
}
