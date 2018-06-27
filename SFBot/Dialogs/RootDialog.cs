using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using SFBot.Models;
using SFBot.Properties;


namespace SFBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private readonly ISFBotDialogFactory dialogFactory;
        private Models.SFRequest SFRequest;

        public RootDialog(ISFBotDialogFactory dialogFactory)
        {
            this.dialogFactory = dialogFactory;
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            await this.WelcomeMessageAsync(context);
        }

        private async Task WelcomeMessageAsync(IDialogContext context)
        {
            var reply = context.MakeMessage();

            string[] options = MessageOptions();

            reply.AddHeroCard(
                Resources.RootDialog_Welcome_Message,
                options, null);

            await context.PostAsync(reply);

            context.Wait(this.OnOptionSelected);
        }

        private static string[] MessageOptions()
        {
            return new[]
            {
                Resources.RootDialog_Welcome_SFRequest,
                Resources.RootDialog_Welcome_Solution_Helper
            };
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Text == Resources.RootDialog_Welcome_SFRequest)
            {
                this.SFRequest = new Models.SFRequest();

                context.Call(this.dialogFactory.Create<AreaRegionsDialog>(), this.AfterRegionSelected);
            }
            else if (message.Text == Resources.RootDialog_Welcome_Solution_Helper)
            {
                await this.StartOverAsync(context, Resources.RootDialog_SF_Solution_Area);
            }
        }

        private async Task AfterRegionSelected(IDialogContext context, IAwaitable<string> result)
        {
            var selectedRegion = await result;

            this.SFRequest.Area = selectedRegion;

            if (selectedRegion == "Region 1")
            {
                context.Call(this.dialogFactory.Create<SubsidiariesDialog>(), this.AfterSubsidiarySelected);
            }
            else
            {
                //context.Call()
            }
        }

        private async Task AfterSubsidiarySelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var selectedSubsidiary = await result;

                this.SFRequest.Subsidiary = selectedSubsidiary;

                var requestForm = new FormDialog<Models.SFRequest>(this.SFRequest, Models.SFRequest.BuildRequestForm, FormOptions.PromptInStart);
                context.Call(requestForm, this.AfterRequestForm);
            }
            catch (TooManyAttemptsException)
            {
                await this.StartOverAsync(context, Resources.RootDialog_TooManyAttempts);
            }
        }

        private async Task AfterRequestForm(IDialogContext context, IAwaitable<SFRequest> result)
        {
            await this.StartOverAsync(context, Resources.RootDialog_SF_Solution_Area);
        }

        //private async Task AfterOpportunity(IDialogContext context, IAwaitable<OpportunityID> result)
        //{
        //    var opportunity = await result as Activity;

        //    this.SFRequest.OpportunityID = opportunity.Text;

        //    await context.PostAsync(string.Format(CultureInfo.CurrentCulture, Resources.RootDialog_Bouquet_Selected, this.order.Bouquet.Name));

        //    PromptDialog.Choice(context, this.AfterDeliveryDateSelected, new[] { StringConstants.Today, StringConstants.Tomorrow }, Resources.RootDialog_DeliveryDate_Prompt);
        //}

        // TODO: review the stackoverflow execption thrown when makemessage method!!!!
        private async Task StartOverAsync(IDialogContext context, string text)
        {
            var message = context.MakeMessage();
            message.Text = text;
            await this.StartOverAsync(context, text);
        }

        private async Task StartOverAsync(IDialogContext context, IMessageActivity message)
        {
            await context.PostAsync(message);
            this.SFRequest = new Models.SFRequest();
            await this.WelcomeMessageAsync(context);
        }
    }
}