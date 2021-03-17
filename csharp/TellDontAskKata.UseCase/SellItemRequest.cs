namespace TellDontAskKata.UseCase
{
    public class SellItemRequest
    {
        private int quantity;
        private string productName;

        public void setQuantity(int quantity)
        {
            this.quantity = quantity;
        }

        public void setProductName(string productName)
        {
            this.productName = productName;
        }

        public int getQuantity()
        {
            return quantity;
        }

        public string getProductName()
        {
            return productName;
        }
    }
}