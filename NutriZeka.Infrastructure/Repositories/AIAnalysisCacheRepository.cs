using Microsoft.EntityFrameworkCore;
using NutriZeka.Application.Interfaces.Repositories;
using NutriZeka.Domain.Entities;
using NutriZeka.Infrastructure.Context; // DbContext'inin olduğu namespace
using System;
using System.Threading.Tasks;

namespace NutriZeka.Infrastructure.Persistence.Repositories
{
    public class AIAnalysisCacheRepository : IAIAnalysisCacheRepository
    {
        private readonly ApplicationDbContext _context;

        public AIAnalysisCacheRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AIAnalysisCache?> GetCachedAnalysisAsync(Guid userId, Guid productId, int questionType, bool isGlutenFree, bool isLactoseFree, bool isPalmOilFree)
        {
            // Kullanıcı, ürün, soru tipi ve "kullanıcının o anki sağlık tercihleri" tamamen eşleşmeli.
            return await _context.AIAnalysisCaches
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.ProductId == productId &&
                    x.QuestionType == questionType &&
                    x.UserWasGlutenFree == isGlutenFree &&
                    x.UserWasLactoseFree == isLactoseFree &&
                    x.UserWasPalmOilFree == isPalmOilFree);
        }

        public async Task AddAsync(AIAnalysisCache cache)
        {
            await _context.AIAnalysisCaches.AddAsync(cache);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}