using TellDontAskKata.Domain;
using TellDontAskKata.Service;

namespace TellDontAskKata.Test.Doubles
{
    public class TestShipmentService : IShipmentService
    {
        private Order shippedOrder = null;

        public Order getShippedOrder()
        {
            return shippedOrder;
        }


        public void ship(Order order)
        {
            this.shippedOrder = order;
        }
    }
}
