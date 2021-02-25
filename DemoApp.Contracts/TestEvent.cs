using System;
using System.Collections.Generic;
using System.Text;

namespace DemoApp.Contracts
{
    public class TestEvent : IMessage
    {
        public string EventId { get; set; }
        public string EventName { get; set; }
        public bool EventStatus { get; set; }
        public int Events { get; set; }
    }
}
