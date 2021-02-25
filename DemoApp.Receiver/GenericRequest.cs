using DemoApp.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApp.Receiver
{
    public class GenericRequest<T> : IRequest<Unit> where T : IMessage
    {
        public GenericRequest(T input) 
        {
            Command = input;
        }
                                       
        public T Command { get; set; }
    }


}
