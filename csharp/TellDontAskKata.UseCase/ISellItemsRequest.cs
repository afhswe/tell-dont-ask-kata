using System.Collections.Generic;

namespace TellDontAskKata.UseCase
{
    public interface ISellItemsRequest
    {
        void SetRequests(List<SellItemRequest> requests);
        List<SellItemRequest> GetRequests();
    }
}