namespace TellDontAskKata.Domain
{
    public class OrderItem
    {
        private Product product;
        private int quantity;
        private decimal taxedAmount;
        private decimal tax;

        public Product GetProduct()
        {
            return product;
        }

        public void SetProduct(Product product)
        {
            this.product = product;
        }

        public int GetQuantity()
        {
            return quantity;
        }

        public void SetQuantity(int quantity)
        {
            this.quantity = quantity;
        }

        public decimal GetTaxedAmount()
        {
            return taxedAmount;
        }

        public void SetTaxedAmount(decimal taxedAmount)
        {
            this.taxedAmount = taxedAmount;
        }

        public decimal GetTax()
        {
            return tax;
        }

        public void SetTax(decimal tax)
        {
            this.tax = tax;
        }
    }
}