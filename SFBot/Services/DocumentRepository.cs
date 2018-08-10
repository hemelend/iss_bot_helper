using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using SFBot.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SFBot.Services
{
    public class DocumentRepository
    {
        /// <summary>  
        /// Retrieve the Database ID to use from the Web Config  
        /// </summary>  
        private static string databaseId;
        private static String DatabaseId
        {
            get
            {
                if (string.IsNullOrEmpty(databaseId))
                {
                    databaseId = ConfigurationManager.AppSettings["DatabaseId"];
                }
                return databaseId;
            }
        }

        /// <summary>  
        /// Retrieves the Collection to use from Web Config  
        /// </summary>  
        private static string collectionId;
        private static String CollectionId
        {
            get
            {
                if (string.IsNullOrEmpty(collectionId))
                {
                    collectionId = ConfigurationManager.AppSettings["SFCollectionId"];
                }
                return collectionId;
            }
        }

        private static DocumentClient client;
        public static DocumentClient Client
        {
            get
            {
                if (client == null)
                {
                    string endpoint = ConfigurationManager.AppSettings["EndPointUrl"];
                    string authKey = ConfigurationManager.AppSettings["AuthorizationKey"];
                    Uri endpointUri = new Uri(endpoint);
                    client = new DocumentClient(endpointUri, authKey);
                }
                return client;
            }
        }

        private static Database database;
        private static Database Database
        {
            get
            {
                if (database == null)
                {
                    database = ReadOrCreateDatabase();
                }
                return database;
            }
        }

        private static Database ReadOrCreateDatabase()
        {
            var db = Client.CreateDatabaseQuery()
                            .Where(d => d.Id == DatabaseId)
                            .AsEnumerable()
                            .FirstOrDefault();

            if (db == null)
            {
                db = Client.CreateDatabaseAsync(new Database { Id = DatabaseId }).Result;
            }
            return db;
        }

        private static DocumentCollection collection;
        public static DocumentCollection Collection
        {
            get
            {
                if (collection == null)
                {
                    collection = ReadOrCreateCollection(Database.SelfLink);
                }

                return collection;
            }
        }

        private static DocumentCollection ReadOrCreateCollection(string databaseLink)
        {

            var col = Client.CreateDocumentCollectionQuery(databaseLink)
                              .Where(c => c.Id == CollectionId)
                              .AsEnumerable()
                              .FirstOrDefault();

            if (col == null)
            {
                col = Client.CreateDocumentCollectionAsync(databaseLink, new DocumentCollection { Id = CollectionId }).Result;
            }
            return col;
        }
    }
}