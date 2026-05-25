using System.Collections.Generic;
using System.Threading.Tasks;
using NutriZeka.Domain.Entities;

namespace NutriZeka.Application.Interfaces
{
    public interface IProductService
    {
        // Tekli dönen (Eski yazdığımız)
        Task<Product> GetBetterAlternativeAsync(string barcode);

        // LİSTE DÖNEN (YENİ EKLENEN)
        Task<IEnumerable<Product>> GetBetterAlternativesAsync(string barcode);
    }
}