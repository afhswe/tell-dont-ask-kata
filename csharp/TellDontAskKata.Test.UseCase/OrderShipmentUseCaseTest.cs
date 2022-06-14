using TellDontAskKata.Service;

namespace TellDonAskKataTest
{
    public class OrderShipmentUseCaseTest
    {
        private readonly Mock<IOrderRepository> orderRepository = new();
        private readonly Mock<IShipmentService> shipmentService = new();
        private readonly OrderShipmentUseCase useCase;

        public OrderShipmentUseCaseTest()
        {
            useCase = new OrderShipmentUseCase(orderRepository.Object, shipmentService.Object);
        }

        [Fact]
        public void ShipsApprovedOrder()
        {
            Order initialOrder = new Order();
            initialOrder.SetId(1);
            initialOrder.SetStatus(OrderStatus.Approved);
            orderRepository.Setup(x => x.GetById(1)).Returns(initialOrder);

            OrderShipmentRequest request = new OrderShipmentRequest();
            request.SetOrderId(1);

            useCase.Run(request);

            shipmentService.Verify(x => 
                x.Ship(It.Is<Order>(o => o.GetId() == 1 && o.GetStatus() == OrderStatus.Approved)), Times.Once);
        }

        [Fact]
        public void CreatedOrdersCannotBeShipped()
        {
            Order initialOrder = new Order();
            initialOrder.SetId(1);
            initialOrder.SetStatus(OrderStatus.Created);
            orderRepository.Setup(x => x.GetById(1)).Returns(initialOrder);

            OrderShipmentRequest request = new OrderShipmentRequest();
            request.SetOrderId(1);

            Action act = () => useCase.Run(request);
            act.Should().Throw<OrderCannotBeShippedException>();
        }

        [Fact]
        public void RejectedOrdersCannotBeShipped()
        {
            Order initialOrder = new Order();
            initialOrder.SetId(1);
            initialOrder.SetStatus(OrderStatus.Rejected);
            orderRepository.Setup(x => x.GetById(1)).Returns(initialOrder);

            OrderShipmentRequest request = new OrderShipmentRequest();
            request.SetOrderId(1);

            Action act = () => useCase.Run(request);
            act.Should().Throw<OrderCannotBeShippedException>();
        }

        [Fact]
        public void ShippedOrdersCannotBeShippedAgain()
        {
            Order initialOrder = new Order();
            initialOrder.SetId(1);
            initialOrder.SetStatus(OrderStatus.Shipped);
            orderRepository.Setup(x => x.GetById(1)).Returns(initialOrder);

            OrderShipmentRequest request = new OrderShipmentRequest();
            request.SetOrderId(1);

            Action act = () => useCase.Run(request);
            act.Should().Throw<OrderCannotBeShippedTwiceException>();
        }
    }
}

