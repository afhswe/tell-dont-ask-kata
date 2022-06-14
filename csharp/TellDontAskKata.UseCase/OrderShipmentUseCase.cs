using System.Collections.Generic;
using System.Collections.ObjectModel;
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
