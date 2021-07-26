using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
        public static async Task SerializeDataToFile<T>(string filename, List<T> modelsList)
        {
            await Task.Run((() =>
            {
                using var stream = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
                var serializer = new YAXSerializer(typeof(List<T>));
                using var textWriter = new StreamWriter(stream);
                serializer.Serialize(modelsList, textWriter);
                textWriter.Flush();
            }));
        }

        /// <summary>
        /// Deserializes data classes list from file
        /// </summary>
        /// <param name="filename">File name</param>
        public static async Task<List<T>> DeserializeGraphDataFromFile<T>(string filename)
        {
            await using var stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var streamReader = new StreamReader(stream);
            stream.Position = 0;
            string xmlData = await streamReader.ReadToEndAsync();
            return await GetSerializationDataAsync<T>(xmlData);
        }

        private static async Task<List<T>> GetSerializationDataAsync<T>(string xmlData)
        {
            return await Task.Run(() => GetSerializationData<T>(xmlData));
        }

        private static List<T> GetSerializationData<T>(string xmlData)
        {
            var deserializer = new YAXSerializer(typeof(List<T>));
            return (List<T>)deserializer.Deserialize(xmlData);
        }
    }
}