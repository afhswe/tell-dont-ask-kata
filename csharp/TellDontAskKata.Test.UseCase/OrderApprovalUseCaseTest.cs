namespace TellDonAskKataTest;

public class OrderApprovalUseCaseTest
{
    private readonly Mock<IOrderRepository> orderRepository;
    private readonly OrderApprovalUseCase useCase;

    public OrderApprovalUseCaseTest()
    {
        orderRepository = new Mock<IOrderRepository>();
        useCase = new OrderApprovalUseCase(orderRepository.Object);
    }

    [Fact]
    public void StoresApprovedOrder()
    {
        Order initialOrder = new Order();
        initialOrder.SetStatus(OrderStatus.Created);
        initialOrder.SetId(1);
        orderRepository.Setup(x => x.GetById(1)).Returns(initialOrder);

        OrderApprovalRequest request = new OrderApprovalRequest();
        request.SetOrderId(1);
        request.SetApproved(true);

        useCase.Run(request);

        orderRepository.Verify(x =>
            x.Save(It.Is<Order>(o => o.GetStatus() == OrderStatus.Approved && o.GetId() == 1)), Times.Once);
    }

    [Fact]
    public void RejectsExistingOrder()
    {
        Order initialOrder = new Order();
        initialOrder.SetStatus(OrderStatus.Created);
        initialOrder.SetId(1);
        orderRepository.Setup(x => x.GetById(1)).Returns(initialOrder);

        OrderApprovalRequest request = new OrderApprovalRequest();
        request.SetOrderId(1);
        request.SetApproved(false);

        useCase.Run(request);

        orderRepository.Verify(x =>
            x.Save(It.Is<Order>(o => o.GetStatus() == OrderStatus.Rejected && o.GetId() == 1)), Times.Once);
    }

    [Fact]
    public void CannotApproveRejectedOrder()
    {
        Order initialOrder = new Order();
        initialOrder.SetStatus(OrderStatus.Rejected);
        initialOrder.SetId(1);
        orderRepository.Setup(x => x.GetById(1)).Returns(initialOrder);

        OrderApprovalRequest request = new OrderApprovalRequest();
        request.SetOrderId(1);
        request.SetApproved(true);

        Action act = () => useCase.Run(request);
        act.Should().Throw<RejectedOrderCannotBeApprovedException>();
    }

    [Fact]
    public void CannotRejectApprovedOrder()
    {
        Order initialOrder = new Order();
        initialOrder.SetStatus(OrderStatus.Approved);
        initialOrder.SetId(1);
        orderRepository.Setup(x => x.GetById(1)).Returns(initialOrder);

        OrderApprovalRequest request = new OrderApprovalRequest();
        request.SetOrderId(1);
        request.SetApproved(false);

        Action act = () => useCase.Run(request);
        act.Should().Throw<ApprovedOrderCannotBeRejectedException>();
    }

    [Fact]
    public void ShippedOrdersCannotBeApproved()
    {
        Order initialOrder = new Order();
        initialOrder.SetStatus(OrderStatus.Shipped);
        initialOrder.SetId(1);
        orderRepository.Setup(x => x.GetById(1)).Returns(initialOrder);

        OrderApprovalRequest request = new OrderApprovalRequest();
        request.SetOrderId(1);
        request.SetApproved(true);

        Action act = () => useCase.Run(request);

        act.Should().Throw<ShippedOrdersCannotBeChangedException>();
    }

    [Fact]
    public void ShippedOrdersCannotBeRejected()
    {
        Order initialOrder = new Order();
        initialOrder.SetStatus(OrderStatus.Shipped);
        initialOrder.SetId(1);
        orderRepository.Setup(x => x.GetById(1)).Returns(initialOrder);

        OrderApprovalRequest request = new OrderApprovalRequest();
        request.SetOrderId(1);
        request.SetApproved(false);

        Action act = () => useCase.Run(request);

        act.Should().Throw<ShippedOrdersCannotBeChangedException>();
    }
}