using System;
using System.Collections.Generic;
using System.Text;

namespace DemoApp.Contracts
{
    public class Message : IMessage
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
