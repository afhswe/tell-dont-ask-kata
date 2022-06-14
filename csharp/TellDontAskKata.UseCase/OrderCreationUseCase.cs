using System;
using System.Collections.Generic;
using System.Linq;
using TellDontAsk.Repository;
using TellDontAskKata.Domain;
using TellDontAskKata.Repository;

namespace TellDontAskKata.UseCase
{
    public class OrderCreationUseCase
    {
        private readonly IOrderRepository orderRepository;
        private readonly IProductCatalog productCatalog;
        private List<Product> _productsCache = new List<Product>();

        public OrderCreationUseCase(IOrderRepository orderRepository, IProductCatalog productCatalog)
        {
            this.orderRepository = orderRepository;
            this.productCatalog = productCatalog;
        }

        public Order Run(SellItemsRequest request)
        {
            Order order = new Order();
            order.SetId(orderRepository.NextId());
            order.SetStatus(OrderStatus.Created);
            order.SetItems(new List<OrderItem>());
            order.SetCurrency("EUR");
            order.SetTotal((decimal)0.0);
            order.SetTax((decimal)0.0);

            int numberOfFoodItems = 0;
            EmptyProductsCache();

            request.MergeItemRequestsOfSameProduct();

            foreach (SellItemRequest itemRequest in request.GetRequests())
            {
                var product = QueryProduct(itemRequest);

                if (product == null)
                {
                    throw new UnknownProductException();
                }

                decimal unitaryTax = Math.Round((product.GetPrice() / 100) * (product.GetCategory().GetTaxPercentage()), 2, MidpointRounding.AwayFromZero);
                decimal unitaryTaxedAmount = Math.Round(product.GetPrice() + unitaryTax, 2, MidpointRounding.AwayFromZero);
                decimal taxedAmount = Math.Round(unitaryTaxedAmount * itemRequest.GetQuantity(), 2, MidpointRounding.AwayFromZero);
                decimal taxAmount = Math.Round(unitaryTax * itemRequest.GetQuantity(), 2, MidpointRounding.AwayFromZero);

                OrderItem orderItem = new OrderItem();
                orderItem.SetProduct(product);
                orderItem.SetQuantity(itemRequest.GetQuantity());
                orderItem.SetTax(taxAmount);
                orderItem.SetTaxedAmount(taxedAmount);
                    order.GetItems().Add(orderItem);

                    foreach (OrderItem item in order.GetItems())
                {
                    if (item.GetProduct().GetCategory().GetName().Equals("food"))
                    {
                        numberOfFoodItems += item.GetQuantity();
                    }
                }

                if (numberOfFoodItems > 100)
                {
                    throw new MaximumNumberOfFoodItemsExceeded();
                }

                order.SetTotal(order.GetTotal() + taxedAmount);
                order.SetTax(order.GetTax() + taxAmount);
            }

            orderRepository.Save(order);

            return order;
        }

        private void EmptyProductsCache()
        {
            _productsCache = new List<Product>();
        }

        private Product QueryProduct(SellItemRequest itemRequest)
        {
            if (_productsCache.Any(p => p.GetName() == itemRequest.GetProductName()))
            {
                return _productsCache.First(p => p.GetName() == itemRequest.GetProductName());
            }
            Product product = productCatalog.GetByName(itemRequest.GetProductName());
            _productsCache.Add(product);
            return product;
        }
    }
}
