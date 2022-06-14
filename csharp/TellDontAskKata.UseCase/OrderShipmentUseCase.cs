using TellDontAsk.Repository;
using TellDontAskKata.Domain;
using TellDontAskKata.Service;

namespace TellDontAskKata.UseCase
{
    public class OrderShipmentUseCase
    {
        private readonly IOrderRepository orderRepository;
        private readonly IShipmentService shipmentService;

        public OrderShipmentUseCase(IOrderRepository orderRepository, IShipmentService shipmentService)
        {
            this.orderRepository = orderRepository;
            this.shipmentService = shipmentService;
        }

        public Order Run(OrderShipmentRequest request)
        {
            Order order = orderRepository.GetById(request.GetOrderId());

            if (order.GetStatus().Equals(OrderStatus.Created) || order.GetStatus().Equals(OrderStatus.Rejected))
            {
                throw new OrderCannotBeShippedException();
            }

            if (order.GetStatus().Equals(OrderStatus.Shipped))
            {
                throw new OrderCannotBeShippedTwiceException();
            }

            shipmentService.Ship(order);

            order.SetStatus(OrderStatus.Shipped);
            orderRepository.Save(order);

            return order;
        }
    }
}
