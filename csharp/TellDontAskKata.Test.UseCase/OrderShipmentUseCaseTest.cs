namespace TellDonAskKataTest
{
    public class OrderShipmentUseCaseTest
    {
        private readonly TestOrderRepository orderRepository;
        private readonly TestShipmentService shipmentService;
        private readonly OrderShipmentUseCase useCase;

        public OrderShipmentUseCaseTest()
        {
            orderRepository = new TestOrderRepository();
            shipmentService = new TestShipmentService();
            useCase = new OrderShipmentUseCase(orderRepository, shipmentService);
        }

        [Fact]
        public void ShipApprovedOrder()
        {
            Order initialOrder = new Order();
            initialOrder.SetId(1);
            initialOrder.SetStatus(OrderStatus.Approved);
            orderRepository.AddOrder(initialOrder);

            OrderShipmentRequest request = new OrderShipmentRequest();
            request.SetOrderId(1);

            useCase.Run(request);

            orderRepository.GetSavedOrder().GetStatus().Should().Be(OrderStatus.Shipped);
            shipmentService.GetShippedOrder().Should().Be(initialOrder);
        }

        [Fact]
        public void CreatedOrdersCannotBeShipped()
        {
            Order initialOrder = new Order();
            initialOrder.SetId(1);
            initialOrder.SetStatus(OrderStatus.Created);
            orderRepository.AddOrder(initialOrder);

            OrderShipmentRequest request = new OrderShipmentRequest();
            request.SetOrderId(1);

            Action act = () => useCase.Run(request);
            act.Should().Throw<OrderCannotBeShippedException>();

            orderRepository.GetSavedOrder().Should().BeNull();
            shipmentService.GetShippedOrder().Should().BeNull();
        }

        [Fact]
        public void RejectedOrdersCannotBeShipped()
        {
            Order initialOrder = new Order();
            initialOrder.SetId(1);
            initialOrder.SetStatus(OrderStatus.Rejected);
            orderRepository.AddOrder(initialOrder);

            OrderShipmentRequest request = new OrderShipmentRequest();
            request.SetOrderId(1);

            Action act = () => useCase.Run(request);
            act.Should().Throw<OrderCannotBeShippedException>();

            orderRepository.GetSavedOrder().Should().BeNull();
            shipmentService.GetShippedOrder().Should().BeNull();
        }

        [Fact]
        public void ShippedOrdersCannotBeShippedAgain()
        {
            Order initialOrder = new Order();
            initialOrder.SetId(1);
            initialOrder.SetStatus(OrderStatus.Shipped);
            orderRepository.AddOrder(initialOrder);

            OrderShipmentRequest request = new OrderShipmentRequest();
            request.SetOrderId(1);

            Action act = () => useCase.Run(request);
            act.Should().Throw<OrderCannotBeShippedTwiceException>();

            orderRepository.GetSavedOrder().Should().BeNull();
            shipmentService.GetShippedOrder().Should().BeNull();
        }
    }
}

