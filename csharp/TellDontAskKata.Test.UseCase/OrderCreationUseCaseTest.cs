using System.Collections.Generic;
using System.Linq;
using FluentAssertions.Common;
using TellDontAskKata.Repository;
using TellDontAskKata.UseCase.OrderCreation;

namespace TellDonAskKataTest
{
    public class OrderCreationUseCaseTest
    {
        private readonly Mock<IOrderRepository> orderRepository = new();
        private readonly Mock<IProductCatalog> productCatalog = new();
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

            productCatalog.Setup(x => x.GetByName("salad")).Returns(salad);
            productCatalog.Setup(x => x.GetByName("tomato")).Returns(tomato);

            useCase = new OrderCreationUseCase(orderRepository.Object, productCatalog.Object);
        }


        [Fact]
        public void CreatesOrder_WithMultipleItems()
        {
            SellItemRequest saladRequest = new SellItemRequest();
            saladRequest.SetProductName("salad");
            saladRequest.SetQuantity(2);

            SellItemRequest tomatoRequest = new SellItemRequest();
            tomatoRequest.SetProductName("tomato");
            tomatoRequest.SetQuantity(3);

            ISellItemsRequest request = new SellItemsRequest();
            request.SetRequests(new List<SellItemRequest>() { saladRequest, tomatoRequest });

            orderRepository.Setup(x => x.NextId()).Returns(1);

            Order createdOrder = useCase.Run(request);
            createdOrder.GetItems().Should().HaveCount(2);
            createdOrder.GetItems().Should().Contain(item => item.GetProduct().GetName() == "salad");
            createdOrder.GetItems().Should().Contain(item => item.GetProduct().GetName() == "tomato");
            createdOrder.GetStatus().Should().Be(OrderStatus.Created);
        }


        [Fact]
        public void CalculatesPriceIncludingTaxes_ForCreatedOrder()
        {
            SellItemRequest saladRequest = new SellItemRequest();
            saladRequest.SetProductName("salad");
            saladRequest.SetQuantity(2);

            ISellItemsRequest request = new SellItemsRequest();
            request.SetRequests(new List<SellItemRequest>() { saladRequest });

            orderRepository.Setup(x => x.NextId()).Returns(1);

            Order createdOrder = useCase.Run(request);

            var saladItemFromOrder = 
                createdOrder.GetItems().First(item => item.GetProduct().GetName() == "salad");
            saladItemFromOrder.GetTax().Should().Be(0.72M);
            saladItemFromOrder.GetTaxedAmount().Should().Be(7.84M);
            saladItemFromOrder.GetQuantity().Should().Be(2);

            createdOrder.GetCurrency().Should().Be("EUR");
            createdOrder.GetTax().Should().Be(0.72M);
            createdOrder.GetTotal().Should().Be(7.84M);
            createdOrder.GetStatus().Should().Be(OrderStatus.Created);

            orderRepository.Verify(x => x.Save(It.Is<Order>(order => order.GetId() == createdOrder.GetId())), Times.Once);
        }

        [Fact]
        public void DoesNotProcessUnknownProduct()
        {
            ISellItemsRequest request = new SellItemsRequest();
            SellItemRequest unknownProductRequest = new SellItemRequest();
            unknownProductRequest.SetProductName("unknown product");
            request.SetRequests(new List<SellItemRequest>() { unknownProductRequest });
            productCatalog.Setup(x => x.GetByName("unknown product")).Returns((Product) null);

            Action act = () => useCase.Run(request);
            act.Should().Throw<UnknownProductException>();
        }

        [Fact]
        public void MaximumNumberOfFoodItemsExceededWithSingleQuantity()
        {
            ISellItemsRequest tooManyFoodItemsRequest = new SellItemsRequest();
            var sellItemRequests = new List<SellItemRequest>();

            for (var i = 0; i <= 100; i++)
            {
                SellItemRequest foodItemRequest = new SellItemRequest();
                foodItemRequest.SetProductName("salad");
                foodItemRequest.SetQuantity(1);
                sellItemRequests.Add(foodItemRequest);
            }
            tooManyFoodItemsRequest.SetRequests(sellItemRequests);

            Action act = () => useCase.Run(tooManyFoodItemsRequest);
            act.Should().Throw<MaximumNumberOfFoodItemsExceeded>();
        }

        [Fact]
        public void MaximumNumberOfFoodItemsExceededWithMultipleQuantity()
        {
            ISellItemsRequest tooManyFoodItemsRequest = new SellItemsRequest();

            SellItemRequest firstSaladItem = new SellItemRequest();
            firstSaladItem.SetProductName("salad");
            firstSaladItem.SetQuantity(30);

            SellItemRequest secondSaladItem = new SellItemRequest();
            secondSaladItem.SetProductName("tomato");
            secondSaladItem.SetQuantity(71);

            tooManyFoodItemsRequest.SetRequests(new List<SellItemRequest>() { firstSaladItem, secondSaladItem, });
            Action act = () => useCase.Run(tooManyFoodItemsRequest);
            act.Should().Throw<MaximumNumberOfFoodItemsExceeded>();
        }

        [Fact]
        public void StoresSucessfullyCreatedOrders()
        {
            SellItemRequest saladRequest = new SellItemRequest();
            saladRequest.SetProductName("salad");
            saladRequest.SetQuantity(1);

            ISellItemsRequest request = new SellItemsRequest();
            request.SetRequests(new List<SellItemRequest>() { saladRequest });

            orderRepository.Setup(x => x.NextId()).Returns(1);

            useCase.Run(request);

            orderRepository.Verify(x => x.Save(It.Is<Order>(o => o.GetStatus() == OrderStatus.Created)), Times.Once);
        }

        [Fact]
        public void MergesItems_WithSameProduct_ToOneItem()
        {
            var twoItemsWithSameProductRequest = new Mock<ISellItemsRequest>();
            var itemRequests = new List<SellItemRequest>();

            SellItemRequest foodItemRequest = new SellItemRequest();
            foodItemRequest.SetProductName("salad");
            foodItemRequest.SetQuantity(1);
            itemRequests.Add(foodItemRequest);

            foodItemRequest = new SellItemRequest();
            foodItemRequest.SetProductName("salad");
            foodItemRequest.SetQuantity(1);
            itemRequests.Add(foodItemRequest);

            twoItemsWithSameProductRequest.Setup(x => x.GetRequests()).Returns(itemRequests);

            var mergedFoodItemRequest = new SellItemRequest();
            mergedFoodItemRequest.SetProductName("salad");
            mergedFoodItemRequest.SetQuantity(2);
            twoItemsWithSameProductRequest.Setup(x => x.GetMergedRequests()).Returns(new List<SellItemRequest>()
            {
                mergedFoodItemRequest
            });

            orderRepository.Setup(x => x.NextId()).Returns(1);
            var orderResult = useCase.Run(twoItemsWithSameProductRequest.Object);

            orderResult.GetItems().Should().HaveCount(1);
            orderResult.GetItems().Should()
                .Contain(item => item.GetProduct().GetName() == "salad" && item.GetQuantity() == 2);

            productCatalog.Verify(x => x.GetByName("salad"), Times.Exactly(1));
            twoItemsWithSameProductRequest.Verify(x => x.MergeItemRequestsOfSameProduct(), Times.Once);
        }
    }
}

