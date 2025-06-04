namespace DynamicWebTWAIN.RestClient.Internal
{
    public interface IJsonSerializer
    {
        /// <summary>
        /// Serializes the given object to a JSON string.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        string Serialize(object item);

        /// <summary>
        /// Deserializes the given JSON string to an object of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        T Deserialize<T>(string json);
    }
}
