using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using SFBot.Models;
using SFBot.Properties;

namespace SFBot.Dialogs
{
    [Serializable]
    public class SubsidiariesDialog : PagedCarouselDialog<string>
    {
        private readonly IRepository<Subsidiary> repository;

        public SubsidiariesDialog(IRepository<Subsidiary> repository)
        {
            this.repository = repository;
        }

        public override string Prompt
        {
            get { return Resources.SubsidiariesDialog_Prompt; }
        }

        public override PagedCarouselCards GetCarouselCards(int pageNumber, int pageSize)
        {
            var pagedResult = this.repository.RetrievePage(pageNumber, pageSize);

            var carouselCards = pagedResult.Items.Select(it => new HeroCard
            {
                Title = it.Name,
                Images = new List<CardImage> { new CardImage(it.ImageUrl, it.Name) },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.ImBack, Resources.SubsidiariesDialog_Select, value: it.Name) }
            });

            return new PagedCarouselCards
            {
                Cards = carouselCards,
                TotalCount = pagedResult.TotalCount
            };
        }

        public override async Task ProcessMessageReceived(IDialogContext context, string SubsidiaryName)
        {
            var subsidiary = this.repository.GetByName(SubsidiaryName);

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