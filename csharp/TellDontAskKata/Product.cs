namespace TellDontAskKata.Domain
{
    public class Product
    {
        private string name;
        private decimal price;
        private Category category;

        public string GetName()
        {
            return name;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public decimal GetPrice()
        {
            return price;
        }

        public void SetPrice(decimal price)
        {
            this.price = price;
        }

        public Category GetCategory()
        {
            return category;
        }

        public void SetCategory(Category category)
        {
            this.category = category;
        }
    }
}