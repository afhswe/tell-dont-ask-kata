using System;
using TellDontAskKata.Domain;

namespace TellDontAskKata.Service
{
    public interface IShipmentService
    {
        void ship(Order order);
    }
}
