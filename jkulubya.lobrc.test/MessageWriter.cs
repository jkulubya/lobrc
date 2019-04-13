using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace jkulubya.lobrc.test
{
    public class MessageWriter : IMessageWriter
    {
        public async Task Write(Message message)
        {
            Console.WriteLine(message);
        }
    }
}