using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jkulubya.lobrc.test
{
    public class TestReader : IMessageReader
    {
        private readonly BlockingCollection<Message> _messages = new BlockingCollection<Message>();
        
        public TestReader(List<Message> messages)
        {
            foreach (var message in messages)
            {
                _messages.Add(message);
            }
        }
        
        public async Task<Message> ReadMessage()
        {
            var message = _messages.Take();
            return message;
        }
    }
}