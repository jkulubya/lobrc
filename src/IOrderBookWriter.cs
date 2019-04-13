using System.Threading.Tasks;

namespace jkulubya.lobrc
{
    public interface IOrderBookWriter
    {
        Task Write(OrderBook orderBook);
    }
}