using System.IO;

namespace Penguin.IO.Serialization
{
    /// <summary>
    /// Contains methods for interacting with Xml strings
    /// </summary>
    public static class XML
    {
        /// <summary>
        /// Deserializes and object from an XML string or file
        /// </summary>
        /// <typeparam name="T">The resulting object type</typeparam>
        /// <param name="source">The source file or XML string</param>
        /// <returns>The deserialized object</returns>
        public static T ToObject<T>(string source)
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));

            if (File.Exists(source))
            {
                using (StringReader sr = new StringReader(File.ReadAllText(source)))
                {
                    return (T)ser.Deserialize(sr);
                }
            }
            else
            {
                using (StringReader sr = new StringReader(source))
                {
                    return (T)ser.Deserialize(sr);
                }
            }
        }
    }
}