using System.Collections.Generic;
using System.Linq;

namespace TellDontAskKata.UseCase
{

    public class SellItemsRequest : ISellItemsRequest
    {
        private List<SellItemRequest> requests;
        private List<SellItemRequest> mergedRequests;

        public void SetRequests(List<SellItemRequest> requests)
        {
            this.requests = requests;
        }

        public List<SellItemRequest> GetRequests()
        {
            return requests;
        }

        public void MergeItemRequestsOfSameProduct()
        {
            mergedRequests = new List<SellItemRequest>();
            foreach (SellItemRequest request in requests)
            {
                if (mergedRequests.Any(r => r.GetProductName() == request.GetProductName()))
                {
                    var current = mergedRequests.First(rq => rq.GetProductName() == request.GetProductName());
                    current.SetQuantity(current.GetQuantity() + request.GetQuantity());
                }
                else
                {
                    var requestCopy = new SellItemRequest();
                    requestCopy.SetProductName(request.GetProductName());
                    requestCopy.SetQuantity(request.GetQuantity());
                    mergedRequests.Add(requestCopy);
                }
            }
        }

        public List<SellItemRequest> GetMergedRequests()
        {
            return mergedRequests;
        }
    }
}