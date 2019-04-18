using System;

namespace jkulubya.lobrc
{
    public class Message
    {
        public Message(string orderId, MessageType messageType, LimitOrder order, DateTimeOffset timestamp,
            decimal incomingSize)
        {
            OrderId = orderId;
            MessageType = messageType;
            Order = order;
            Timestamp = timestamp;
            IncomingSize = incomingSize;
        }

        private string OrderId { get; }
        private MessageType MessageType { get; }

        private decimal EffectiveSize
        {
            get
            {
                switch (MessageType)
                {
                    case MessageType.Submission:
                        return Order.RemainingSize;
                    case MessageType.Execution:
                        return -1 * IncomingSize;
                    case MessageType.Deletion:
                        return -1 * IncomingSize;
                    default:
                        return decimal.Zero;
                }
            }
        }

        private LimitOrder Order { get; set; }
        private DateTimeOffset Timestamp { get; }
        private decimal IncomingSize { get; set; }

        public static Message New(LimitOrder order, DateTimeOffset timestamp)
        {
            return new Message(order.Id, MessageType.Submission, order, timestamp, default);
        }

        public static Message Delete(string orderId, DateTimeOffset timestamp)
        {
            return new Message(orderId, MessageType.Deletion, default, timestamp, default);
        }

        public static Message Execution(string orderId, decimal executedQuantity, DateTimeOffset timestamp)
        {
            return new Message(orderId, MessageType.Execution, default, timestamp, executedQuantity);
        }

        internal void UpdateOrderPool(OrderPool orderPool)
        {
            if (MessageType == MessageType.Submission)
            {
                orderPool.AddLimitOrder(Order);
            }
            else if (MessageType == MessageType.Deletion)
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