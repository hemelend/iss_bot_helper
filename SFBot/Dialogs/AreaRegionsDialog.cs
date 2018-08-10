using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using SFBot.Models;
using SFBot.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SFBot.Dialogs
{
    [Serializable]
    public class AreaRegionsDialog : PagedCarouselDialog<string>
    {
        private readonly IRepository<AreaRegion> repository;

        public AreaRegionsDialog(IRepository<AreaRegion> repository)
        {
            this.repository = repository;
        }

        public override string Prompt
        {
            get { return Resources.AreaRegionsDialog_Prompt; }
        }

        public override PagedCarouselCards GetCarouselCards(int pageNumber, int pageSize)
        {
            var pagedResult = this.repository.RetrievePage(pageNumber, pageSize);

            var carouselCards = pagedResult.Items.Select(it => new HeroCard
            {
                Title = it.Name,
                Images = new List<CardImage> { new CardImage(it.ImageUrl, it.Name) },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.ImBack, Resources.AreaRegionsDialog_Select, value: it.Name) }
            });

            return new PagedCarouselCards
            {
                Cards = carouselCards,
                TotalCount = pagedResult.TotalCount
            };
        }

        public override async Task ProcessMessageReceived(IDialogContext context, string AreaRegionName)
        {
            var areaRegion = this.repository.GetByName(AreaRegionName);

            if (areaRegion != null)
            {
                context.Done(areaRegion.Name);
            }
            else
            {
                await context.PostAsync(string.Format(CultureInfo.CurrentCulture, Resources.AreaRegionsDialog_InvalidOption, AreaRegionName));
                await this.ShowProducts(context);
                context.Wait(this.MessageReceivedAsync);
            }
        }
    }
}