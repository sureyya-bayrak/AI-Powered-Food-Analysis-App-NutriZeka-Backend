using NutriZeka.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace NutriZeka.Application.Interfaces.Repositories
{
    public interface IAIAnalysisCacheRepository
    {
        // 🚀 Bütün sihir burada: Birebir aynı şartlarda sorulmuş bir soru var mı diye kontrol edeceğiz
        Task<AIAnalysisCache?> GetCachedAnalysisAsync(Guid userId, Guid productId, int questionType, bool isGlutenFree, bool isLactoseFree, bool isPalmOilFree);

        Task AddAsync(AIAnalysisCache cache);
        Task SaveChangesAsync();
    }
}