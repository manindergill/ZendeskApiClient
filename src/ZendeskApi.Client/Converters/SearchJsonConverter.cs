﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZendeskApi.Client.Models;
using ZendeskApi.Client.Responses;

namespace ZendeskApi.Client.Converters
{
    public class SearchJsonConverter : JsonConverter
    {
        private static Type[] DeserializableSearchTypes =
            new Type[] { typeof(Ticket), typeof(User), typeof(Group), typeof(Organization) /*, typeof(Topic) */ }; // TODO: Introduce Topics?

        public override bool CanConvert(Type objectType)
        {
            return typeof(SearchResultsResponse) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var keys = DeserializableSearchTypes.ToDictionary(k =>
                k.GetTypeInfo().GetCustomAttribute<JsonObjectAttribute>().Id,
                v => v);

            var token = serializer
                .Deserialize<JObject>(reader);
            
            var results = new List<ISearchResult>();

            foreach (var result in token.SelectToken("results").Children())
            {
                results.Add((ISearchResult)result.ToObject(keys[result.Value<string>("result_type")]));
            }

            var nextPage = token.Value<string>("next_page");
            var previousPage = token.Value<string>("previous_page");

            return new SearchResultsResponse
            {
                Count = token.Value<int>("count"),
                NextPage = nextPage != null ? new Uri(nextPage) : null,
                PreviousPage = previousPage != null ? new Uri(previousPage) : null,
                Item = results
            };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }
    }
}
