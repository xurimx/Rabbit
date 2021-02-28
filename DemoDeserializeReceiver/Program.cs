using DemoApp.Contracts;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace DemoDeserializeReceiver
{
    public class Program
    {
        public static void Main()
        {
            var factory = new ConnectionFactory() { HostName = "broker" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("default", ExchangeType.Topic);
                channel.QueueDeclare("serialize", true, false, false);
                channel.QueueBind("serialize", "default", "");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    object type = null;
                    ea.BasicProperties.Headers?.TryGetValue("MessageType", out type);

                    if (type != null && Enum.Parse<MessageType>(type.ToString()) == MessageType.Message)
                    {
                        Message msg = JsonConvert.DeserializeObject<Message>(message);

                        Console.WriteLine(" [x] Received {0}", message);
                        Console.WriteLine(" [x] Deserialized {0}", msg);
                    }
                };
                channel.BasicConsume(queue: "serialize",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
