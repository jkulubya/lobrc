using System;

namespace jkulubya.lobrc
{
    public class Message
    {
        private OrderEvent OrderEvent { get; set; }
        private decimal EffectiveSize { get; set; }
        private LimitOrder Order { get; set; }
        private DateTimeOffset Timestamp { get; set; }
        private string OrderId { get; set; }
        public string Symbol { get; set; }
        
        public Message(string orderId, string symbol, OrderEvent orderEvent, DateTimeOffset timestamp)
        {
            OrderId = orderId;
            Symbol = symbol;
            OrderEvent = orderEvent;
            Timestamp = timestamp;
        }
        
        public void UpdateOrderPool(OrderPool orderPool)
        {
            Order = orderPool.FindLimitOrder(OrderId);
            Order?.AddSize(EffectiveSize);
        }

        public void UpdateOrderBook(OrderBook orderBook)
        {
            orderBook.AddPriceSize(Order.Price, EffectiveSize, Order.IsBid);
        }
    }
}