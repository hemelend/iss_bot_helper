using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using SFBot.Properties;


namespace SFBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        //private Models.SFRequest SFRequest;


        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            //return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            await this.WelcomeMessageAsync(context);
            //context.Wait(MessageReceivedAsync);
        }

        private async Task WelcomeMessageAsync(IDialogContext context)
        {

            var reply = context.MakeMessage();

            string[] options = MessageOptions();

            reply.AddHeroCard(
                Resources.RootDialog_Welcome_Message,
                //Resources.RootDialog_Welcome_Subtitle,
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

        private Task OnOptionSelected(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }
    }
}