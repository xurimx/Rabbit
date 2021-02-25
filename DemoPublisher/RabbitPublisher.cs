using DemoApp.Contracts;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DemoPublisher
{
    public class RabbitPublisher : IRabbitPublisher
    {
        private readonly IConnection connection;

        public RabbitPublisher(IConnection connection)
        {
            this.connection = connection;
        }
        public Task Publish(IMessage message, MessageType type, string? queue = "default", string? exchange = "")
        {
            using (var model = connection.CreateModel())
            {
                model.ExchangeDeclare(exchange, ExchangeType.Fanout);
                model.QueueDeclare(queue, true, false, false);

                IBasicProperties props = model.CreateBasicProperties();
                props.Headers = new Dictionary<string, object>();
                props.Headers.Add("MessageType", (int)MessageType.Message);
                props.ContentType = "application/json";

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                model.BasicPublish(exchange: exchange,
                                     routingKey: "",
                                     basicProperties: props,
                                     body: body);
            }
            return Task.CompletedTask;
        }

        public Task Publish(IMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
