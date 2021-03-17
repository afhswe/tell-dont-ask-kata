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

        public Order getSavedOrder()
        {
            return insertedOrder;
        }

        public void save(Order order)
        {
            insertedOrder = order;
        }

        public Order getById(int orderId)
        {
            return orders.Where(o => o.getId() == orderId).First();
            //return orders.stream().filter(o->o.getId() == orderId).findFirst().get();
        }

        public void addOrder(Order order)
        {
            orders.Add(order);
        }
    }
}
