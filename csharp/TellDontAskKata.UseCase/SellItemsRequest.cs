using System.Collections.Generic;

namespace TellDontAskKata.UseCase
{

    public class SellItemsRequest
    {
        private List<SellItemRequest> requests;
        public void SetRequests(List<SellItemRequest> requests)
        {
            this.requests = requests;
        }

        public List<SellItemRequest> GetRequests()
        {
            return requests;
        }
    }
}