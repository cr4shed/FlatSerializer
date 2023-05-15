using Newtonsoft.Json;
using System.Collections;
using System.Reflection;

namespace FlatSerializer
{
    /// <summary>
    /// FlatSerializer class used to serialize objects to a flattened JSON structure.
    /// </summary>
    public class FlatSerializer
    {
        /// <summary>
        /// Flag to determine if the first property has been processed.
        /// </summary>
        private static bool IsFirstProp { get; set; } = true;

        /// <summary>
        /// Generic method to deserialize JSON into any type.
        /// </summary>
        /// <typeparam name="T">Type to deserialize into.</typeparam>
        /// <param name="json">JSON to deserialize.</param>
        /// <returns>Deserialized object.</returns>
        public T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Deserialize JSON into a dictionary of (string, object).
        /// </summary>
        /// <param name="json">JSON to deserialize.</param>
        /// <returns>Deserialized object.</returns>
        public Dictionary<string, object> DeserializeObject(string json)
        {
            return DeserializeObject<Dictionary<string, object>>(json);
        }

        /// <summary>
        /// Flattens and serializes the object.
        /// </summary>
        /// <param name="obj">Object to serialize.</param>
        /// <returns>Serialized object as JSON.</returns>
        public string FlattenObjectToJson(object obj)
        {
            IsFirstProp = true;
            return $"{{ {RecursiveFlattenToJson(obj)} }}";
        }

        /// <summary>
        /// Recursively flatten to JSON.
        /// </summary>
        /// <param name="obj">Object to serialize.</param>
        /// <param name="path">Path of the object.</param>
        /// <returns>Serialized object as JSON.</returns>
        private static string RecursiveFlattenToJson(object obj, string path = "")
        {
            if (IsEnumerable(obj.GetType()))
            {
                return FlattenCollectionToJson(obj, path);
            }
            else
            {
                return FlattenPropertiesToJson(obj, obj.GetType().GetProperties(), path);
            }
        }

        /// <summary>
        /// Serializes all properties in a collection.
        /// </summary>
        /// <param name="collection">Collection to serialize.</param>
        /// <param name="path">Path of the object.</param>
        /// <returns>Serialized object as JSON.</returns>
        private static string FlattenCollectionToJson(object collection, string path)
        {
            string json = "";

            if (collection is IDictionary)
            {
                IDictionary dictionary = (IDictionary)collection;
                foreach (DictionaryEntry item in dictionary)
                {
                    json += RecursiveFlattenToJson(item.Value, path + item.Key.ToString());
                }
            }
            else
            {
                IEnumerable enumerable = collection as IEnumerable;
                foreach (var item in enumerable)
                {
                    json += RecursiveFlattenToJson(item, path);
                }
            }

            return json;
        }

        /// <summary>
        /// Serializes all fields in an object.
        /// </summary>
        /// <param name="obj">Object to serialize.</param>
        /// <param name="properties">List of properties in this object to serialize.</param>
        /// <param name="path">Path of the properties.</param>
        /// <returns>Serialized object as JSON.</returns>
        private static string FlattenPropertiesToJson(object obj, PropertyInfo[] properties, string path)
        {
            string json = "";
            FlatIdentifierAttribute identifier = (FlatIdentifierAttribute)obj.GetType().GetCustomAttribute(typeof(FlatIdentifierAttribute));

            if (identifier != null)
            {
                foreach (var property in properties)
                {
                    if (identifier?.GetIdentifierProperty() == property.Name)
                    {
                        path += identifier.GetFormattedValue(property.GetValue(obj).ToString());
                    }
                }
            }
            foreach (var prop in properties)
            {
                if (prop.GetCustomAttribute(typeof(FlatIgnoreAttribute)) != null)
                {
                    continue;
                }
                else if (prop.GetCustomAttribute(typeof(FlatSkipAttribute)) != null)
                {
                    json += JsonConvert.SerializeObject(prop.GetValue(obj));
                }
                else if (IsEnumerable(prop.PropertyType))
                {
                    json += FlattenCollectionToJson(prop.GetValue(obj), path + prop.Name);
                }
                else
                {
                    json += IsFirstProp ? "" : ",";
                    json += $"\"{path + prop.Name}\": \"{prop.GetValue(obj)}\"";
                    IsFirstProp = false;
                }
            }
            return json;
        }

        /// <summary>
        /// Checks if an object has a base type of IEnumerable.
        /// </summary>
        /// <param name="obj">Object to check.</param>
        /// <returns>True if object has a base type of IEnumerable.</returns>
        private static bool IsEnumerable(object obj)
        {
            return typeof(IEnumerable).IsAssignableFrom(obj.GetType()) && obj.GetType() != typeof(string);
        }
    }
}