using NutriZeka.Application.DTOs;
using System;
using System.Threading.Tasks;

namespace NutriZeka.Application.Interfaces.Services
{
    public interface IAIAnalysisService
    {
        Task<string> GetAnalysisAsync(Guid userId, AIAnalysisRequestDto request);
    }
}