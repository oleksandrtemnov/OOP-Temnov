namespace Lab12v5
{
    public class Product
    {
        public string Title { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }

        public Product(string title, double price, int stock)
        {
            Title = title;
            Price = price;
            Stock = stock;
        }

        public override string ToString()
        {
            return $"{Title} (Ціна: ${Price}, Кількість: {Stock} шт.)";
        }
    }
}