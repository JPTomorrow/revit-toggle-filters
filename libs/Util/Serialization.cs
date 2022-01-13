using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using JPMorrow.Tools.Diagnostics;

namespace JPMorrow.Tools.Data
{
    /// <summary>
    /// Class for Binary Serialization on data (NOTE: NOT SAFE FOR REVIT)
    /// </summary>
    public static class BinarySerializer
    {
        /// <summary>Formatter used for serialization</summary>
        private static readonly BinaryFormatter formatter = new BinaryFormatter();

        /// <summary>
        /// Serialize an object
        /// </summary>
        /// <param name="toSerialize">object to serialize</param>
        /// <returns>byte array of serialized object</returns>
        public static byte[] Serialize(object toSerialize)
        {
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, toSerialize);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Deserialize an object
        /// </summary>
        /// <typeparam name="T">Type to deserialize</typeparam>
        /// <param name="serialized">serialized byte array</param>
        /// <returns>deserialized object</returns>
        public static T Deserialize<T>(byte[] serialized = null)
        {
            using (var stream = new MemoryStream(serialized))
            {
                var result = (T)formatter.Deserialize(stream);
                return result;
            }
        }

        /// <summary>
        /// Serialize an object to a file
        /// </summary>
        /// <param name="toSerialize">object to serialize</param>
        /// <param name="file">file to serialize to</param>
        /// <returns>byte array of the serialized object</returns>
        public static byte[] SerializeToFile(object toSerialize, string file)
        {
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, toSerialize);

                List<string> split = file.Split('\\').ToList();
                split.Remove(split.Last());
                if (!Directory.Exists(String.Join("\\", split)))
                    Directory.CreateDirectory(String.Join("\\", split));

                using (var fs = new FileStream(file, FileMode.OpenOrCreate))
                {
                    stream.WriteTo(fs);
                }
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Deserialize an object from a file
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize</typeparam>
        /// <param name="file">filepath of object</param>
        /// <param name="typeSub">type surrogate interface for type substitution</param>
        /// <returns>deserialized object of type T</returns>
        public static T DeserializeFromFile<T>(string file, ISurrogateSelector typeSub)
        {
            if (!File.Exists(file)) return default;
            using (var fs = new FileStream(file, FileMode.Open))
            {
                formatter.SurrogateSelector = typeSub;
                var result = (T)formatter.Deserialize(fs);
                return result;
            }
        }
    }

    /// <summary>
    /// A Class for JSON Serialzation of objects
    /// </summary>
    public static class JSON_Serialization
    {
        /// <summary>
        /// Serialize an object to an object graph and save it to a file
        /// </summary>
        /// <typeparam name="T">Type of the object to serialize</typeparam>
        /// <param name="graph">object to serialize to graph</param>
        /// <param name="file">file to save serialization to</param>
        public static void SerializeToFile<T>(object graph, string file)
        {
            if (File.Exists(file))
                File.Delete(file);
            List<string> split = file.Split('\\').ToList();
            split.Remove(split.Last());
            if (!Directory.Exists(String.Join("\\", split)))
                Directory.CreateDirectory(String.Join("\\", split));

            var stream1 = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(T));
            ser.WriteObject(stream1, graph);
            using (var fs = new FileStream(file, FileMode.OpenOrCreate))
            {
                ser.WriteObject(fs, graph);
            }
        }

        /// <summary>
        /// Deserialize an object from a file
        /// </summary>
        /// <typeparam name="T">Type of object ot deserialize</typeparam>
        /// <param name="file">file to deserialize from</param>
        /// <returns></returns>
        public static T DeserializeFromFile<T>(string file)
        {
            if (!File.Exists(file)) return default;
            var ser = new DataContractJsonSerializer(typeof(T));
            using (var fs = new FileStream(file, FileMode.Open))
            {
                return (T)ser.ReadObject(fs);
            }
        }
    }
}
