using Confluent.Kafka;
using Newtonsoft.Json;

namespace KafkaConsumerPOC
{
    public class JsonSerializer<T> : ISerializer<T>
    {
        public byte[] Serialize(T data, SerializationContext context)
        {
            return Serializers.Utf8.Serialize(JsonConvert.SerializeObject(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), context);
        }
    }

}
