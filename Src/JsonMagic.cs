using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sdr.JsonMagic
{
    /// <summary>
    /// 
    /// </summary>
    public class JsonMagic
    {
        private static StringBuilder _jsonString;
        private static IDictionary<object, int> _refsDict;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ToJson(object o)
        {
            _jsonString = new StringBuilder();
            _refsDict = new Dictionary<object, int>();

            _jsonString.Append(ToJsonInternal(o));

            return _jsonString.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static string ToJsonInternal(object o, bool isRoot=true, bool isAssigneeSealed=true)
        {
            StringBuilder sb = new StringBuilder();

            if (o != null)
            {
                Type type = o.GetType();

                if (!(o is string) && !type.IsPrimitive)
                {
                    if (_refsDict.ContainsKey(o))
                    {
                        sb.Append(string.Format("{{\"$refid\": {0}}}", _refsDict[o]));
                        return sb.ToString();
                    }
                    else
                    {
                        _refsDict.Add(o, _refsDict.Count + 1);
                    }
                }

                if (type.IsArray)
                {
                    sb.Append("[");
                }
                else if (type.IsClass || (type.IsGenericType))
                {
                    if (o is string)
                    {
                        sb.Append("\"");
                    }
                    //else if (o is IEnumerable)
                    //{
                    //    sb.Append("[");
                    //}
                    else
                    {
                        if (isRoot || !isAssigneeSealed)
                        {
                            sb.Append(string.Format(
                                "{{\"$type\": \"{0}\", \"$id\": {1},", GetShortTypeName(type), _refsDict[o]));
                        }
                    }
                }
                else if (type.IsValueType)
                {
                    if (type.IsPrimitive)
                    {
                    }
                    else
                    {
                        sb.Append("{");
                    }
                }


                if (type.IsArray)
                {
                    foreach (var item in (Array)o)
                    {
                        sb.AppendFormat("{0},", ToJsonInternal(item, false));
                    }
                }
                else if(type.IsClass) 
                {
                    if (o is string)
                    {
                        sb.Append(o);
                    }
                    else
                    {
                        if (o is IEnumerable)
                        {
                            sb.Append("\"$items\": [");

                            // Go through each item in the Enumerable, and Jsonize it...
                            foreach (var item in (IEnumerable)o)
                            {
                                sb.Append(string.Format("{0},", ToJsonInternal(item, false, false)));
                            }

                            sb.Append("]");
                        }
                        else
                        {
                            // Get each property and try jsonize them...
                            var props = type.GetProperties();

                            foreach (var propertyInfo in props)
                            {
                                sb.Append(
                                    string.Format("\"{0}\": {1},", 
                                        propertyInfo.Name, 
                                        ToJsonInternal(
                                            propertyInfo.GetValue(o, null),
                                            false,
                                            propertyInfo.PropertyType.IsSealed)));
                            }
                        }
                    }
                }
                else if (type.IsValueType)
                {
                    if (type.IsPrimitive)
                    {
                        sb.Append(o);
                    }
                    else
                    {
                        var props = type.GetProperties();

                        // Go through each object Property and Jsonize it...
                        foreach (var propertyInfo in props)
                        {
                            sb.Append(
                                string.Format("\"{0}\": {1},", 
                                propertyInfo.Name,
                                ToJsonInternal(propertyInfo.GetValue(o, null))));
                        }
                    }
                }


                if (type.IsArray)
                {
                    sb.Append("]");
                }
                else if (type.IsClass)
                {
                    if (o is string)
                    {
                        sb.Append("\"");
                    }
                    else
                    {
                        sb.Append("}");
                    }
                }
                else if (type.IsValueType)
                {
                    if (type.IsPrimitive)
                    {
                    }
                    else
                    {
                        sb.Append("}");
                    }
                }


            }

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetShortTypeName(Type type)
        {
            if (type != null)
            {
                //var str = Regex.Replace(
                //    type.AssemblyQualifiedName, 
                //    @"(,\s*Version=(\.?\d*)*)|(,\s*Culture=\w*)|(,\s*PublicKeyToken=\w*)",
                //    @"");

                return type.AssemblyQualifiedName;
            }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static object FromJson(string json)
        {
            // Delegate to the method that actually does the work...
            return FromJson<object>(json);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T FromJson<T>(string json)
            where T:class
        {
            if (!string.IsNullOrEmpty(json))
            {
                // Check if we are a string...
                if (json.StartsWith("\"") && json.EndsWith("\""))
                {
                    return json.Substring(1, json.Length - 1) as T;
                }
                else if (json.StartsWith("{") && json.EndsWith("}"))
                {
                    // Split the 'object' string into NAME : VALUE pairs...
                }
            }

            return default(T);
        }

    }
}
