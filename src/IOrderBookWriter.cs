using System.Threading.Tasks;
using jkulubya.lobrc;

namespace MarketDataService.Lobster
{
    public interface IOrderBookWriter
    {
        Task Write(OrderBook orderBook);
    }
}