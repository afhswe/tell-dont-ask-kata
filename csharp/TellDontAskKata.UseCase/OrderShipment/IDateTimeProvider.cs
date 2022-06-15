using System;

namespace TellDontAskKata.UseCase.OrderShipment;

public interface IDateTimeProvider
{
    DateTime CurrentDateTime();
}