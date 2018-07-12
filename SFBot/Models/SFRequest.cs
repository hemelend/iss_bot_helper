using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SFBot.Models
{

    [Serializable]
    public class SFRequest
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [BsonElement("Area")]
        public string Area { get; set; }

        [BsonElement("Subsidiary")]
        public string Subsidiary { get; set; }

        [BsonElement("CustomerName")]
        [Prompt("Provide Customer Name")]
        public string CustomerName { get; set; }

        [BsonElement("OpportunityID")]
        [Prompt("Provide MSX ID, it should be something like 7-XXXXXXX")]
        [Pattern(@"^7-.{1,10}$")]
        public string OpportunityID { get; set; }

        [BsonElement("Demo")]
        [Prompt("Require Demo? Say 'Yes' or 'No'")]
        public bool Demo { get; set; }

        [BsonElement("StartTime")]
        [Prompt("Meeting Date")]
        public DateTime StartTime { get; set; }

        [BsonElement("MeetingLength")]
        [Prompt("Meeting Length")]
        [Range(1, 100, ErrorMessage = "Please enter a value lower than {1}")]
        public int MeetingLength { get; set; }

        [BsonElement("Description")]
        [Prompt("What is your need?")]
        public string Description { get; set; }

        [BsonElement("SolutionArea")]
        [Prompt("Solution Area")]
        public string SolutionArea { get; set; }

        public static IForm<SFRequest> BuildRequestForm()
        {
            return new FormBuilder<SFRequest>()
                .Field(nameof(CustomerName))
                .Field(nameof(OpportunityID))
                .Field(nameof(Demo))
                .Field(nameof(StartTime))
                .Field(nameof(MeetingLength))
                .Field(nameof(Description))
                .Field(nameof(SolutionArea))
                .Build();
        }
    }
}