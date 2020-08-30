using System.IO;
using System.Xml.Serialization;

namespace StudentsDiary
{
    public class FileHelper<T> where T: new()
    {
        private string _filePath;
        public FileHelper(string filepath)
        {
            _filePath = filepath;
        }
        public void SerializeToFile(T students)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var streamWriter = new StreamWriter(_filePath))
            {
                serializer.Serialize(streamWriter, students);
                streamWriter.Close();
            }
        }

        public T DeserializeFromFile()
        {
            if (!File.Exists(_filePath))
                return new T();

            var serializer = new XmlSerializer(typeof(T));
            using (var streamReader = new StreamReader(_filePath))
            {
                var students = (T)serializer.Deserialize(streamReader);
                streamReader.Close();
                return students;
            }
        }
    }
}
