using System;
using Autofac;
using System.Web.Http;
using System.Reflection;
using System.Configuration;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Dialogs.Internals;


namespace SFBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            this.RegisterBotDependencies();

            //Mapper.Initialize(cfg => cfg.CreateMap<Models.SFRequest, Models.AreaRegion>());

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        private void RegisterBotDependencies()
        {
            Conversation.UpdateContainer(builder => {
                builder.RegisterModule(new ReflectionSurrogateModule());
                builder.RegisterModule<SFBotModule>();
                builder.RegisterModule(new AzureModule(Assembly.GetExecutingAssembly()));
                //builder.RegisterControllers(typeof(WebApiApplication).Assembly);

                Uri docDbEmulatorUri = new Uri(ConfigurationManager.AppSettings["EndPointUrl"]);
                string docDbEmulatorKey = ConfigurationManager.AppSettings["AuthorizationKey"];
                string docbdId = ConfigurationManager.AppSettings["DatabaseId"];

                // Bot Storage: Here we register the state storage for your bot. 
                // Default store: volatile in-memory store - Only for prototyping!
                // We provide adapters for Azure Table, CosmosDb, SQL Azure, or you can implement your own!
                // For samples and documentation, see: https://github.com/Microsoft/BotBuilder-Azure
                //var store = new InMemoryDataStore();

                // Other storage options
                // var store = new TableBotDataStore("...DataStorageConnectionString..."); // requires Microsoft.BotBuilder.Azure Nuget package 
                // var store = new DocumentDbBotDataStore("cosmos db uri", "cosmos db key"); // requires Microsoft.BotBuilder.Azure Nuget package 
                var store = new DocumentDbBotDataStore(docDbEmulatorUri, docDbEmulatorKey, docbdId);

                builder.Register(c => store)
                    .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                    .AsSelf()
                    .SingleInstance();

            });

            //DependencyResolver.SetResolver(new AutofacDependencyResolver(Conversation.Container));
        }
    }
}
