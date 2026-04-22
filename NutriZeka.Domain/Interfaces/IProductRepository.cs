using NutriZeka.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NutriZeka.Domain.Interfaces
{
    public interface IProductRepository
    {
        // 1. Kamera için: Barkodu tam okuduğunda tek ürün getirir (Tam eşleşme)
        Task<Product?> GetByBarcodeAsync(string barcode);

        // 2. Arama çubuğu için: Hem isme göre hem de barkodun "başlangıcına" göre liste getirir
        Task<IEnumerable<Product>> SearchAsync(string searchTerm);

        // 3. Admin ve diğer listelemeler için
        Task<IEnumerable<Product>> GetAllAsync();

        // 4. CRUD işlemleri
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Guid id);
    }
}