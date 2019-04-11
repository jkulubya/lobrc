using System.Threading.Tasks;

namespace jkulubya.lobrc
{
    public interface IMessageReader
    {
        Task<Message> ReadMessage();
    }
}