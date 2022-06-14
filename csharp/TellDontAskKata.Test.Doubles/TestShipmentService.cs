using System;
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

        public void Verify(Order order)
        {
            if (order.GetId() != shippedOrder.GetId())
            {
                throw new Exception("The expected value was not received");
            }
        }
    }
}
