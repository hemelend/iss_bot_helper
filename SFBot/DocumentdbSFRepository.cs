using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace SFBot
{
    //public class DocumentdbSFRepository
    //{
    //    //Read config
    //    private static readonly string endpointUrl = ConfigurationManager.AppSettings["EndPointUrl"];
    //    private static readonly string authorizationKey = ConfigurationManager.AppSettings["AuthorizationKey"];
    //    private static readonly string databaseId = ConfigurationManager.AppSettings["DatabaseId"];
    //    private static readonly string collectionId = ConfigurationManager.AppSettings["CollectionId"];

    //    private static readonly ConnectionPolicy connectionPolicy = new ConnectionPolicy { UserAgentSuffix = " samples-net/2" };
    

    //    //The instance of a Database which we will be using for all the Collection operations being demo'd
    //    private static Database database;
    //}
    /// <summary>
    /// A generic repository that provides basic document operations.
    /// </summary>
    /// <typeparam name="TDocument">Type of the document.</typeparam>
    public class DocumentdbSFRepository<TDocument> where TDocument : Resource
    {
        
        //    private static readonly string endpointUrl = ConfigurationManager.AppSettings["EndPointUrl"];
        //    private static readonly string authorizationKey = ConfigurationManager.AppSettings["AuthorizationKey"];
        private static readonly string databaseId = ConfigurationManager.AppSettings["DatabaseId"];
        //    private static readonly string collectionId = ConfigurationManager.AppSettings["CollectionId"];

        //    private static readonly ConnectionPolicy connectionPolicy = new ConnectionPolicy { UserAgentSuffix = " samples-net/2" };

        //    //Reusable instance of DocumentClient which represents the connection to a DocumentDB endpoint
        private readonly DocumentClient documentClient;

        private readonly ICollectionProvider collectionProvider;

        // <summary>Initializes a new instance of the <see cref="DocumentdbSFRepository{TDocument}"/> class.</summary>
        // <param name="documentClient">DocumentDb Document Client</param>
        // <param name="databaseId">Database identifier</param>
        public DocumentdbSFRepository(DocumentClient documentClient, string databaseId) : this(
            documentClient,
            new GenericCollectionProvider<TDocument>(
                documentClient,
                new BasicDatabaseProvider(documentClient, databaseId)))
        {
        }

        // <summary>Initializes a new instance of the <see cref="DocumentdbSFRepository{TDocument}"/> class.</summary>
        // <param name="documentClient">DocumentDb Document Client</param>
        // <param name="collectionProvider">
        // Collection provider to obtain collection
        // where the documents are stored
        // </param>
        public DocumentdbSFRepository(
            DocumentClient documentClient,
            ICollectionProvider collectionProvider)
        {
            this.documentClient = documentClient;
            this.collectionProvider = collectionProvider;
        }

        // <summary>Creates document query using the <see cref="DocumentClient"/></summary>
        // <returns>Document query as <see cref="IOrderedQueryable{TDocument}"/></returns>
        public virtual async Task<IOrderedQueryable<TDocument>> CreateDocumentQuery()
        {
            return documentClient.CreateDocumentQuery<TDocument>(
                await this.collectionProvider.GetCollectionDocumentsLink());
        }

        // <summary>Gets all documents in the collection</summary>
        // <returns>
        // All documents in the collection as <see cref="IOrderedQueryable{TDocument}"/>
        // </returns>
        public virtual async Task<IOrderedQueryable<TDocument>> GetAll()
        {
            return await this.CreateDocumentQuery();
        }

        // <summary>Gets documents in the collection that meets the predicate</summary>
        // <param name="predicate">Predicate that the documents must meet</param>
        // <returns>
        // Documents that meet the prdicate as <see cref="IQueryable{TDocument}"/>
        // </returns>
        public virtual async Task<IQueryable<TDocument>> GetWhere(Expression<Func<TDocument, bool>> predicate)
        {
            return (await this.CreateDocumentQuery()).Where(predicate);
        }

        // <summary>Gets a document that meets the specified predicate.</summary>
        // <param name="predicate">Predicate that the document must meet</param>
        // <returns>
        // The document or null if such a document does not exist
        // </returns>
        public virtual async Task<TDocument> Get(Expression<Func<TDocument, bool>> predicate)
        {
            return (await this.GetWhere(predicate))
                .AsEnumerable()
                .FirstOrDefault();
        }

        // <summary>Gets a document by the specified DocumentDb identifier.</summary>
        // <param name="id">Document identifier.</param>
        // <returns>
        // The document or null if a document with the id does not exist
        // </returns>
        public virtual async Task<TDocument> Get(string id)
        {
            return await this.Get(d => d.Id == id);
        }

        // <summary>Creates the specified document.</summary>
        // <param name="document">Document to create</param>
        // <returns>
        // The created document, the new instance contains document id and resource
        // metadata so it is actually differnt from the given document instance.
        // </returns>
        public virtual async Task<TDocument> Create(TDocument document)
        {
            return (TDocument)(dynamic)(await documentClient.CreateDocumentAsync(
                await this.collectionProvider.GetCollectionDocumentsLink(),
                document))
                .Resource;
        }

        // <summary>
        // Updates the specified document. Uses the document self link to match
        // the object in database. If the link does not exist, the method tries to obtain
        // it from the collection by the document id. If the link cannot be retrieved,
        // throw <see cref="InvalidOperationException"/>.
        // </summary>
        // <param name="document">Document to update</param>
        // <returns>Updated document</returns>
        // <exception cref="System.InvalidOperationException">Document does not exist in collection</exception>
        public virtual async Task<TDocument> Update(TDocument document)
        {
            var selfLink = await this.GetDocumentSelfLink(document);

            if (string.IsNullOrWhiteSpace(selfLink))
            {
                throw new InvalidOperationException("Document does not exist in collection");
            }

            return (TDocument)(dynamic)(await documentClient.ReplaceDocumentAsync(
                selfLink,
                document))
                .Resource;
        }

        // <summary>
        // Deletes the specified document. Uses the document self link, 
        // see <seealso cref="Update(TDocument)"/>
        // </summary>
        // <param name="document">Document to delete</param>
        // <returns>Nothing</returns>
        // <exception cref="System.InvalidOperationException">Document does not exist in collection</exception>
        public virtual async Task Delete(TDocument document)
        {
            var selfLink = await this.GetDocumentSelfLink(document);

            if (string.IsNullOrWhiteSpace(selfLink))
            {
                throw new InvalidOperationException("Document does not exist in collection");
            }

            await documentClient.DeleteDocumentAsync(selfLink);
        }

        // <summary>Gets the document self link.</summary>
        // <param name="document">Document of which we want the self link</param>
        // <returns>Document self link</returns>
        protected virtual async Task<string> GetDocumentSelfLink(TDocument document)
        {
            return string.IsNullOrWhiteSpace(document.SelfLink)
                                  ? (await this.Get(document.Id))?.SelfLink
                                  : document.SelfLink;
        }
    }

}