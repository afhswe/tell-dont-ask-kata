﻿namespace TellDontAskKata.UseCase
{
    public class OrderShipmentRequest
    {
        private int orderId;

        public void setOrderId(int orderId)
        {
            this.orderId = orderId;
        }

        public int getOrderId()
        {
            return orderId;
        }
    }
}
