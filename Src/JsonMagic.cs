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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ToJson(object o)
        {
            JsonReferenceHolder.Reset();

            _jsonString = new StringBuilder();

            _jsonString.Append(ToJsonInternal(o));

            return _jsonString.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static string ToJsonInternal(object o)
        {
            StringBuilder sb = new StringBuilder();

            if (o != null)
            {
                Type type = o.GetType();

                #region WRITE and STORE REFERENCE ID
                if (!(o is string) && !type.IsPrimitive)
                {
                    if (JsonReferenceHolder.ContainsObject(o))
                    {
                        sb.Append(string.Format("{{\"$refid\": {0}}}", JsonReferenceHolder.GetId(o)));
                        return sb.ToString();
                    }
                    else
                    {
                        JsonReferenceHolder.Add(o);
                    }
                }
                #endregion

                #region WRITE OPENING TAG
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
                    else
                    {
                        if (!type.IsSealed)
                        {
                            sb.Append(string.Format(
                                "{{\"$type\": \"{0}\", \"$id\": {1},", GetShortTypeName(type), JsonReferenceHolder.GetId(o)));
                        }
                        else
                        {
                            sb.Append("{ ");
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
                #endregion

                #region WRITE DATA
                if (type.IsArray)
                {
                    foreach (var item in (Array)o)
                    {
                        sb.AppendFormat("{0},", ToJsonInternal(item));
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
                        if (o is IDictionary)
                        {
                            IDictionary dict = o as IDictionary;
                            sb.Append(string.Format("\"$keys\": {0},", ToJsonInternal(ToArray(dict.Keys))));
                            sb.Append(string.Format("\"$values\": {0}", ToJsonInternal(ToArray(dict.Values))));
                        }
                        else if (o is IEnumerable)
                        {
                            sb.Append("\"$items\": [");

                            // Go through each item in the Enumerable, and Jsonize it...
                            foreach (var item in (IEnumerable)o)
                            {
                                sb.Append(string.Format("{0},", ToJsonInternal(item)));
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
                                            propertyInfo.GetValue(o, null)/*,
                                            propertyInfo.PropertyType.IsSealed*/
                                        ))
                                    );
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
                #endregion

                #region WRITE CLOSING TAG
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
                #endregion

            }

            return sb.ToString();
        }

        private static object[] ToArray(IEnumerable list)
        {

            if (list != null)
            {
                object[] array = list.Cast<object>().ToArray();
                return array;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetShortTypeName(Type type)
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
                JsonReferenceHolder.Reset();

                IJsonObjectRoot jsonObject = JsonTokenizer.Extract(json);

                return (T) jsonObject.Object;
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
                    json = json.TrimStart('"').TrimEnd('"');
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
                    // We SHOULD only have a NUMBER, either an integer or a floating-point...
                    int intValue;
                    if (int.TryParse(json, out intValue))
                    {
                        jsonObject = new JInteger(json);
                    }
                    else
                    {
                        double dblValue;
                        if (double.TryParse(json, out dblValue))
                        {
                            jsonObject = new JDouble(json);
                        }
                    }
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

                while (true)
                {
                    // Find the first "
                    int index = copy.IndexOf('"');

                    if (index >= 0)
                    {
                        copy = copy.Substring(index + 1);

                        int end = copy.IndexOf("\"");
                        string name = copy.Substring(0, end);

                        end = copy.IndexOf(":");
                        copy = copy.Substring(end + 1).Trim();

                        end = FindMatchingEndToken(copy);

                        string valueString = copy.Substring(end);

                        IJsonObjectRoot jsonObject = JsonTokenizer.Extract(valueString);

                        objectProps.Add(name, jsonObject);

                        copy = copy.Substring(end);

//                        copy = copy.Substring(copy.IndexOf('"', end));
                    }
                    else
                    {
                        break;
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
                else
                {
                    // we are encountering either 'true/false' or 'null' or a number +-10 or +-10.55
                    // If we reach a SPACE or COMMA then were at the end of this value...
                    if (copy[i] == ',' || copy[i] == ' ')
                        tokenCount--;
                }

                if (tokenCount == 0)
                    return i + (startToken.HasValue ? 1 : 0);
            }

            return 0;
        }

    }
}
