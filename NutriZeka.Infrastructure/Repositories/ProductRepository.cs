using Microsoft.EntityFrameworkCore;
using NutriZeka.Domain.Entities;
using NutriZeka.Domain.Interfaces;
using NutriZeka.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NutriZeka.Infrastructure.Repositories
{
    // IProductRepository (Menü) sözleşmesini imzaladığımızı belirtiyoruz
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        // Ocağı (ApplicationDbContext) mutfağa alıyoruz
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Kameradan gelen tam barkod okuması (Sadece 1 ürün döner)
        public async Task<Product?> GetByBarcodeAsync(string barcode)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Barcode == barcode);
        }

        // 2. Senin harika UX tespitin: Hem isim hem de barkod başlangıcına göre canlı arama! (Liste döner)
        // Senin harika UX tespitin: Hem Türkçe isim, hem İngilizce isim, hem Marka hem de barkod başlangıcına göre devasa canlı arama!
        public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
        {
            return await _context.Products
                .Where(p =>
                       p.NameTr.Contains(searchTerm) ||
                       p.NameEn.Contains(searchTerm) ||
                       p.Brand.Contains(searchTerm) ||
                       p.Barcode.StartsWith(searchTerm))
                .ToListAsync();
        }

        // 3. Admin listelemeleri veya genel sorgular için tüm ürünler
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        // 4. Yeni ürün ekleme (RAM'e Add yapıp, SaveChanges ile MSSQL'e kalıcı yazıyoruz)
        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        // 5. Ürün güncelleme (Örn: Nutri-Score değişirse)
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        // 6. Ürün silme
        public async Task DeleteAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}