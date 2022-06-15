using System;
using TellDontAsk.Repository;
using TellDontAskKata.Domain;
using TellDontAskKata.Service;

namespace TellDontAskKata.UseCase.OrderShipment
{
    public class OrderShipmentUseCase
    {
        private readonly IOrderRepository orderRepository;
        private readonly IShipmentService shipmentService;
        private readonly IDateTimeProvider dateTimeProvider;

        public OrderShipmentUseCase(IOrderRepository orderRepository, IShipmentService shipmentService,
            IDateTimeProvider dateTimeProvider)
        {
            this.orderRepository = orderRepository;
            this.shipmentService = shipmentService;
            this.dateTimeProvider = dateTimeProvider;
        }

        public Order Run(OrderShipmentRequest request)
        {
            if (dateTimeProvider.CurrentDateTime() - request.OrderCreatedAt > new TimeSpan(30, 0, 0, 0))
            {
                throw new OutdatedOrdersCannotBeShippedException();
            }

            Order order = orderRepository.GetById(request.OrderId);

            if (order.GetStatus().Equals(OrderStatus.Created) || order.GetStatus().Equals(OrderStatus.Rejected))
            {
                throw new NonApprovedOrdersCannotBeShipped();
            }

            if (order.GetStatus().Equals(OrderStatus.Shipped))
            {
                throw new OrderCannotBeShippedTwiceException();
            }

            shipmentService.Ship(order);

            var orderToStore = new Order();
            orderToStore.SetId(order.GetId());
            orderToStore.SetCurrency(order.GetCurrency());
            orderToStore.SetTax(order.GetTax());
            orderToStore.SetTotal(order.GetTotal());
            orderToStore.SetItems(order.GetItems());
            orderToStore.SetStatus(OrderStatus.Shipped);

            orderRepository.Save(orderToStore);

            return order;
        }
    }
}
