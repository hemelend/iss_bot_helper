using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SFBot.Models;

namespace SFBot
{
    public class InMemoryAreaRegionsRepository : InMemoryRepositoryBase<AreaRegion>
    {
        private IEnumerable<AreaRegion> areaRegions;

        

        string[] regions = new string[] { "APAC", "Canada", "CentralAndEasternEurope", "France", "Germany", "GreaterChina", "India", "Japan", "Latam", "MEA", "UK", "UnitedStates", "WesternEurope" };

        public InMemoryAreaRegionsRepository()
        {
            this.areaRegions = Enumerable.Range(1, regions.Length)
                .Select(i => new AreaRegion
                {
                    Name = $"{regions.GetValue(i - 1)}",
                    ImageUrl = $"https://sfbotimages.blob.core.windows.net/regions/{regions.GetValue(i - 1)}.png"
                });
        }

        public override AreaRegion GetByName(string name)
        {
            return this.areaRegions.SingleOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        protected override IEnumerable<AreaRegion> Find(Func<AreaRegion, bool> predicate)
        {
            return predicate != default(Func<AreaRegion, bool>) ? this.areaRegions.Where(predicate) : this.areaRegions;
        }
    }
}