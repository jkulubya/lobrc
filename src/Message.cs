using System;

namespace jkulubya.lobrc
{
    public class Message
    {
        private string OrderId { get; }
        private OrderEvent OrderEvent { get; set; }
        private decimal EffectiveSize
        {
            get
            {
                switch (OrderEvent)
                {
                    case OrderEvent.Submission:
                        return Order.RemainingSize;
                    case OrderEvent.Execution:
                        return -1 * IncomingSize;
                    case OrderEvent.Deletion:
                        return -1 * IncomingSize;
                    default:
                        return decimal.Zero;
                }
            }
        }
        private LimitOrder Order { get; set; }
        private DateTimeOffset Timestamp { get; }
        public string Symbol => Order.Symbol;
        private decimal IncomingSize { get; set; }

        private Message(string orderId, OrderEvent orderEvent, LimitOrder order, DateTimeOffset timestamp,
            decimal incomingSize)
        {
            OrderId = orderId;
            OrderEvent = orderEvent;
            Order = order;
            Timestamp = timestamp;
            IncomingSize = incomingSize;
        }

        public static Message New(LimitOrder order, DateTimeOffset timestamp)
        {
            return new Message(order.Id, OrderEvent.Submission, order, timestamp, default);
        }
        
        public static Message Delete(string orderId, DateTimeOffset timestamp)
        {
            return new Message(orderId, OrderEvent.Deletion, default, timestamp, default);
        }

        public static Message Execution(string orderId, decimal executedQuantity, DateTimeOffset timestamp)
        {
            return new Message(orderId, OrderEvent.Execution, default, timestamp, executedQuantity);
        }
        
        internal void UpdateOrderPool(OrderPool orderPool)
        {
            if (OrderEvent == OrderEvent.Submission)
            {
                orderPool.AddLimitOrder(Order);
            }
            else if (OrderEvent == OrderEvent.Deletion)
            {
                Order = orderPool.FindLimitOrder(OrderId);
                IncomingSize = Order?.RemainingSize ?? 0;
                orderPool.DeleteLimitOrder(OrderId);
            }
            else
            {
                Order = orderPool.FindLimitOrder(OrderId);
                Order?.AddSize(EffectiveSize);
            }
        }

        internal void UpdateOrderBook(OrderBook orderBook)
        {
            orderBook.AddPriceSize(Order.Price, EffectiveSize, Order.IsBid);
        }

        public override string ToString()
        {
            return $"Id - {Order.Id}";
        }
    }
}