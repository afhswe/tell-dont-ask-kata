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
            food.SetName("food");
            food.SetTaxPercentage((decimal)10);

            var products = new List<Product>();
            var salad = new Product();
            salad.SetName("salad");
            salad.SetPrice((decimal)3.56);
            salad.SetCategory(food);

            products.Add(salad);

            var tomato = new Product();
            tomato.SetName("tomato");
            tomato.SetPrice((decimal)4.65);
            tomato.SetCategory(food);

            products.Add(tomato);

            var productCatalog = new InMemoryProductCatalog(products);

            useCase = new OrderCreationUseCase(orderRepository, productCatalog);
        }


        [Fact]
        public void SellMultipleItems()
        {
            SellItemRequest saladRequest = new SellItemRequest();
            saladRequest.SetProductName("salad");
            saladRequest.SetQuantity(2);

            SellItemRequest tomatoRequest = new SellItemRequest();
            tomatoRequest.SetProductName("tomato");
            tomatoRequest.SetQuantity(3);

            SellItemsRequest request = new SellItemsRequest();
            request.SetRequests(new List<SellItemRequest>() { saladRequest, tomatoRequest });

            useCase.Run(request);

            Order insertedOrder = orderRepository.GetSavedOrder();
            insertedOrder.GetStatus().Should().Be(OrderStatus.Created);
            insertedOrder.GetTotal().Should().Be((decimal)23.20);
            insertedOrder.GetTax().Should().Be((decimal)2.13);
            insertedOrder.GetCurrency().Should().Be("EUR");
            insertedOrder.GetItems().Should().HaveCount(2);
            insertedOrder.GetItems()[0].GetProduct().GetName().Should().Be("salad");
            insertedOrder.GetItems()[0].GetProduct().GetPrice().Should().Be((decimal)3.56);
            insertedOrder.GetItems()[0].getQuantity().Should().Be(2);
            insertedOrder.GetItems()[0].GetTaxedAmount().Should().Be((decimal)7.84);
            insertedOrder.GetItems()[0].GetTax().Should().Be((decimal)0.72);
            insertedOrder.GetItems()[1].GetProduct().GetName().Should().Be("tomato");
            insertedOrder.GetItems()[1].GetProduct().GetPrice().Should().Be((decimal)4.65);
            insertedOrder.GetItems()[1].getQuantity().Should().Be(3);
            insertedOrder.GetItems()[1].GetTaxedAmount().Should().Be((decimal)15.36);
            insertedOrder.GetItems()[1].GetTax().Should().Be((decimal)1.41);
        }

        [Fact]
        public void UnknownProduct()
        {
            SellItemsRequest request = new SellItemsRequest();
            request.SetRequests(new List<SellItemRequest>());
            SellItemRequest unknownProductRequest = new SellItemRequest();
            unknownProductRequest.SetProductName("unknown product");
            request.GetRequests().Add(unknownProductRequest);

            Action act = () => useCase.Run(request);
            act.Should().Throw<UnknownProductException>();
        }

        [Fact]
        public void maximumNumberOfFoodItemsExceededWithSingleQuantity()
        {
            SellItemsRequest tooManyFoodItemsRequest = new SellItemsRequest();
            tooManyFoodItemsRequest.SetRequests(new List<SellItemRequest>());

            for (int i = 0; i <= 100; i++)
            {
                SellItemRequest foodItemRequest = new SellItemRequest();
                foodItemRequest.SetProductName("salad");
                foodItemRequest.SetQuantity(1);
                tooManyFoodItemsRequest.GetRequests().Add(foodItemRequest);
            }

            Action act = () => useCase.Run(tooManyFoodItemsRequest);
            act.Should().Throw<MaximumNumberOfFoodItemsExceeded>();
        }

        [Fact]
        public void MaximumNumberOfFoodItemsExceededWithMultipleQuantity()
        {
            SellItemsRequest tooManyFoodItemsRequest = new SellItemsRequest();
            tooManyFoodItemsRequest.SetRequests(new List<SellItemRequest>());

            SellItemRequest foodItemRequest = new SellItemRequest();
            foodItemRequest.SetProductName("salad");
            foodItemRequest.SetQuantity(30);
            tooManyFoodItemsRequest.GetRequests().Add(foodItemRequest);

            foodItemRequest = new SellItemRequest();
            foodItemRequest.SetProductName("tomato");
            foodItemRequest.SetQuantity(71);
            tooManyFoodItemsRequest.GetRequests().Add(foodItemRequest);

            Action act = () => useCase.Run(tooManyFoodItemsRequest);
            act.Should().Throw<MaximumNumberOfFoodItemsExceeded>();
        }
    }
}

