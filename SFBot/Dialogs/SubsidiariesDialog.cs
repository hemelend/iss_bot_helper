using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using SFBot.Models;
using SFBot.Properties;
using SFBot.Services;

namespace SFBot.Dialogs
{
    [Serializable]
    public class SubsidiariesDialog : PagedCarouselDialog<string>
    {
        //TODO:create a Collection with documents of subsidiaries for each region
        private readonly IRepository<Subsidiary> repository;
        //private readonly IEnumerable<Subsidiary> subs;

        //public IEnumerable<Subsidiary> Regionsubs { get; set; }
        //public string Region { get; set; }

        public SubsidiariesDialog(IRepository<Subsidiary> repository)
        //public SubsidiariesDialog(IEnumerable<Subsidiary> Regionsubs)
        //public SubsidiariesDialog(string Region)
        {
            this.repository = repository;

            //this.subs = SFDocDB.GetAllSubsidiaries(Region);
            //subs = Regionsubs;
        }

        public override string Prompt
        {
            get { return Resources.SubsidiariesDialog_Prompt; }
        }

        public override PagedCarouselCards GetCarouselCards(int pageNumber, int pageSize)
        {
            var pagedResult = this.repository.RetrievePage(pageNumber, pageSize);

            var carouselCards = pagedResult.Items.Select(it => new HeroCard

            //var carouselCards = subs.Select(it => new HeroCard
            {
                Title = it.Name,
                Images = new List<CardImage> { new CardImage(it.ImageUrl, it.Name) },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.ImBack, Resources.SubsidiariesDialog_Select, value: it.Name) }
            });

            return new PagedCarouselCards
            {
                Cards = carouselCards,
                TotalCount = pagedResult.TotalCount
                //TotalCount = subs.Count()
            };
        }

        public override async Task ProcessMessageReceived(IDialogContext context, string SubsidiaryName)
        {
            var subsidiary = this.repository.GetByName(SubsidiaryName);
            //var subsidiary = SFDocDB.GetSubsidiary(SubsidiaryName);

            if (subsidiary != null)
            {
                context.Done(subsidiary.Name);
            }
            else
            {
                await context.PostAsync(string.Format(CultureInfo.CurrentCulture, Resources.SubsidiariesDialog_InvalidOption, SubsidiaryName));
                await this.ShowProducts(context);
                context.Wait(this.MessageReceivedAsync);
            }
        }
    }
}