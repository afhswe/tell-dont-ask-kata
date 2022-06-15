using TellDontAskKata.Service;
using TellDontAskKata.UseCase.OrderShipment;

namespace TellDonAskKataTest
{
    public class OrderShipmentUseCaseTest
    {
        private readonly Mock<IOrderRepository> orderRepository = new();
        private readonly Mock<IShipmentService> shipmentService = new();
        private readonly Mock<IDateTimeProvider> dateTimeProvider = new();
        private readonly OrderShipmentUseCase useCase;

        public OrderShipmentUseCaseTest()
        {
            useCase = new OrderShipmentUseCase(orderRepository.Object, shipmentService.Object, dateTimeProvider.Object);
        }

        [Fact]
        public void ShipsApprovedOrder()
        {
            Order initialOrder = new Order();
            initialOrder.SetId(1);
            initialOrder.SetStatus(OrderStatus.Approved);
            orderRepository.Setup(x => x.GetById(1)).Returns(initialOrder);

            OrderShipmentRequest request = new OrderShipmentRequest();
            request.OrderId = 1;

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
            request.OrderId = 1;

            Action act = () => useCase.Run(request);
            act.Should().Throw<NonApprovedOrdersCannotBeShipped>();
        }

        [Fact]
        public void RejectedOrdersCannotBeShipped()
        {
            Order initialOrder = new Order();
            initialOrder.SetId(1);
            initialOrder.SetStatus(OrderStatus.Rejected);
            orderRepository.Setup(x => x.GetById(1)).Returns(initialOrder);

            OrderShipmentRequest request = new OrderShipmentRequest();
            request.OrderId = 1;

            Action act = () => useCase.Run(request);
            act.Should().Throw<NonApprovedOrdersCannotBeShipped>();
        }

        [Fact]
        public void ShippedOrdersCannotBeShippedAgain()
        {
            Order initialOrder = new Order();
            initialOrder.SetId(1);
            initialOrder.SetStatus(OrderStatus.Shipped);
            orderRepository.Setup(x => x.GetById(1)).Returns(initialOrder);

            OrderShipmentRequest request = new OrderShipmentRequest();
            request.OrderId = 1;

            Action act = () => useCase.Run(request);
            act.Should().Throw<OrderCannotBeShippedTwiceException>();
        }

        [Fact]
        public void RejectsShipment_OfOrderOlderThanThirtyDays()
        {
            Order initialOrder = new Order();
            initialOrder.SetId(1);
            initialOrder.SetStatus(OrderStatus.Shipped);
            orderRepository.Setup(x => x.GetById(1)).Returns(initialOrder);
            var orderCreatedAt = new DateTime(2022, 10, 30);
            dateTimeProvider.Setup(x => x.CurrentDateTime()).Returns(orderCreatedAt);


            OrderShipmentRequest request = new OrderShipmentRequest();
            request.OrderId = 1;
            var thirtyOneDays = new TimeSpan(31, 0, 0, 0);
            request.OrderCreatedAt = orderCreatedAt.Subtract(thirtyOneDays);

            Action act = () => useCase.Run(request);
            act.Should().Throw<OutdatedOrdersCannotBeShippedException>();
        }
    }
}

