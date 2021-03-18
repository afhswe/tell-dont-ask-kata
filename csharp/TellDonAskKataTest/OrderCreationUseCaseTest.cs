using FluentAssertions;
using System;
using System.Collections.Generic;
using TellDontAskKata.Domain;
using TellDontAskKata.Test.Doubles;
using TellDontAskKata.UseCase;
using Xunit;

namespace TellDonAskKataTest
{
    public class OrderCreationUseCaseTest
    {
        private readonly TestOrderRepository orderRepository = new TestOrderRepository();
        private readonly Category food;
        private readonly OrderCreationUseCase useCase;

        public OrderCreationUseCaseTest()
        {
            food = new Category();
            food.setName("food");
            food.setTaxPercentage((decimal)10);

            var products = new List<Product>();
            var salad = new Product();
            salad.setName("salad");
            salad.setPrice((decimal)3.56);
            salad.setCategory(food);

            products.Add(salad);

            var tomato = new Product();
            tomato.setName("tomato");
            tomato.setPrice((decimal)4.65);
            tomato.setCategory(food);

            products.Add(tomato);

            var productCatalog = new InMemoryProductCatalog(products);

            useCase = new OrderCreationUseCase(orderRepository, productCatalog);
        }


        [Fact]
        public void sellMultipleItems()
        {
            SellItemRequest saladRequest = new SellItemRequest();
            saladRequest.setProductName("salad");
            saladRequest.setQuantity(2);

            SellItemRequest tomatoRequest = new SellItemRequest();
            tomatoRequest.setProductName("tomato");
            tomatoRequest.setQuantity(3);

            SellItemsRequest request = new SellItemsRequest();
            request.setRequests(new List<SellItemRequest>() { saladRequest, tomatoRequest });

            useCase.run(request);

            Order insertedOrder = orderRepository.getSavedOrder();
            insertedOrder.getStatus().Should().Be(OrderStatus.CREATED);
            insertedOrder.getTotal().Should().Be((decimal)23.20);
            insertedOrder.getTax().Should().Be((decimal)2.13);
            insertedOrder.getCurrency().Should().Be("EUR");
            insertedOrder.getItems().Should().HaveCount(2);
            insertedOrder.getItems()[0].getProduct().getName().Should().Be("salad");
            insertedOrder.getItems()[0].getProduct().getPrice().Should().Be((decimal)3.56);
            insertedOrder.getItems()[0].getQuantity().Should().Be(2);
            insertedOrder.getItems()[0].getTaxedAmount().Should().Be((decimal)7.84);
            insertedOrder.getItems()[0].getTax().Should().Be((decimal)0.72);
            insertedOrder.getItems()[1].getProduct().getName().Should().Be("tomato");
            insertedOrder.getItems()[1].getProduct().getPrice().Should().Be((decimal)4.65);
            insertedOrder.getItems()[1].getQuantity().Should().Be(3);
            insertedOrder.getItems()[1].getTaxedAmount().Should().Be((decimal)15.36);
            insertedOrder.getItems()[1].getTax().Should().Be((decimal)1.41);
        }

        [Fact]
        public void unknownProduct()
        {
            SellItemsRequest request = new SellItemsRequest();
            request.setRequests(new List<SellItemRequest>());
            SellItemRequest unknownProductRequest = new SellItemRequest();
            unknownProductRequest.setProductName("unknown product");
            request.getRequests().Add(unknownProductRequest);

            Action act = () => useCase.run(request);
            act.Should().Throw<UnknownProductException>();
        }
    }

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
        public void shipApprovedOrder()
        {
            Order initialOrder = new Order();
            initialOrder.setId(1);
            initialOrder.setStatus(OrderStatus.APPROVED);
            orderRepository.addOrder(initialOrder);

            OrderShipmentRequest request = new OrderShipmentRequest();
            request.setOrderId(1);

            useCase.run(request);

            orderRepository.getSavedOrder().getStatus().Should().Be(OrderStatus.SHIPPED);
            shipmentService.getShippedOrder().Should().Be(initialOrder);
        }

        [Fact]
        public void createdOrdersCannotBeShipped()
        {
            Order initialOrder = new Order();
            initialOrder.setId(1);
            initialOrder.setStatus(OrderStatus.CREATED);
            orderRepository.addOrder(initialOrder);

            OrderShipmentRequest request = new OrderShipmentRequest();
            request.setOrderId(1);

            Action act = () => useCase.run(request);
            act.Should().Throw<OrderCannotBeShippedException>();

            orderRepository.getSavedOrder().Should().BeNull();
            shipmentService.getShippedOrder().Should().BeNull();
        }

        [Fact]
        public void rejectedOrdersCannotBeShipped()
        {
            Order initialOrder = new Order();
            initialOrder.setId(1);
            initialOrder.setStatus(OrderStatus.REJECTED);
            orderRepository.addOrder(initialOrder);

            OrderShipmentRequest request = new OrderShipmentRequest();
            request.setOrderId(1);

            Action act = () => useCase.run(request);
            act.Should().Throw<OrderCannotBeShippedException>();

            orderRepository.getSavedOrder().Should().BeNull();
            shipmentService.getShippedOrder().Should().BeNull();
        }

        [Fact]
        public void shippedOrdersCannotBeShippedAgain()
        {
            Order initialOrder = new Order();
            initialOrder.setId(1);
            initialOrder.setStatus(OrderStatus.SHIPPED);
            orderRepository.addOrder(initialOrder);

            OrderShipmentRequest request = new OrderShipmentRequest();
            request.setOrderId(1);

            Action act = () => useCase.run(request);
            act.Should().Throw<OrderCannotBeShippedTwiceException>();

            orderRepository.getSavedOrder().Should().BeNull();
            shipmentService.getShippedOrder().Should().BeNull();
        }
    }
}

