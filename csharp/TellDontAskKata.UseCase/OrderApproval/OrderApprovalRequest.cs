namespace TellDontAskKata.UseCase.OrderApproval
{
    public class OrderApprovalRequest
    {
        private int orderId;
        private bool approved;

        public void SetOrderId(int orderId)
        {
            this.orderId = orderId;
        }

        public int GetOrderId()
        {
            return orderId;
        }

        public void SetApproved(bool approved)
        {
            this.approved = approved;
        }

        public bool IsApproved()
        {
            return approved;
        }
    }

}
