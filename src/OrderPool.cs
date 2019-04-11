using C5;
using Microsoft.Extensions.Logging;

namespace jkulubya.lobrc
{
    public class OrderPool
    {
        private readonly ILogger _logger;
        private HashDictionary<string, LimitOrder> Pool { get; } = new HashDictionary<string, LimitOrder>();

        public OrderPool(ILogger logger)
        {
            _logger = logger;
        }
        
        public LimitOrder FindLimitOrder(string orderId)
        {
            return Pool[orderId];
        }

        public void AddLimitOrder(LimitOrder order)
        {
            Pool.Add(order.Id, order);
        }

        public void DeleteLimitOrder(string orderId)
        {
            Pool.Remove(orderId);
        }
    }
}
