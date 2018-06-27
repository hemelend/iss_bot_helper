using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFBot.Models
{

    [Serializable]
    public class SFRequest
    {

        public string Area { get; set; }

        public string Subsidiary { get; set; }

        [Prompt("Provide Customer Name")]
        public string CustomerName { get; set; }

        [Prompt("Provide MSX ID, it should be something like 7-XXXXXXX")]
        public string OpportunityID { get; set; }

        [Prompt("Require Demo?")]
        public bool Demo { get; set; }

        [Prompt("Meeting Date")]
        public DateTime StartTime { get; set; }

        [Prompt("Meeting Length")]
        public int MeetingLength { get; set; }

        [Prompt("What is your need?")]
        public string Description { get; set; }

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