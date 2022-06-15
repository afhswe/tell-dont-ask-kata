using System.Collections.Generic;
using System.Linq;

namespace TellDontAskKata.UseCase
{

    public class SellItemsRequest
    {
        private List<SellItemRequest> requestItems;

        public void SetRequests(List<SellItemRequest> requests)
        {
            requestItems = requests;
            if (ContainsMoreThanOneItemWithSameProduct())
            {
                requestItems = MergeItemRequestsOfSameProduct();
            }
        }

        private bool ContainsMoreThanOneItemWithSameProduct()
        {
            return requestItems.Count > requestItems.GroupBy(item => item.GetProductName()).ToList().Count;
        }

        public List<SellItemRequest> GetRequests()
        {
            return requestItems;
        }

        public List<SellItemRequest> MergeItemRequestsOfSameProduct()
        {
            var mergedRequests = new List<SellItemRequest>();
            foreach (SellItemRequest request in requestItems)
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

            return mergedRequests;
        }
    }
}