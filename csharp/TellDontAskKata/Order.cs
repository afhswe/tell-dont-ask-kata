using System;
using System.Collections.Generic;

namespace TellDontAskKata.Domain
{
    public class Order
    {
        private decimal total;
        private String currency;
        private List<OrderItem> items;
        private decimal tax;
        private OrderStatus status;
        private int id;

        public decimal GetTotal()
        {
            return total;
        }

        public void SetTotal(decimal total)
        {
            this.total = total;
        }

        public String GetCurrency()
        {
            return currency;
        }

        public void SetCurrency(String currency)
        {
            this.currency = currency;
        }

        public List<OrderItem> GettItems()
        {
            return items;
        }

        public void SetItems(List<OrderItem> items)
        {
            this.items = items;
        }

        public decimal GetTax()
        {
            return tax;
        }

        public void SetTax(decimal tax)
        {
            this.tax = tax;
        }

        public OrderStatus GetStatus()
        {
            return status;
        }

        public void SetStatus(OrderStatus status)
        {
            this.status = status;
        }

        public int GetId()
        {
            return id;
        }

        public void SetId(int id)
        {
            this.id = id;
        }
    }
}
