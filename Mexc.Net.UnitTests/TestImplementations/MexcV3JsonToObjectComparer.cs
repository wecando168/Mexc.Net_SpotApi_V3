﻿using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mexc.Net.UnitTests.TestImplementations
{
    public class MexcV3JsonToObjectComparer<T>
    {
        private Func<string, T> _clientFunc;

        public MexcV3JsonToObjectComparer(Func<string, T> getClient)
        {
            _clientFunc = getClient;
        }

        public async Task ProcessSubject<K>(
           string folderPrefix,
           Func<T, K> getSubject,
           string[] parametersToSetNull = null,
           Dictionary<string, string> useNestedJsonPropertyForCompare = null,
           Dictionary<string, List<string>> ignoreProperties = null)
        {
            EnumValueTraceListener listener = new EnumValueTraceListener();
            Trace.Listeners.Add(listener);

            MethodInfo[] methods = typeof(K).GetMethods();
            List<MethodInfo> callResultMethods = methods.Where(m => m.Name.EndsWith("Async")).ToList();
            List<string> skippedMethods = new List<string>();

            foreach (MethodInfo method in callResultMethods)
            {
                for (int i = 0; i < 10; i++)
                {
                    string path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
                    FileStream file = null;
                    try
                    {
                        file = File.OpenRead(Path.Combine(path, $"JsonResponses", folderPrefix, $"{method.Name}{(i == 0 ? "": i.ToString())}.txt"));
                    }
                    catch (FileNotFoundException)
                    {
                        if(i == 0)
                            skippedMethods.Add(method.Name);
                        break;
                    }

                    byte[] buffer = new byte[file.Length];
                    await file.ReadAsync(buffer, 0, buffer.Length);
                    file.Close();

                    string json = Encoding.UTF8.GetString(buffer);
                    T client = _clientFunc(json);

                    ParameterInfo[] parameters = method.GetParameters();
                    List<object> input = new List<object>();
                    foreach (ParameterInfo parameter in parameters)
                    {
                        if (parametersToSetNull?.Contains(parameter.Name) == true)
                            input.Add(null);
                        else
                            input.Add(MexcV3TestHelpers.GetTestValue(parameter.ParameterType, 1));
                    }

                    // act
                    CallResult result = (CallResult)await MexcV3TestHelpers.InvokeAsync(method, getSubject(client), input.ToArray());

                    // asset
                    Assert.Null(result.Error, method.Name);

                    object resultData = result.GetType().GetProperty("Data", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(result);
                    ProcessData(method.Name + (i == 0 ? "" : i.ToString()), resultData, json, parametersToSetNull, useNestedJsonPropertyForCompare, ignoreProperties);
                }
            }

            if (skippedMethods.Any())
                Debug.WriteLine("Skipped methods:");
            foreach(string method in skippedMethods)
                Debug.WriteLine(method);

            Trace.Listeners.Remove(listener);
        }

        internal static void ProcessData(string method, object resultData, string json, string[] parametersToSetNull = null,
           Dictionary<string, string> useNestedJsonPropertyForCompare = null,
           Dictionary<string, List<string>> ignoreProperties = null)
        {
            IEnumerable<(PropertyInfo p, JsonPropertyAttribute)> resultProperties = resultData.GetType().GetProperties().Select(p => (p, (JsonPropertyAttribute)p.GetCustomAttributes(typeof(JsonPropertyAttribute), true).SingleOrDefault()));
            JToken jsonObject = JToken.Parse(json);
            if (useNestedJsonPropertyForCompare?.ContainsKey(method) == true)
            {
                jsonObject = jsonObject[useNestedJsonPropertyForCompare[method]];
            }

            if (resultData.GetType().GetInterfaces().Contains(typeof(IDictionary)))
            {
                IDictionary dict = (IDictionary)resultData;
                JObject jObj = (JObject)jsonObject;
                IEnumerable<JProperty> properties = jObj.Properties();
                foreach (JProperty dictProp in properties)
                {
                    if (!dict.Contains(dictProp.Name))
                        throw new Exception($"{method}: Dictionary has no value for {dictProp.Name} while input json `{dictProp.Name}` has value {dictProp.Value}");

                    if (dictProp.Value.Type == JTokenType.Object)
                    {
                        // TODO Some additional checking for objects
                        foreach (JProperty prop in ((JObject)dictProp.Value).Properties())
                            CheckObject(method, prop, dict[dictProp.Name], ignoreProperties);
                    }
                    else
                    {
                        if (dict[dictProp.Name] == default && dictProp.Value.Type != JTokenType.Null)
                        {
                            // Property value not correct
                            throw new Exception($"{method}: Dictionary entry `{dictProp.Name}` has no value while input json has value {dictProp.Value}");
                        }
                    }
                }
            }
            else if (jsonObject.Type == JTokenType.Array)
            {
                JArray jObjs = (JArray)jsonObject;
                IEnumerable list = (IEnumerable)resultData;
                IEnumerator enumerator = list.GetEnumerator();
                foreach (JToken jObj in jObjs)
                {
                    enumerator.MoveNext();
                    if (jObj.Type == JTokenType.Object)
                    {
                        foreach (JProperty subProp in ((JObject)jObj).Properties())
                        {
                            if (ignoreProperties?.ContainsKey(method) == true && ignoreProperties[method].Contains(subProp.Name))
                                continue;
                            CheckObject(method, subProp, enumerator.Current, ignoreProperties);
                        }
                    }
                    else if (jObj.Type == JTokenType.Array)
                    {
                        object resultObj = enumerator.Current;
                        IEnumerable<(PropertyInfo p, ArrayPropertyAttribute)> resultProps = resultObj.GetType().GetProperties().Select(p => (p, p.GetCustomAttributes(typeof(ArrayPropertyAttribute), true).Cast<ArrayPropertyAttribute>().SingleOrDefault()));
                        object arrayConverterProperty = resultObj.GetType().GetCustomAttributes(typeof(JsonConverterAttribute), true).FirstOrDefault();
                        Type jsonConverter = (arrayConverterProperty as JsonConverterAttribute).ConverterType;
                        if (jsonConverter != typeof(ArrayConverter))
                            // Not array converter?
                            continue;

                        int i = 0;
                        foreach (JToken item in jObj.Values())
                        {
                            PropertyInfo arrayProp = resultProps.SingleOrDefault(p => p.Item2.Index == i).p;
                            if (arrayProp != null)
                            {
                                CheckPropertyValue(method, item, arrayProp.GetValue(resultObj), arrayProp.Name, "Array index " + i, arrayProp, ignoreProperties);
                            }
                            i++;
                        }
                    }
                    else
                    {
                        object value = enumerator.Current;
                        if (value == default && ((JValue)jObj).Type != JTokenType.Null)
                        {
                            throw new Exception($"{method}: Array has no value while input json array has value {jObj}");
                        }
                    }
                }
            }
            else
            {
                foreach (JToken item in jsonObject)
                {
                    if (item is JProperty prop)
                    {
                        if (ignoreProperties?.ContainsKey(method) == true && ignoreProperties[method].Contains(prop.Name))
                            continue;

                        CheckObject(method, prop, resultData, ignoreProperties);
                    }
                }
            }

            Debug.WriteLine($"Successfully validated {method}");
        }

        private static void CheckObject(string method, JProperty prop, object obj, Dictionary<string, List<string>> ignoreProperties)
        {
            IEnumerable<(PropertyInfo p, JsonPropertyAttribute)> resultProperties = obj.GetType().GetProperties().Select(p => (p, (JsonPropertyAttribute)p.GetCustomAttributes(typeof(JsonPropertyAttribute), true).SingleOrDefault()));

            // Property has a value
            PropertyInfo property = resultProperties.SingleOrDefault(p => p.Item2?.PropertyName == prop.Name).p;
            if (property is null)
                property = resultProperties.SingleOrDefault(p => p.p.Name == prop.Name).p;
            if (property is null)
                property = resultProperties.SingleOrDefault(p => p.p.Name.ToUpperInvariant() == prop.Name.ToUpperInvariant()).p;

            if (property is null)
            {
                // Property not found
                throw new Exception($"{method}: Missing property `{prop.Name}` on `{obj.GetType().Name}`");
            }

            object propertyValue = property.GetValue(obj);
            if(property.GetCustomAttribute<JsonPropertyAttribute>(true)?.ItemConverterType == null)
                CheckPropertyValue(method, prop.Value, propertyValue, property.Name, prop.Name, property, ignoreProperties);
        }

        private static void CheckPropertyValue(string method, JToken propValue, object propertyValue, string propertyName = null, string propName = null, PropertyInfo info = null, Dictionary<string, List<string>> ignoreProperties = null)
        {
            if (propertyValue == default && propValue.Type != JTokenType.Null && !string.IsNullOrEmpty(propValue.ToString()))
            {
                // Property value not correct
                if (propValue.ToString() != "0")
                    throw new Exception($"{method}: Property `{propertyName}` has no value while input json `{propName}` has value {propValue}");
            }

            if (propertyValue == default && (propValue.Type == JTokenType.Null || string.IsNullOrEmpty(propValue.ToString())) || propValue.ToString() == "0")
                return;

            if (propertyValue.GetType().GetInterfaces().Contains(typeof(IDictionary)))
            {
                IDictionary dict = (IDictionary)propertyValue;
                JObject jObj = (JObject)propValue;
                IEnumerable<JProperty> properties = jObj.Properties();
                foreach (JProperty dictProp in properties)
                {
                    if (!dict.Contains(dictProp.Name))
                        throw new Exception($"{method}: Property `{propertyName}` has no value while input json `{propName}` has value {propValue}");

                    if (dictProp.Value.Type == JTokenType.Object)
                    {
                        // TODO Some additional checking for objects
                    }
                    else
                    {
                        if (dict[dictProp.Name] == default && dictProp.Value.Type != JTokenType.Null)
                        {
                            // Property value not correct
                            throw new Exception($"{method}: Dictionary entry `{dictProp.Name}` has no value while input json has value {propValue} for");
                        }
                    }
                }
            }
            else if (propertyValue.GetType().GetInterfaces().Contains(typeof(IEnumerable))
                && propertyValue.GetType() != typeof(string))
            {
                JArray jObjs = (JArray)propValue;
                IEnumerable list = (IEnumerable)propertyValue;
                IEnumerator enumerator = list.GetEnumerator();
                foreach (JToken jtoken in jObjs)
                {
                    enumerator.MoveNext();
                    object[] typeConverter = enumerator.Current.GetType().GetCustomAttributes(typeof(JsonConverterAttribute), true);
                    if (typeConverter.Any() && ((JsonConverterAttribute)typeConverter.First()).ConverterType != typeof(ArrayConverter))
                        // Custom converter for the type, skip
                        continue;

                    if (jtoken.Type == JTokenType.Object)
                    {
                        foreach (JProperty subProp in ((JObject)jtoken).Properties())
                        {
                            if (ignoreProperties?.ContainsKey(method) == true && ignoreProperties[method].Contains(subProp.Name))
                                continue;

                            CheckObject(method, subProp, enumerator.Current, ignoreProperties);
                        }
                    }
                    else if (jtoken.Type == JTokenType.Array)
                    {
                        object resultObj = enumerator.Current;
                        IEnumerable<(PropertyInfo p, ArrayPropertyAttribute)> resultProps = resultObj.GetType().GetProperties().Select(p => (p, p.GetCustomAttributes(typeof(ArrayPropertyAttribute), true).Cast<ArrayPropertyAttribute>().SingleOrDefault()));
                        object arrayConverterProperty = resultObj.GetType().GetCustomAttributes(typeof(JsonConverterAttribute), true).FirstOrDefault();
                        Type jsonConverter = (arrayConverterProperty as JsonConverterAttribute).ConverterType;
                        if (jsonConverter != typeof(ArrayConverter))
                            // Not array converter?
                            continue;

                        int i = 0;
                        foreach (JToken item in jtoken.Values())
                        {
                            PropertyInfo arrayProp = resultProps.SingleOrDefault(p => p.Item2.Index == i).p;
                            if (arrayProp != null)
                                CheckPropertyValue(method, item, arrayProp.GetValue(resultObj), arrayProp.Name, "Array index " + i, arrayProp, ignoreProperties);

                            i++;
                        }
                    }
                    else
                    {
                        object value = enumerator.Current;
                        if (value == default && ((JValue)jtoken).Type != JTokenType.Null)
                        {
                            throw new Exception($"{method}: Property `{propertyName}` has no value while input json `{propName}` has value {jtoken}");
                        }

                        CheckValues(method, propertyName, (JValue)jtoken, value);
                    }
                }
            }
            else
            {
                if (propValue.Type == JTokenType.Object)
                {
                    foreach (JToken item in propValue)
                    {
                        if (item is JProperty prop)
                        {
                            if (ignoreProperties?.ContainsKey(method) == true && ignoreProperties[method].Contains(prop.Name))
                                continue;

                            CheckObject(method, prop, propertyValue, ignoreProperties);
                        }
                    }
                }
                else
                {
                    if (info.GetCustomAttribute<JsonConverterAttribute>(true) == null
                        && info.GetCustomAttribute<JsonPropertyAttribute>(true)?.ItemConverterType == null)
                        CheckValues(method, propertyName, (JValue)propValue, propertyValue);
                }
            }
        }

        private static void CheckValues(string method, string property, JValue jsonValue, object objectValue)
        {
            if (jsonValue.Type == JTokenType.String)
            {
                if(objectValue is decimal dec)
                {
                    if(jsonValue.Value<decimal>() != dec)
                        throw new Exception($"{method}: {property} not equal: {jsonValue.Value<decimal>()} vs {dec}");
                }
                else if(objectValue is DateTime time)
                {
                    // timestamp, hard to check..
                }
                else if (jsonValue.Value<string>().ToLowerInvariant() != objectValue.ToString().ToLowerInvariant())
                    throw new Exception($"{method}: {property} not equal: {jsonValue.Value<string>()} vs {objectValue.ToString()}");
            }
            else if (jsonValue.Type == JTokenType.Integer)
            {
                if (jsonValue.Value<long>() != Convert.ToInt64(objectValue))
                    throw new Exception($"{method}: {property} not equal: {jsonValue.Value<long>()} vs {Convert.ToInt64(objectValue)}");
            }
            else if (jsonValue.Type == JTokenType.Boolean)
            {
                if (jsonValue.Value<bool>() != (bool)objectValue)
                    throw new Exception($"{method}: {property} not equal: {jsonValue.Value<bool>()} vs {(bool)objectValue}");
            }
        }
    }
}
