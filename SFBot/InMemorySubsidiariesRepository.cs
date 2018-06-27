using SFBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace SFBot
{
    public class InMemorySubsidiariesRepository : InMemoryRepositoryBase<Subsidiary>
    {
        private IEnumerable<Subsidiary> subsidiaries;

        public InMemorySubsidiariesRepository()
        {
            this.subsidiaries = Enumerable.Range(1, 5)
                .Select(i => new Subsidiary
                {
                    Name = $"Subsidiary {i}",
                    ImageUrl = $"https://placeholdit.imgix.net/~text?txtsize=48&txt={HttpUtility.UrlEncode("Subsidiary " + i)}&w=640&h=330"
                });
        }

        public override Subsidiary GetByName(string name)
        {
            return this.subsidiaries.SingleOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        protected override IEnumerable<Subsidiary> Find(Func<Subsidiary, bool> predicate)
        {
            return predicate != default(Func<Subsidiary, bool>) ? this.subsidiaries.Where(predicate) : this.subsidiaries;
        }
    }
}