using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace KafkaConsumerPOC
{
    //var user = new User { registertime = DateTime.Now.Ticks, userid = "userid_1", gender = "F", regionid = "P14" };
    public class Program
    {
        public static readonly string BokerList = "b-1.dev-stg.jjxx8w.c8.kafka.us-east-1.amazonaws.com:9092,b-2.dev-stg.jjxx8w.c8.kafka.us-east-1.amazonaws.com:9092";
        static async Task Main(string[] args)
        {
            var tasks = new List<Task>();
            var task1 = Task.Factory.StartNew(async () =>
             {
                 using (var producer = KafkaProducerFactory.GetKafkaProducer<Ignore, User>(BokerList))
                 {
                     for (long i = 0; i < Int64.MaxValue; i++)
                     {
                         var rec = new User
                         {
                             registertime = DateTime.Now.Ticks,
                             gender = i % 4 == 0 ? "FEMALE" : "MALE",
                             regionid = $"{DateTime.Now:dd/MM/yy hh:mm:ss:fff}",
                             userid = "userid_" + i,
                         };

                         KafkaProducerFactory.ProduceMessages(producer, "users-old1", null, rec);
                         Thread.Sleep(100);
                     }
                 }

             }, TaskCreationOptions.LongRunning);
            tasks.Add(task1);


            await Task.Factory.StartNew(async () =>
            {
                Console.WriteLine("task2 getting started");
                //using (var consumer = KafkaConsumerFactory.GetKafkaConsumer<Ignore, User>("localhost:9092", "groupId-3", "users-old1"))
                //using (var producer = KafkaProducerFactory.GetKafkaProducer<Ignore, NewUser>("localhost:9092","my-first-hello-trx-3",5000))
                //{
                //    while (true)
                //    {
                //        var oldUsers = KafkaConsumerFactory.ConsumeMessages(consumer);
                //        var newUsers = process(oldUsers);
                //        foreach (var rec in newUsers)
                //        {
                //            KafkaProducerFactory.ProduceMessages(producer, "users-new1", null, rec);
                //        }
                //    }
                //} 
                KafkaConsumerFactory.ConsumeTransformProduce();

            }, TaskCreationOptions.LongRunning);
        }

        public static List<NewUser> process(List<User> oldUsers)
        {
            var newUsers = new List<NewUser>();
            if (oldUsers != null && oldUsers.Any())
            {
                oldUsers.ForEach(u => newUsers.Add(transformToNewUser(u)));
            }

            return newUsers;
        }

        public static NewUser transformToNewUser(User u)
        {
            return new NewUser { registertime = u.registertime, gender = u.gender, noticedTimeStamp = DateTime.Now.Ticks, regionid = u.regionid, userid = u.userid };
        }
    }
    public static class KafkaProducerFactory
    {
        public static IProducer<K, V> GetKafkaProducer<K, V>(string brokerList, string transactionalId = null, int? transactionTimeoutMs = null)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = brokerList,
                TransactionalId = transactionalId,
                TransactionTimeoutMs = transactionTimeoutMs
            };

            return new ProducerBuilder<K, V>(config)
                .SetKeySerializer(new JsonSerializer<K>())
                .SetValueSerializer(new JsonSerializer<V>())
                .Build();
        }
        public static void ProduceMessages<K, V>(IProducer<K, V> producer, string topic, K key, V Value)
        {
            try
            {
                producer.Produce(topic, new Message<K, V>
                {
                    Key = key,
                    Value = Value
                }, onDelivery);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error producing messages");
                Console.WriteLine($"Error producing messages: {ex.Message}");
                Console.WriteLine($"Error producing messages: {ex.StackTrace}");
                throw ex;
            } 
        }

        private static void onDelivery<K,V>(DeliveryReport<K, V> deliveryReport)
        {
            if (deliveryReport.Error.IsError || deliveryReport.Error.IsFatal || deliveryReport.Error.IsLocalError || deliveryReport.Error.IsBrokerError)
            {
                var logMsg = $"Kakfa Producer Error Code: {deliveryReport.Error.Code}, Reason:{deliveryReport.Error.Reason}, " +
                                $" Topic:{deliveryReport.Topic}, Partition:{deliveryReport.Partition.Value}, Offset:{deliveryReport.Offset.Value}, " +
                                $" MsgTimeStamp:{deliveryReport.Message.Timestamp}, MessageKey:{deliveryReport.Message.Key}";
                Console.WriteLine(logMsg);
            }
        }
    }
    public static class KafkaConsumerFactory
    {
        public static IConsumer<K, V> GetKafkaConsumer<K, V>(string brokerList, string groupId, string topic)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = brokerList,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                IsolationLevel = IsolationLevel.ReadCommitted // Set isolation level to ReadCommitted
            };

            var consumer = new ConsumerBuilder<K, V>(config)
                .SetKeyDeserializer(new JsonDeserializer<K>())
                .SetValueDeserializer(new JsonDeserializer<V>())
                .Build();

            consumer.Subscribe(topic);

            return consumer;

        }
        public static ConsumeResult<K, V> ConsumeSingleMessages<K, V>(IConsumer<K, V> consumer)
        {
            try
            {
                do
                {
                    var consumedMessage = consumer.Consume(400);
                    if (consumedMessage != null)
                        return consumedMessage;
                } while (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error consuming messages:");
                Console.WriteLine($"Error consuming messages: {ex.Message}");
                Console.WriteLine($"Error consuming messages: {ex.StackTrace}");
                throw ex;
            }
            return null;
        }
        public static IEnumerable<ConsumeResult<K, V>> ConsumeNMessages<K, V>(IConsumer<K, V> consumer, int N)
        {
            var lst = new List<V>();
            int i = 0;
            while (i < N)
            {
                var consumedMessage = consumer.Consume(400);
                if (consumedMessage != null)
                {
                    yield return consumedMessage;
                    i++;
                }
            }

        }
        public static List<V> ConsumeMessages<K, V>(IConsumer<K, V> consumer)
        {
            try
            {
                var lst = new List<V>();
                for (int i = 0; i < 100; i++)
                {
                    var consumedMessage = consumer.Consume();
                    lst.Add(consumedMessage.Message.Value);
                }
                return lst;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error consuming messages: {ex.Message}");
            }
            return null;
        }

        public static void ConsumeTransformProduce()
        {
            try
            {
                int messagesConsumed =0;
                int batchSize = 100;
                using (var consumer = KafkaConsumerFactory.GetKafkaConsumer<Ignore, User>(Program.BokerList, "groupId-12", "users-old1"))
                {
                    int i = 2;
                    bool flag = true;
                    while (flag)
                    {
                        IProducer<Ignore, NewUser> producer = KafkaProducerFactory.GetKafkaProducer<Ignore, NewUser>(Program.BokerList, $"my-first-hello-trx-12", 2500);
                        Console.WriteLine($"producer{i} created");
                        try
                        {
                            var consumedMessages = KafkaConsumerFactory.ConsumeNMessages(consumer, batchSize).ToList();
                            var oldUsers = consumedMessages.Select(s => (User)s.Message.Value).ToList();
                            var newUsers = Program.process(oldUsers);
                            producer.InitTransactions(TimeSpan.FromSeconds(50)); // Set transaction timeout to 10 seconds
                            producer.BeginTransaction();
                            messagesConsumed += newUsers.Count;
                            foreach (var newUser in newUsers)
                            { 
                                KafkaProducerFactory.ProduceMessages(producer, "users-new5", null, newUser);
                            }
                            Console.WriteLine($"producer{i} producing msgs done");
                            var lastMessageOffset = consumedMessages.Last();
                            producer.SendOffsetsToTransaction(new[] { new TopicPartitionOffset(lastMessageOffset.TopicPartition, lastMessageOffset.Offset + 1) }, consumer.ConsumerGroupMetadata, TimeSpan.FromMilliseconds(50)); // Send the message offset to the transactional producer
                            Console.WriteLine($"producer{i} SendOffsetsToTransaction done");
                            producer.CommitTransaction(TimeSpan.FromSeconds(50)); // Commit the transaction
                            Console.WriteLine($"producer{i} CommitTransaction done");
                            Console.WriteLine($"{DateTime.Now:hh:mm:ss.fff} produced " + batchSize + " new messages on users - new1");
                        }
                        finally
                        {
                            producer.Flush();
                            Console.WriteLine($"producer{i} Flush done");
                            producer.Dispose();
                            Console.WriteLine($"producer{i} Dispose done");
                            i++;
                        }
                        if (messagesConsumed > 100)
                            batchSize = 100;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw ex;
            }
        }
    }

}
