namespace jkulubya.lobrc.test
{
    public class MessageReaderInput
    {
        public MessageReaderInputType Type { get; }
        public decimal Quantity { get; }
        public decimal Price { get; }
        public string Symbol { get; }
    }

    public enum MessageReaderInputType
    {
        NewOrder,
        Trade,
        Cancel,
        TradeBust
    }
}