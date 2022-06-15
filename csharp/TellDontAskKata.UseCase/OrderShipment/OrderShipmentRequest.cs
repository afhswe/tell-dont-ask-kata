using System;

namespace TellDontAskKata.UseCase.OrderShipment
{
    public class OrderShipmentRequest
    {
        public int OrderId { get; set; }
        public DateTime OrderCreatedAt { get; set; }

    }
}
