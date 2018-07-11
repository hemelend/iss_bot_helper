using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace SFBot
{
    public interface ICollectionProvider
    {
        /// <summary>
        /// Creates or gets the document collection.
        /// </summary>
        /// <returns>Document collection where the documents are stored</returns>
        Task<DocumentCollection> CreateOrGetCollection();

        /// <summary>
        /// Gets the collection documents link
        /// </summary>
        /// <returns>
        /// Collection documents link of the collection that is created b
        /// <see cref="CreateOrGetCollection"/>
        /// </returns>
        Task<String> GetCollectionDocumentsLink();
    }
}
