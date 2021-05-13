using System.Collections.Generic;
using System.IO;
using YAXLib;

namespace ControlsLibrary.FileSerialization
{
    /// <summary>
    /// WPF implementation of file serialization and deserialization
    /// </summary>
    public static class FileServiceProviderWpf
    {
        /// <summary>
        /// Serializes data classes list to file
        /// </summary>
        /// <param name="filename">File name</param>
        /// <param name="modelsList">Data classes list</param>
        public static void SerializeDataToFile<T>(string filename, List<T> modelsList)
        {
            using (FileStream stream = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                var serializer = new YAXSerializer(typeof(List<T>));
                using (var textWriter = new StreamWriter(stream))
                {
                    serializer.Serialize(modelsList, textWriter);
                    textWriter.Flush();
                }
            }
        }

        /// <summary>
        /// Deserializes data classes list from file
        /// </summary>
        /// <param name="filename">File name</param>
        public static List<T> DeserializeGraphDataFromFile<T>(string filename)
        {
            using (FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var deserializer = new YAXSerializer(typeof(List<T>));
                using (var textReader = new StreamReader(stream))
                {
                    return (List<T>)deserializer.Deserialize(textReader);
                }
            }
        }
    }
}