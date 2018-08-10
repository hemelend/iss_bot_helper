using SFBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace SFBot
{
    public class InMemorySubsidiariesRepository : InMemoryRepositoryBase<Subsidiary>
    {
        private IEnumerable<Subsidiary> Subsidiaries;

        string[] APAC_Subs = new string[] { "APAC Judgment", "Australia", "Indonesia", "Korea", "Malaysia", "New Zealand", "Philippines", "SEA Emerging Region", "Singapore", "Thailand", "Vietnam" };
        string[] Canada_Subs = new string[] { "Canada" };
        string[] CentralAndEasternEurope_Subs = new string[] { "Cyprus", "Czech Republic", "Greece", "Hungary", "Malta", "Poland", "Romania", "Slovakia", "Albania", "Armenia", "Azerbaijan", "Belarus",
                                                                "Bosnia and Hersegovina", "Bulgaria", "Central Asia", "Croatia", "Estonia", "Georgia", "Kazakhstan", "Latvia", "Lithuania", "Macedonia (FYROM)",
                                                                "Moldova", "Montenegro", "Serbia", "Slovenia", "Turkmenistan", "Ukraine", "Kosovo", "Central and Eastern Europe Judgment", "Rusia" };
        string[] France_Subs = new string[] { "France" };
        string[] Germany_Subs = new string[] { "Germany" };
        string[] GreaterChina_Subs = new string[] { "China", "Hong Kong", "Taiwan", "Greater China Judgment" };
        string[] India_Subs = new string[] { "India SC" };
        string[] Japan_Subs = new string[] { "Japan" };
        string[] LATAM_Subs = new string[] { "Argentina", "Uruguay", "Brazil", "Chile", "Colombia", "Latam Judgment", "Bolivia", "Costa Rica", "Dominican Republic", "Ecuador", "El Salvador", "Guatemala", "Honduras", "Panama",
                                                "Paraguay", "Peru", "Puerto Rico", "Trinidad & Tobago", "Venezuela", "BCCBBJ", "Mexico"};
        string[] MEA_Subs = new string[] { "Egypt", "Bahrain", "Kuwait", "Oman", "Qatar", "United Arab Emirates", "Israel", "MEA Judgment", "Algeria", "Iraq", "Jordan", "Lebanon", "Libya", "Morocco", "NEPA New Markets",
                                            "Pakistan", "Tunisia", "Iran", "Nigeria", "Saudi Arabia", "South Africa", "Turkey", "Africa New Markets", "Angola", "Ghana", "Indian Ocean Islands", "Kenya", "Rest of East & Southern Africa",
                                            "West and Central Africa" };
        string[] UK_Subs = new string[] { "United Kingdom" };
        string[] UnitedStates_Subs = new string[] { "United States" };
        string[] WesternEurope_Subs = new string[] { "Austria", "Belgium", "Luxembourg", "Denmark", "Iceland", "Finland", "Ireland", "Italy", "Netherlands", "Norway", "Portugal", "Spain", "Sweden", "Switzerland", "Western Europe Judgment" };

        public InMemorySubsidiariesRepository()
        {
            
            this.Subsidiaries = Enumerable.Range(1, LATAM_Subs.Length)
                .Select(i => new Subsidiary
                {
                    Name = $"{LATAM_Subs.GetValue(i - 1)}",
                    ImageUrl = $"https://placeholdit.imgix.net/~text?txtsize=48&txt={HttpUtility.UrlEncode("" + LATAM_Subs.GetValue(i - 1))}&w=640&h=330"
                });
        }

        public override Subsidiary GetByName(string name)
        {
            return this.Subsidiaries.SingleOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        protected override IEnumerable<Subsidiary> Find(Func<Subsidiary, bool> predicate)
        {
            return predicate != default(Func<Subsidiary, bool>) ? this.Subsidiaries.Where(predicate) : this.Subsidiaries;
        }
    }
}