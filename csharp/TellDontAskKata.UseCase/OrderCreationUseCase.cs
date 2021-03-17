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

        public void run(SellItemsRequest request)
        {
            Order order = new Order();
            order.setStatus(OrderStatus.CREATED);
            order.setItems(new List<OrderItem>());
            order.setCurrency("EUR");
            order.setTotal((decimal)0.0);
            order.setTax((decimal)0.0);

            foreach (SellItemRequest itemRequest in request.getRequests())
            {
                Product product = productCatalog.getByName(itemRequest.getProductName());

                if (product == null)
                {
                    throw new UnknownProductException();
                }
                else
                {
                    decimal unitaryTax = Math.Round(product.getPrice() / (100 * (product.getCategory().getTaxPercentage())), 2, MidpointRounding.AwayFromZero);
                    decimal unitaryTaxedAmount = Math.Round(product.getPrice() + unitaryTax, 2, MidpointRounding.AwayFromZero);
                    decimal taxedAmount = Math.Round(unitaryTaxedAmount * (itemRequest.getQuantity()), 2, MidpointRounding.AwayFromZero);
                    decimal taxAmount = Math.Round(unitaryTax * itemRequest.getQuantity(), 2, MidpointRounding.AwayFromZero);

                    OrderItem orderItem = new OrderItem();
                    orderItem.setProduct(product);
                    orderItem.setQuantity(itemRequest.getQuantity());
                    orderItem.setTax(taxAmount);
                    orderItem.setTaxedAmount(taxedAmount);
                    order.getItems().Add(orderItem);

                    order.setTotal(order.getTotal() + taxedAmount);
                    order.setTax(order.getTax() + taxAmount);
                }
            }

            orderRepository.save(order);
        }
    }
}
