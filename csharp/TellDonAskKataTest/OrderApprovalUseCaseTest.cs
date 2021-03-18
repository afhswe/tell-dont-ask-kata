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
        public void approvedExistingOrder()
        {
            Order initialOrder = new Order();
            initialOrder.setStatus(OrderStatus.CREATED);
            initialOrder.setId(1);
            orderRepository.addOrder(initialOrder);

            OrderApprovalRequest request = new OrderApprovalRequest();
            request.setOrderId(1);
            request.setApproved(true);

            useCase.run(request);

            Order savedOrder = orderRepository.getSavedOrder();
            savedOrder.getStatus().Should().Be(OrderStatus.APPROVED);
        }

        [Fact]
        public void rejectedExistingOrder()
        {
            Order initialOrder = new Order();
            initialOrder.setStatus(OrderStatus.CREATED);
            initialOrder.setId(1);
            orderRepository.addOrder(initialOrder);

            OrderApprovalRequest request = new OrderApprovalRequest();
            request.setOrderId(1);
            request.setApproved(false);

            useCase.run(request);

            Order savedOrder = orderRepository.getSavedOrder();
            savedOrder.getStatus().Should().Be(OrderStatus.REJECTED);
        }

        [Fact]
        public void cannotApproveRejectedOrder()
        {
            Order initialOrder = new Order();
            initialOrder.setStatus(OrderStatus.REJECTED);
            initialOrder.setId(1);
            orderRepository.addOrder(initialOrder);

            OrderApprovalRequest request = new OrderApprovalRequest();
            request.setOrderId(1);
            request.setApproved(true);

            Action act = () => useCase.run(request);
            act.Should().Throw<RejectedOrderCannotBeApprovedException>();

            orderRepository.getSavedOrder().Should().BeNull();
        }

        [Fact]
        public void cannotRejectApprovedOrder()
        {
            Order initialOrder = new Order();
            initialOrder.setStatus(OrderStatus.APPROVED);
            initialOrder.setId(1);
            orderRepository.addOrder(initialOrder);

            OrderApprovalRequest request = new OrderApprovalRequest();
            request.setOrderId(1);
            request.setApproved(false);

            Action act = () => useCase.run(request);
            act.Should().Throw<ApprovedOrderCannotBeRejectedException>();

            orderRepository.getSavedOrder().Should().BeNull();
        }

        [Fact]
        public void shippedOrdersCannotBeApproved()
        {
            Order initialOrder = new Order();
            initialOrder.setStatus(OrderStatus.SHIPPED);
            initialOrder.setId(1);
            orderRepository.addOrder(initialOrder);

            OrderApprovalRequest request = new OrderApprovalRequest();
            request.setOrderId(1);
            request.setApproved(true);

            Action act = () => useCase.run(request);

            act.Should().Throw<ShippedOrdersCannotBeChangedException>();
            orderRepository.getSavedOrder().Should().BeNull();
        }

        [Fact]
        public void shippedOrdersCannotBeRejected()
        {
            Order initialOrder = new Order();
            initialOrder.setStatus(OrderStatus.SHIPPED);
            initialOrder.setId(1);
            orderRepository.addOrder(initialOrder);

            OrderApprovalRequest request = new OrderApprovalRequest();
            request.setOrderId(1);
            request.setApproved(false);

            Action act = () => useCase.run(request);

            act.Should().Throw<ShippedOrdersCannotBeChangedException>();
            orderRepository.getSavedOrder().Should().BeNull();
        }
    }
}

