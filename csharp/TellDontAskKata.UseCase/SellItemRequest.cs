namespace TellDontAskKata.UseCase
{
    public class SellItemRequest
    {
        private int quantity;
        private string productName;

        public void SetQuantity(int quantity)
        {
            this.quantity = quantity;
        }

        public void SetProductName(string productName)
        {
            this.productName = productName;
        }

        public int GetQuantity()
        {
            return quantity;
        }

        public string GetProductName()
        {
            return productName;
        }
    }
}