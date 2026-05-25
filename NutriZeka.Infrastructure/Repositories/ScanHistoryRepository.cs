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
    public class ScanHistoryRepository : IScanHistoryRepository
    {
        private readonly ApplicationDbContext _context;

        public ScanHistoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ScanHistory>> GetUserHistoriesAsync(Guid userId)
        {
            return await _context.ScanHistories
                .Include(h => h.Product)
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }

        // --- YENİ EKLENEN KISIMLAR ---
        public async Task<IEnumerable<ScanHistory>> GetFavoritesAsync(Guid userId)
        {
            return await _context.ScanHistories
                .Include(h => h.Product)
                .Where(h => h.UserId == userId && h.IsFavorite)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ScanHistory>> GetSavedAsync(Guid userId)
        {
            return await _context.ScanHistories
                .Include(h => h.Product)
                .Where(h => h.UserId == userId && h.IsSaved)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }

        public async Task<ScanHistory?> GetByIdAsync(Guid id)
        {
            return await _context.ScanHistories
                .Include(h => h.Product)
                .FirstOrDefaultAsync(h => h.Id == id);
        }
        // ----------------------------

        public async Task<ScanHistory?> GetHistoryByUserAndProductAsync(Guid userId, Guid productId)
        {
            return await _context.ScanHistories
                .Include(h => h.Product) // Detay sayfasında ürün adı lazım olduğu için Include ekledik
                .FirstOrDefaultAsync(h => h.UserId == userId && h.ProductId == productId);
        }

        public async Task AddAsync(ScanHistory history)
        {
            await _context.ScanHistories.AddAsync(history);
        }

        public void Update(ScanHistory history)
        {
            // EF Core'un nesneyi takip ettiğinden ve durumunun 'Modified' olduğundan emin oluyoruz
            _context.Entry(history).State = EntityState.Modified;
            _context.ScanHistories.Update(history);
        }

        public void RemoveRange(IEnumerable<ScanHistory> histories)
        {
            _context.ScanHistories.RemoveRange(histories);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
         public async Task<IEnumerable<ScanHistory>> GetAllAsync()
        {
            return await _context.ScanHistories
                .Include(h => h.Product)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        } // Bunu repository içine ekle
        public async Task<IEnumerable<ScanHistory>> GetAllHistoriesAsync()
        {
            return await _context.ScanHistories
                .Include(h => h.Product)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }
      
    }
}