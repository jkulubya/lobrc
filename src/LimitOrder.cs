using System;

namespace jkulubya.lobrc
{
    public class LimitOrder
    {
        public LimitOrder(string id, string symbol = null, bool isBid = true, decimal price = 0, decimal size = 0,
            string marketParticipantId = null)
        {
            Id = id;
            Symbol = symbol;
            IsBid = isBid;
            Price = price;
            Size = size;
            MarketParticipantId = marketParticipantId;
            RemainingSize = size;
        }

        public string Id { get; }
        public string Symbol { get; }
        public decimal Size { get; }
        public decimal Price { get; }
        private string MarketParticipantId { get; }
        public decimal RemainingSize { get; private set; }
        public bool IsBid { get; }

        public void AddSize(decimal sizeDelta)
        {
            lock (this)
            {
                RemainingSize = Math.Max(RemainingSize + sizeDelta, 0);
            }
        }
    }
}