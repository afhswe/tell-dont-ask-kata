using System;
using TellDontAskKata.Domain;

namespace TellDontAsk.Repository
{
    public interface IOrderRepository
    {
        void Save(Order order);
        Order GetById(int orderId);
        int NextId();
    }

}
