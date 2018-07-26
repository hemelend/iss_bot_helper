using System;
using System.Configuration;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents.Client;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;

using SFBot.Models;
using SFBot.Properties;
using SFBot.Services;

namespace SFBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private readonly ISFBotDialogFactory dialogFactory;
        private Models.SFRequest SFRequest;
        private bool userWelcomed;

        ////private static readonly string endpointUrl = ConfigurationManager.AppSettings["EndPointUrl"];
        ////private static readonly string authorizationKey = ConfigurationManager.AppSettings["AuthorizationKey"];
        ////private static readonly string databaseId = ConfigurationManager.AppSettings["DatabaseId"];
        ////private static readonly string collectionId = ConfigurationManager.AppSettings["CollectionId"];

        public RootDialog(ISFBotDialogFactory dialogFactory)
        {
            this.dialogFactory = dialogFactory;
            
        }

        public async Task StartAsync(IDialogContext context)
        {
            //context.Wait(MessageReceivedAsync);
            string userName;

            if (!context.UserData.TryGetValue("username", out userName))
            {
                PromptDialog.Text(context, ResumeAfterPrompt, "Before get started, please tell me your name?");
                return;
            }

            if (!userWelcomed)
            {
                userWelcomed = true;
                await context.PostAsync($"Welcome back {userName}!");

                context.Wait(MessageReceivedAsync);
            }

        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            await this.WelcomeMessageAsync(context);
        }
        private async Task ResumeAfterPrompt(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var userName = await result;
                userWelcomed = true;

                await context.PostAsync($"Welcome {userName}!");

                context.UserData.SetValue("username", userName);
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Oops! Something went wrong :( Technical Details: {ex}");
            }

            context.Wait(MessageReceivedAsync);
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
                await this.StartOverTextAsync(context, Resources.RootDialog_SF_Solution_Area);
            }
        }

        private async Task AfterRegionSelected(IDialogContext context, IAwaitable<string> result)
        {
            var selectedRegion = await result;

            this.SFRequest.Area = selectedRegion;

            context.Call(this.dialogFactory.Create<SubsidiariesDialog>(), this.AfterSubsidiarySelected);

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
                await this.StartOverTextAsync(context, Resources.RootDialog_TooManyAttempts);
            }
        }

        private async Task AfterRequestForm(IDialogContext context, IAwaitable<SFRequest> result)
        {
            context.PrivateConversationData.SetValue("sfrequest", SFRequest);

            //await repository.CreateItemAsync(SFRequest);
            await DocumentRepository.CreateItemAsync(SFRequest);


            await this.StartOverTextAsync(context, Resources.RootDialog_SF_Solution_Area);
        }

        //private async Task AfterOpportunity(IDialogContext context, IAwaitable<OpportunityID> result)
        //{
        //    var opportunity = await result as Activity;

        //    this.SFRequest.OpportunityID = opportunity.Text;

        //    await context.PostAsync(string.Format(CultureInfo.CurrentCulture, Resources.RootDialog_Bouquet_Selected, this.order.Bouquet.Name));

        //    PromptDialog.Choice(context, this.AfterDeliveryDateSelected, new[] { StringConstants.Today, StringConstants.Tomorrow }, Resources.RootDialog_DeliveryDate_Prompt);
        //}

        private async Task StartOverTextAsync(IDialogContext context, string text)
        {
            var message = context.MakeMessage();
            message.Text = text;
            
            await this.StartOverAsync(context, message);
            //context.Done<string>(null);
        }

        private async Task StartOverAsync(IDialogContext context, IMessageActivity message)
        {
            await context.PostAsync(message);

            await this.WelcomeMessageAsync(context);
            

        }
    }
}