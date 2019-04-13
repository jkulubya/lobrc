namespace jkulubya.lobrc
{
    public class OrderBookLevel
    {
        public OrderBookLevel(decimal price, decimal quantity)
        {
            Price = price;
            Quantity = quantity;
        }
        
        public decimal Price { get; }
        public decimal Quantity { get; }
    }
}