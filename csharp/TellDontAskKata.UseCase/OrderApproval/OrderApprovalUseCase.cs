using TellDontAsk.Repository;
using TellDontAskKata.Domain;

namespace TellDontAskKata.UseCase.OrderApproval
{
    public class OrderApprovalUseCase
    {
        private readonly IOrderRepository orderRepository;

        public OrderApprovalUseCase(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        public Order Run(OrderApprovalRequest request)
        {
            Order order = orderRepository.GetById(request.GetOrderId());

            if (order.GetStatus().Equals(OrderStatus.Shipped))
            {
                throw new ShippedOrdersCannotBeChangedException();
            }

            if (request.IsApproved() && order.GetStatus().Equals(OrderStatus.Rejected))
            {
                throw new RejectedOrderCannotBeApprovedException();
            }

            if (!request.IsApproved() && order.GetStatus().Equals(OrderStatus.Approved))
            {
                throw new ApprovedOrderCannotBeRejectedException();
            }

            order.SetStatus(request.IsApproved() ? OrderStatus.Approved : OrderStatus.Rejected);
            orderRepository.Save(order);

            return order;
        }
    }
}
