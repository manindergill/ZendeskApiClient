﻿using System.Collections.Generic;
using Newtonsoft.Json;
using ZendeskApi.Client.Models;

namespace ZendeskApi.Client.Responses
{
    [JsonObject]
    public class RequestsResponse : PaginationResponse<Request>
    {
        [JsonProperty("requests")]
        public override IEnumerable<Request> Item { get; set; }
    }
}
