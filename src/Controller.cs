using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SCG = System.Collections.Generic;

namespace jkulubya.lobrc
{
    public class Controller
    {
        private CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();
        private readonly IMessageReader _messageReader;
        private readonly SCG.IEnumerable<IMessageWriter> _messageWriters;
        private readonly IOrderBookWriter _orderBookWriter;
        private readonly OrderPool _orderPool;
        private readonly OrderBook _orderBook;
        
        private Task ControllerTask { get; set; }
        
        public Controller(IMessageReader messageReader, SCG.IEnumerable<IMessageWriter> messageWriters, IOrderBookWriter orderBookWriter, ILogger logger)
        {
            _messageReader = messageReader;
            _messageWriters = messageWriters;
            _orderBookWriter = orderBookWriter;
            _orderPool = new OrderPool(logger);
            _orderBook = new OrderBook(logger);
        }

        public void Start()
        {
            ControllerTask = Task.Run(() => Run(CancellationTokenSource.Token));
        }

        private async Task Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = await _messageReader.ReadMessage();

                message.UpdateOrderPool(_orderPool);
                
                message.UpdateOrderBook(_orderBook);
                
                await _orderBookWriter.Write(_orderBook);

                foreach (var messageWriter in _messageWriters)
                {
                    await messageWriter.Write(message);
                }
            }
        }

        public void Stop()
        {
            CancellationTokenSource.Cancel();
        }
    }
}