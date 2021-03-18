using TellDontAskKata.Domain;

namespace TellDontAsk.Repository
{
    public interface IOrderRepository
    {
        void save(Order order);
        Order getById(int orderId);
    }

}
