using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoApp.Contracts
{
    public static class MessageFactory
    {
        public static IMessage Create(string json, MessageType type)
        {
            switch (type)
            {
                case MessageType.Message:
                    return JsonConvert.DeserializeObject<Message>(json);
                case MessageType.TestEvent:
                    return JsonConvert.DeserializeObject<TestEvent>(json);
                default:
                    return null;
            }
        }
    }
}