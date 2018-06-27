using System.Web.Http;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Autofac;

using Microsoft.Bot.Connector;
using System.Reflection;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace SFBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            this.RegisterBotDependencies();

            //Mapper.Initialize(cfg => cfg.CreateMap<Models.SFRequest, Models.AreaRegion>());

            GlobalConfiguration.Configure(WebApiConfig.Register);

            //Conversation.UpdateContainer(
            //builder =>
            //{
            //    builder.RegisterModule(new AzureModule(Assembly.GetExecutingAssembly()));
                

            //    // Bot Storage: Here we register the state storage for your bot. 
            //    // Default store: volatile in-memory store - Only for prototyping!
            //    // We provide adapters for Azure Table, CosmosDb, SQL Azure, or you can implement your own!
            //    // For samples and documentation, see: [https://github.com/Microsoft/BotBuilder-Azure](https://github.com/Microsoft/BotBuilder-Azure)
            //    var store = new InMemoryDataStore();

            //    // Other storage options
            //    // var store = new TableBotDataStore("...DataStorageConnectionString..."); // requires Microsoft.BotBuilder.Azure Nuget package 
            //    // var store = new DocumentDbBotDataStore("cosmos db uri", "cosmos db key"); // requires Microsoft.BotBuilder.Azure Nuget package 

            //    builder.Register(c => store)
            //        .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
            //        .AsSelf()
            //        .SingleInstance();

                
            //});
            
        }

        private void RegisterBotDependencies()
        {
            Conversation.UpdateContainer(builder => {
                builder.RegisterModule(new ReflectionSurrogateModule());
                builder.RegisterModule<SFBotModule>();
                //builder.RegisterControllers(typeof(WebApiApplication).Assembly);


                builder.RegisterModule(new AzureModule(Assembly.GetExecutingAssembly()));

                // Bot Storage: Here we register the state storage for your bot. 
                // Default store: volatile in-memory store - Only for prototyping!
                // We provide adapters for Azure Table, CosmosDb, SQL Azure, or you can implement your own!
                // For samples and documentation, see: https://github.com/Microsoft/BotBuilder-Azure
                var store = new InMemoryDataStore();

                // Other storage options
                // var store = new TableBotDataStore("...DataStorageConnectionString..."); // requires Microsoft.BotBuilder.Azure Nuget package 
                // var store = new DocumentDbBotDataStore("cosmos db uri", "cosmos db key"); // requires Microsoft.BotBuilder.Azure Nuget package 

                builder.Register(c => store)
                    .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                    .AsSelf()
                    .SingleInstance();
            });

            //DependencyResolver.SetResolver(new AutofacDependencyResolver(Conversation.Container));
        }
    }
}
