using System;

namespace TellDonAskKataTest;

public interface IDateTimeProvider
{
    DateTime CurrentDateTime();
}