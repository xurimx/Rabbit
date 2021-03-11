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

        public Task Publish(IMessage message)
        {
            using (var model = connection.CreateModel())
            {
                var exchange = message.GetType().Name;

                model.ExchangeDeclare(exchange, ExchangeType.Fanout, true);

                IBasicProperties props = model.CreateBasicProperties();
                
                props.Headers = new Dictionary<string, object>();
                props.Headers.Add("type", message.GetType().Name);
                props.ContentType = "application/json";

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                model.BasicPublish(exchange: exchange,
                     routingKey: "",
                     basicProperties: props,
                     body: body);
            }
            return Task.CompletedTask;
        }

        public Task Publish(IMessage message, MessageType type, string queue = "", string exchange = "", string routingKey = "")
        {
            throw new NotImplementedException();
        }
    }
}
