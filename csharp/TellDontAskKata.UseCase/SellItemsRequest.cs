using System.Collections.Generic;
using System.Linq;

namespace TellDontAskKata.UseCase
{

    public class SellItemsRequest : ISellItemsRequest
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

        public void MergeItemRequestsOfSameProduct()
        {
            /*var mergedList = new List<SellItemRequest>();
            foreach (SellItemRequest request in requests)
            {
                if (mergedList.Any(r => r.GetProductName() == request.GetProductName()))
                {
                    var current = mergedList.First(rq => rq.GetProductName() == request.GetProductName());
                    current.SetQuantity(current.GetQuantity() + request.GetQuantity());
                }
                else
                {
                    var requestCopy = new SellItemRequest();
                    requestCopy.SetProductName(request.GetProductName());
                    requestCopy.SetQuantity(request.GetQuantity());
                    mergedList.Add(requestCopy);
                }
            }

            this.requests = mergedList;*/
        }
    }
}