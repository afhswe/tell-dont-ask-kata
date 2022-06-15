﻿using System.Collections.Generic;

namespace TellDontAskKata.UseCase
{
    public interface ISellItemsRequest
    {
        void SetRequests(List<SellItemRequest> requests);
        List<SellItemRequest> GetRequests();
        void MergeItemRequestsOfSameProduct();
        List<SellItemRequest> GetMergedRequests();
    }
}