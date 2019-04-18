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
        private readonly TreeDictionary<decimal, decimal> _asks = new TreeDictionary<decimal, decimal>();
        private readonly TreeDictionary<decimal, decimal> _bids  =
            new TreeDictionary<decimal, decimal>(SCG.Comparer<decimal>.Create((x, y) => y.CompareTo(x)));

        public OrderBook(ILogger logger)
        {
            _logger = logger;
        }
        
        internal void AddPriceSize(decimal price, decimal sizeDelta, bool isBid)
        {
            var dict = isBid ? _bids : _asks;
            if(dict.Find(ref price, out var quantity))
            {
                lock (dict)
                {
                    quantity = Math.Max((quantity + sizeDelta), 0M);
                    if (quantity == decimal.Zero)
                    {
                        dict.Remove(price);
                    }
                    else
                    {
                        dict[price] = quantity;
                    }
                }
            }
            else
            {
                if(sizeDelta <= 0) return;
                
                lock (dict)
                {
                    dict[price] = sizeDelta;
                }
            }
        }

        public (decimal, decimal) GetPriceVolume(int level)
        {
            SCG.IEnumerable<KeyValuePair<decimal, decimal>> asks;
            SCG.IEnumerable<KeyValuePair<decimal, decimal>> bids;
            
            lock (_asks)
            {
                asks = _asks.Snapshot();
            }
            lock (_bids)
            {
                bids = _bids.Snapshot();
            }

            var askVolume = asks.ElementAtOrDefault(level);
            var bidVolume = bids.ElementAtOrDefault(level);

            return (bidVolume.Value, askVolume.Value);
        }

        public decimal? GetAskPrice(int level)
        {
            try
            {
                SCG.IEnumerable<C5.KeyValuePair<decimal, decimal>> asks;
                lock (_asks)
                {
                    asks = _asks.Snapshot();
                    
                }

                var price = asks.ElementAt(level);

                return price.Key;
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
                SCG.IEnumerable<KeyValuePair<decimal, decimal>> bids;
                lock (_bids)
                {
                    bids = _bids.Snapshot();
                }

                var price = bids.ElementAt(level);

                return price.Key;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return null;
            }
        }

        public SCG.IEnumerable<OrderBookLevel> Bids
        {
            get
            {
                SCG.IEnumerable<KeyValuePair<decimal, decimal>> bids;
                lock (_bids)
                {
                    bids = _bids.Snapshot();
                }

                return bids.Select(b => new OrderBookLevel(b.Key, b.Value));
            }
        }
        
        public SCG.IEnumerable<OrderBookLevel> Asks
        {
            get
            {
                SCG.IEnumerable<KeyValuePair<decimal, decimal>> asks;
                lock (_asks)
                {
                    asks = _asks.Snapshot();
                }

                return asks.Select(b => new OrderBookLevel(b.Key, b.Value));
            }
        }
    }
}