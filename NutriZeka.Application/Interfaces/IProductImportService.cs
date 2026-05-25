using System.IO;
using System.Threading.Tasks;

namespace NutriZeka.Application.Interfaces
{
    public interface IProductImportService
    {
        // Bu metot bir dosya akışı (stream) alacak, kaç ürün eklendiğini/güncellendiğini dönecek.
        Task<(int AddedCount, int UpdatedCount, string Message)> ImportFromCsvAsync(Stream fileStream);
    }
}