using System;
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
            try
            {
                return Pool[orderId];
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return null;
            }
        }

        public void AddLimitOrder(LimitOrder order)
        {
            try
            {
                Pool.Add(order.Id, order);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
        }

        public void DeleteLimitOrder(string orderId)
        {
            try
            {
                Pool.Remove(orderId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
        }
    }
}
