using FluentAssertions;
using System;
using TellDontAskKata.Domain;
using TellDontAskKata.Test.Doubles;
using TellDontAskKata.UseCase;
using Xunit;

namespace TellDonAskKataTest
{
    public class OrderApprovalUseCaseTest
    {
        private readonly TestOrderRepository orderRepository;
        private readonly OrderApprovalUseCase useCase;

        public OrderApprovalUseCaseTest()
        {
            orderRepository = new TestOrderRepository();
            useCase = new OrderApprovalUseCase(orderRepository);
        }

        [Fact]
        public void ApprovedExistingOrder()
        {
            Order initialOrder = new Order();
            initialOrder.SetStatus(OrderStatus.Created);
            initialOrder.SetId(1);
            orderRepository.AddOrder(initialOrder);

            OrderApprovalRequest request = new OrderApprovalRequest();
            request.SetOrderId(1);
            request.SetApproved(true);

            useCase.Run(request);

            Order savedOrder = orderRepository.GetSavedOrder();
            savedOrder.GetStatus().Should().Be(OrderStatus.Approved);
        }

        [Fact]
        public void RejectedExistingOrder()
        {
            Order initialOrder = new Order();
            initialOrder.SetStatus(OrderStatus.Created);
            initialOrder.SetId(1);
            orderRepository.AddOrder(initialOrder);

            OrderApprovalRequest request = new OrderApprovalRequest();
            request.SetOrderId(1);
            request.SetApproved(false);

            useCase.Run(request);

            Order savedOrder = orderRepository.GetSavedOrder();
            savedOrder.GetStatus().Should().Be(OrderStatus.Rejected);
        }

        [Fact]
        public void CannotApproveRejectedOrder()
        {
            Order initialOrder = new Order();
            initialOrder.SetStatus(OrderStatus.Rejected);
            initialOrder.SetId(1);
            orderRepository.AddOrder(initialOrder);

            OrderApprovalRequest request = new OrderApprovalRequest();
            request.SetOrderId(1);
            request.SetApproved(true);

            Action act = () => useCase.Run(request);
            act.Should().Throw<RejectedOrderCannotBeApprovedException>();

            orderRepository.GetSavedOrder().Should().BeNull();
        }

        [Fact]
        public void CannotRejectApprovedOrder()
        {
            Order initialOrder = new Order();
            initialOrder.SetStatus(OrderStatus.Approved);
            initialOrder.SetId(1);
            orderRepository.AddOrder(initialOrder);

            OrderApprovalRequest request = new OrderApprovalRequest();
            request.SetOrderId(1);
            request.SetApproved(false);

            Action act = () => useCase.Run(request);
            act.Should().Throw<ApprovedOrderCannotBeRejectedException>();

            orderRepository.GetSavedOrder().Should().BeNull();
        }

        [Fact]
        public void ShippedOrdersCannotBeApproved()
        {
            Order initialOrder = new Order();
            initialOrder.SetStatus(OrderStatus.Shipped);
            initialOrder.SetId(1);
            orderRepository.AddOrder(initialOrder);

            OrderApprovalRequest request = new OrderApprovalRequest();
            request.SetOrderId(1);
            request.SetApproved(true);

            Action act = () => useCase.Run(request);

            act.Should().Throw<ShippedOrdersCannotBeChangedException>();
            orderRepository.GetSavedOrder().Should().BeNull();
        }

        [Fact]
        public void ShippedOrdersCannotBeRejected()
        {
            Order initialOrder = new Order();
            initialOrder.SetStatus(OrderStatus.Shipped);
            initialOrder.SetId(1);
            orderRepository.AddOrder(initialOrder);

            OrderApprovalRequest request = new OrderApprovalRequest();
            request.SetOrderId(1);
            request.SetApproved(false);

            Action act = () => useCase.Run(request);

            act.Should().Throw<ShippedOrdersCannotBeChangedException>();
            orderRepository.GetSavedOrder().Should().BeNull();
        }
    }
}

