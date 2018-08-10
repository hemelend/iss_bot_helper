using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using SFBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SFBot.Services
{
    public class SFDocDB : DocumentRepository
    {
        public static async Task<Document> CreateItemAsync(SFRequest sFRequest)
        {
            return await Client.CreateDocumentAsync(Collection.SelfLink, sFRequest);
        }

        public static SFRequest GetSFRequest(string id)
        {
            return Client.CreateDocumentQuery<SFRequest>(Collection.DocumentsLink)
                        .Where(d => d.Id == id)
                        .AsEnumerable()
                        .FirstOrDefault();
        }

        public static async Task<Document> UpdateNoteAsync(SFRequest sFRequest)
        {
            Document doc = Client.CreateDocumentQuery(Collection.DocumentsLink)
                                .Where(d => d.Id == sFRequest.Id)
                                .AsEnumerable()
                                .FirstOrDefault();

            return await Client.ReplaceDocumentAsync(doc.SelfLink, sFRequest);
        }

        public static Subsidiary GetSubsidiary(string name)
        {
            return Client.CreateDocumentQuery<Subsidiary>(Collection.DocumentsLink)
                        .Where(d => d.Name == name)
                        .AsEnumerable()
                        .FirstOrDefault();
        }

        public static async Task<IEnumerable<Subsidiary>> GetAllSubsidiaries(string region)
        {
            IDocumentQuery<Subsidiary> query = Client.CreateDocumentQuery<Subsidiary>(Collection.DocumentsLink)
                                                .Where(d => d.Region == region)
                                                .AsDocumentQuery();

            List<Subsidiary> subsidiaries = new List<Subsidiary>();

            while (query.HasMoreResults)
            {
                subsidiaries.AddRange(await query.ExecuteNextAsync<Subsidiary>());
            }

            return subsidiaries;
        }
    }
}