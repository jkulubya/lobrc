using System;
using System.Linq;
using C5;
using Microsoft.Extensions.Logging;
using SCG = System.Collections.Generic;

namespace jkulubya.lobrc
{
    public class OrderBook
    {
        private readonly ILogger _logger;
        private TreeDictionary<decimal, decimal> Asks { get; } = new TreeDictionary<decimal, decimal>();
        private TreeDictionary<decimal, decimal> Bids { get; } =
            new TreeDictionary<decimal, decimal>(SCG.Comparer<decimal>.Create((x, y) => y.CompareTo(x)));

        public OrderBook(ILogger logger)
        {
            _logger = logger;
        }
        
        public void AddPriceSize(decimal price, decimal sizeDelta, bool isBid)
        {
            var dict = isBid ? Bids : Asks;
            if(dict.Find(ref price, out var quantity))
            {
                lock (dict)
                {
                    dict[price] = Math.Max((quantity + sizeDelta), 0M);
                }
            }
            else
            {
                lock (dict)
                {
                    dict[price] = Math.Max(sizeDelta, 0);
                }
            }
        }

        public (decimal, decimal) GetPriceVolume(int level)
        {
            var asks = Asks.Snapshot();
            var bids = Bids.Snapshot();

            var askVolume = asks.ElementAtOrDefault(level);
            var bidVolume = bids.ElementAtOrDefault(level);

            return (bidVolume.Value, askVolume.Value);
        }

        public decimal? GetAskPrice(int level)
        {
            try
            {
                SCG.IEnumerable<C5.KeyValuePair<decimal, decimal>> asks;
                lock (Asks)
                {
                    asks = Asks.Snapshot();
                    
                }

                var price = asks.ElementAt(level);

                return price.Value;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return null;
            }
        }

        public decimal? GetBidPrice(int level)
        {
            try
            {
                SCG.IEnumerable<C5.KeyValuePair<decimal, decimal>> bids;
                lock (Bids)
                {
                    bids = Bids.Snapshot();
                }

                var price = bids.ElementAt(level);

                return price.Value;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return null;
            }
        }
    }
}