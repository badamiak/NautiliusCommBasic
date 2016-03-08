using NautiliusCommBasic.Api.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NautiliusCommBasic.Api.Model;

namespace NautiliusCommBasic
{
    class NCommService : INCommService
    {
        public void SendMessage(Message message)
        {
            Console.WriteLine("===");
            Console.WriteLine(string.Format("{0}\tIN: FROM: {1}: {2}", message.Created,message.Login, message.Text));
            Console.WriteLine("===");
        }

        public void TestConnection()
        {
            return;
        }
    }
}
