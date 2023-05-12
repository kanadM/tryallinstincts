using System;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace KafkaConsumerPOC
{
    public class JsonDeserializer<T> : IDeserializer<T>
    {
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            string json = Deserializers.Utf8.Deserialize(data, isNull, context);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

}
