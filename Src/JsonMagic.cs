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
                IJsonObjectRoot jsonObject = JsonTokenizer.Extract(json);

                return (T) jsonObject.Value;
            }

            return default(T);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class JsonTokenizer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static IJsonObjectRoot Extract(string json)
        {
            IJsonObjectRoot jsonObject=null;

            if (!string.IsNullOrEmpty(json))
            {
                // Determine the type of JSON thing we're dealing with here...
                json = json.Trim();

                if (json.StartsWith("{") && json.EndsWith("}"))
                {
                    jsonObject = new JObject(json);
                }
                else if (json.StartsWith("[") && json.EndsWith("]"))
                {
                    jsonObject = new JArray(json);
                }
                else if (json.StartsWith("\"") && json.EndsWith("\""))
                {
                    // We got a string...
                    jsonObject = new JString(json);
                }
                else if (json.Equals("true", StringComparison.OrdinalIgnoreCase) || json.Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    jsonObject = new JBoolean(json);
                }
                else if (json.Equals("null", StringComparison.OrdinalIgnoreCase))
                {
                    jsonObject = new JNull();
                }
                else
                {
                    // We SHOULD only have a NUMBER here...
                }
            }

            return jsonObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static IDictionary<string, IJsonObjectRoot> ExtractProperties(string json)
        {
            IDictionary<string, IJsonObjectRoot> objectProps = new Dictionary<string, IJsonObjectRoot>();

            if (!string.IsNullOrEmpty(json))
            {
                string copy = json;

                while (copy.Length > 0)
                {
                    copy = copy.Trim();
                    if (copy.StartsWith("\""))
                    {
                        copy = copy.Substring(1);

                        int end = copy.IndexOf("\"");
                        string name = copy.Substring(0, end);

                        end = copy.IndexOf(":");
                        copy = copy.Substring(end + 1).Trim();

                        end = FindMatchingEndToken(copy);

                        string valueString = copy.Substring(0, end);

                        IJsonObjectRoot jsonObject = JsonTokenizer.Extract(valueString);

                        objectProps.Add(name, jsonObject);

                        copy = copy.Substring(copy.IndexOf('"', end));
                    }
                }

            }

            return objectProps;
        }

        public static int FindMatchingEndToken(string json)
        {
            var copy = json.Trim();

            char? startToken = copy[0];
            startToken = startToken == '{' || startToken == '[' || startToken == '"' ? startToken : null;

            int tokenCount = 1;

            for (int i = 1; i < copy.Length && tokenCount > 0; i++)
            {
                if (startToken == '{')
                {
                    if (copy[i] == '{')
                        tokenCount++;
                    else if(copy[i] == '}')
                        tokenCount--;
                }
                else if (startToken == '[')
                {
                    if (copy[i] == '[')
                        tokenCount++;
                    else if (copy[i] == ']')
                        tokenCount--;
                }
                else if (startToken == '"')
                {
                    if (copy[i] == '"')
                        tokenCount--;
                }

                if (tokenCount == 0)
                    return i+1;
            }

            return 0;
        }

    }
}
