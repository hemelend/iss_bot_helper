using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFBot.Models
{
    public class SFRequest
    {
         public enum AreaOptions
        {
            APAC, Canada, CentralAndEasternEurope, France, Germany, GreaterChina, India, Japan, Latam, MEA, UK, UnitedStates, WesternEurope
        }
        public AreaOptions? Area;

        [Prompt]
        public string CustomerName { get; set; }

        [Prompt]
        public string OpportunityID { get; set; }

        [Prompt]
        public bool Demo { get; set; }

        [Prompt]
        public DateTime StartTime { get; set; }

        [Prompt]
        public string Description { get; set; }

        [Prompt]
        public string SolutionArea { get; set; }


        public static IForm<SFRequest> BuildRequestForm()
        {
            return new FormBuilder<SFRequest>()
                .Field(new FieldReflector<SFRequest>(nameof(AreaOptions)))
                .Field(nameof(CustomerName))
                .Field(nameof(OpportunityID))
                .Field(nameof(Demo))
                .Field(nameof(StartTime))
                .Field(nameof(Description))
                .Field(nameof(SolutionArea))
                .Build();
        }
    }
}