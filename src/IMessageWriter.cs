using System.Threading.Tasks;

namespace jkulubya.lobrc
{
    public interface IMessageWriter
    {
        Task Write(Message message);
    }
}