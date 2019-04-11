using System;

namespace jkulubya.lobrc
{
    public class LimitOrder
    {
        public string Id { get; }
        private decimal Size { get; }
        public decimal Price { get; set; }
        private string MarketParticipantId { get; set; }
        public decimal RemainingSize { get; private set; }
        public bool IsBid { get; set; }

        public LimitOrder()
        {
            
        }

        public void AddSize(decimal sizeDelta)
        {
            lock (this)
            {
                RemainingSize = Math.Max((RemainingSize + sizeDelta), 0);
            }
        }
    }
}