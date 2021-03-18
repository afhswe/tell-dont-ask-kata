namespace TellDontAskKata.UseCase
{
    public class OrderShipmentRequest
    {
        private int orderId;

        public void SetOrderId(int orderId)
        {
            this.orderId = orderId;
        }

        public int GetOrderId()
        {
            return orderId;
        }
    }
}
