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

        public async Task<IEnumerable<Product>> SearchAsync(string searchTerm, string language)
        {
            var term = searchTerm.Trim().ToLower();
            var query = _context.Products.AsQueryable();

            if (language.StartsWith("en"))
            {
                // İNGİLİZCE: Sadece NameEn ve Barcode'da ara
                query = query.Where(p =>
                    (p.NameEn != null && p.NameEn.ToLower().Contains(term)) ||
                    (p.Barcode != null && p.Barcode.Contains(term)) // Barcode küçük/büyük harf önemsiz
                );
            }
            else
            {
                // TÜRKÇE: Sadece NameTr ve Barcode'da ara
                query = query.Where(p =>
                    (p.NameTr != null && p.NameTr.ToLower().Contains(term)) ||
                    (p.Barcode != null && p.Barcode.Contains(term))
                );
            }

            return await query.Take(50).ToListAsync(); // İlk 50 sonuç
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
        // --- YENİ EKLENEN KISIM: ID'ye göre ürün getirme (AI Servisi için gerekli) ---
        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products.FindAsync(id);
        }
        // --- YENİ EKLENEN KISIM ---
        // Servisin filtreleme yapabilmesi için IQueryable dönüyoruz
        public IQueryable<Product> GetQueryable()
        {
            return _context.Products.AsQueryable();
        }
    }
}