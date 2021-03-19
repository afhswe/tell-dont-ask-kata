using System;
using System.Collections.Generic;
using TellDontAsk.Repository;
using TellDontAskKata.Domain;
using TellDontAskKata.Repository;

namespace TellDontAskKata.UseCase
{
    public class OrderCreationUseCase
    {
        private readonly IOrderRepository orderRepository;
        private readonly IProductCatalog productCatalog;

        public OrderCreationUseCase(IOrderRepository orderRepository, IProductCatalog productCatalog)
        {
            this.orderRepository = orderRepository;
            this.productCatalog = productCatalog;
        }

        public void Run(SellItemsRequest request)
        {
            Order order = new Order();
            order.SetStatus(OrderStatus.Created);
            order.SetItems(new List<OrderItem>());
            order.SetCurrency("EUR");
            order.SetTotal((decimal)0.0);
            order.SetTax((decimal)0.0);

            foreach (SellItemRequest itemRequest in request.GetRequests())
            {
                Product product = productCatalog.GetByName(itemRequest.GetProductName());

                if (product == null)
                {
                    throw new UnknownProductException();
                }
                else
                {
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

                    order.SetTotal(order.GetTotal() + taxedAmount);
                    order.SetTax(order.GetTax() + taxAmount);
                }
            }

            orderRepository.Save(order);
        }
    }
}
