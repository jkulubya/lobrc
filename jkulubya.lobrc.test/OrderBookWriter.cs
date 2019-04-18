using System.Collections.Generic;
using System.Threading.Tasks;

namespace jkulubya.lobrc.test
{
    public class OrderBookWriter : IOrderBookWriter
    {
        public List<OrderBook> OrderBooks { get; } = new List<OrderBook>();

        public async Task Write(OrderBook orderBook)
        {
            OrderBooks.Add(orderBook);
        }
    }
}