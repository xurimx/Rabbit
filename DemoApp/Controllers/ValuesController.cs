using DemoApp.Contracts;
using DemoPublisher;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IRabbitPublisher publisher;

        public ValuesController(IRabbitPublisher publisher)
        {
            this.publisher = publisher;
        }

        [HttpGet("{msg}")]
        public async Task<string> GetAsync(string msg = "Hello World")
        {
            await publisher.Publish(new Message { Title = "Hello", Description = msg }, MessageType.Message, "hello", "get");

            return "ok";
        }

        [HttpPost]
        public async Task<string> PostAsync(Message message)
        {
            await publisher.Publish(message, MessageType.Message, "serialize", "default");

            return "ok";
        }

        [HttpPost("worker")]
        public async Task<string> Worker(Message message)
        {
            await publisher.Publish(message, MessageType.Message, "worker", "mediator");

            return "ok";
        }

    }
}
