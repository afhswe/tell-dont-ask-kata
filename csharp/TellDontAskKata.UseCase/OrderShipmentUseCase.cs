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

        public void run(OrderShipmentRequest request)
        {
            Order order = orderRepository.getById(request.getOrderId());

            if (order.getStatus().Equals(OrderStatus.CREATED) || order.getStatus().Equals(OrderStatus.REJECTED))
            {
                throw new OrderCannotBeShippedException();
            }

            if (order.getStatus().Equals(OrderStatus.SHIPPED))
            {
                throw new OrderCannotBeShippedTwiceException();
            }

            shipmentService.ship(order);

            order.setStatus(OrderStatus.SHIPPED);
            orderRepository.save(order);
        }
    }
}
