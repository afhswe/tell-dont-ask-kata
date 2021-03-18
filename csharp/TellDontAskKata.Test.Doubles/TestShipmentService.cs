using TellDontAskKata.Domain;
using TellDontAskKata.Service;

namespace TellDontAskKata.Test.Doubles
{
    public class TestShipmentService : IShipmentService
    {
        private Order shippedOrder = null;

        public Order GetShippedOrder()
        {
            return shippedOrder;
        }


        public void Ship(Order order)
        {
            this.shippedOrder = order;
        }
    }
}
