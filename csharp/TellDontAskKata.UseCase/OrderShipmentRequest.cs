﻿using System;

namespace TellDontAskKata.UseCase
{
    public class OrderShipmentRequest
    {
        public int OrderId { get; set; }
        public DateTime OrderCreatedAt { get; set; }

    }
}
