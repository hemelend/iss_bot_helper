using Microsoft.Azure.Documents;
using System.Threading.Tasks;

namespace SFBot
{
    public interface IDatabaseProvider
    {
        /// <summary>Creates or gets the DocumentDb database.</summary>
        /// <returns>DocumentDb database</returns>
        Task<Database> CreateOrGetDb();

        /// <summary>Gets the DocumentDb database self link.</summary>
        /// <returns>DocumentDb database self link</returns>
        Task<string> GetDbSelfLink();
    }
}