using System.Collections.Generic;
using System.Linq;
using TellDontAskKata.Repository;

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

            SellItemsRequest request = new SellItemsRequest();
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

            SellItemsRequest request = new SellItemsRequest();
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
        }

        [Fact]
        public void DoesNotProcessUnknownProduct()
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
        public void MaximumNumberOfFoodItemsExceededWithSingleQuantity()
        {
            SellItemsRequest tooManyFoodItemsRequest = new SellItemsRequest();
            tooManyFoodItemsRequest.SetRequests(new List<SellItemRequest>());

            for (var i = 0; i <= 100; i++)
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

        [Fact]
        public void StoresSucessfullyCreatedOrder()
        {
            SellItemRequest saladRequest = new SellItemRequest();
            saladRequest.SetProductName("salad");
            saladRequest.SetQuantity(1);

            SellItemsRequest request = new SellItemsRequest();
            request.SetRequests(new List<SellItemRequest>() { saladRequest });

            orderRepository.Setup(x => x.NextId()).Returns(1);

            useCase.Run(request);

            orderRepository.Verify(x => x.Save(It.Is<Order>(o => o.GetStatus() == OrderStatus.Created)), Times.Once);
        }

        [Fact]
        public void MergesItems_WithSameProduct_ToOneItem()
        {
            SellItemsRequest twoItemsWithSameProductRequest = new SellItemsRequest();
            twoItemsWithSameProductRequest.SetRequests(new List<SellItemRequest>());

            SellItemRequest foodItemRequest = new SellItemRequest();
            foodItemRequest.SetProductName("salad");
            foodItemRequest.SetQuantity(1);
            twoItemsWithSameProductRequest.GetRequests().Add(foodItemRequest);

            foodItemRequest = new SellItemRequest();
            foodItemRequest.SetProductName("salad");
            foodItemRequest.SetQuantity(1);
            twoItemsWithSameProductRequest.GetRequests().Add(foodItemRequest);

            orderRepository.Setup(x => x.NextId()).Returns(1);
            var orderResult = useCase.Run(twoItemsWithSameProductRequest);
            orderResult.GetItems().Should().HaveCount(1);
            orderResult.GetItems().Should()
                .Contain(item => item.GetProduct().GetName() == "salad" && item.GetQuantity() == 2);

            productCatalog.Verify(x => x.GetByName("salad"), Times.Exactly(2));
        }
    }
}

