using System.Collections.Generic;
using System.Linq;
using TellDontAsk.Repository;
using TellDontAskKata.Domain;

namespace TellDontAskKata.Test.Doubles
{
    public class TestOrderRepository : IOrderRepository
    {
        private Order insertedOrder;
        private List<Order> orders = new List<Order>();

        public Order GetSavedOrder()
        {
            return insertedOrder;
        }

        public void Save(Order order)
        {
            insertedOrder = order;
        }

        public Order GetById(int orderId)
        {
            return orders.Where(o => o.GetId() == orderId).First();
        }

        public void AddOrder(Order order)
        {
            orders.Add(order);
        }
    }
}
