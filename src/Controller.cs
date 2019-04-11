using System.Threading;
using System.Threading.Tasks;
using C5;
using MarketDataService.Lobster;
using Microsoft.Extensions.Logging;
using SCG = System.Collections.Generic;

namespace jkulubya.lobrc
{
    public class Controller
    {
        private CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();
        private readonly IMessageReader _messageReader;
        private readonly SCG.IEnumerable<IMessageWriter> _messageWriters;
        private readonly ILogger _logger;
        private readonly IOrderBookWriter _orderBookWriter;
        private readonly OrderPool _orderPool;
        private readonly TreeDictionary<string, OrderBook> _orderBooks = new TreeDictionary<string, OrderBook>();
        
        private Task ControllerTask { get; set; }
        
        public Controller(IMessageReader messageReader, SCG.IEnumerable<IMessageWriter> messageWriters, IOrderBookWriter orderBookWriter, ILogger logger)
        {
            _messageReader = messageReader;
            _messageWriters = messageWriters;
            _orderBookWriter = orderBookWriter;
            _logger = logger;
            _orderPool = new OrderPool(logger);
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

                var orderBook = GetOrderBook(message);
                message.UpdateOrderBook(orderBook);
                
                await _orderBookWriter.Write(orderBook);

                foreach (var messageWriter in _messageWriters)
                {
                    await messageWriter.Write(message);
                }
            }
        }

        private OrderBook GetOrderBook(Message message)
        {
            var symbol = message.Symbol;
            if (_orderBooks.Find(ref symbol, out var orderbook))
            {
                return orderbook;
            }
            
            orderbook = new OrderBook(_logger);
            _orderBooks.Add(symbol, orderbook);

            return _orderBooks[symbol];
        }

        public void Stop()
        {
            CancellationTokenSource.Cancel();
        }
    }
}