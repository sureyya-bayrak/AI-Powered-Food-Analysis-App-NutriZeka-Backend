using System.Threading.Tasks;

namespace NutriZeka.Application.Interfaces.Services
{
    public interface IGenerativeAiService
    {
        // Sadece metni gönderip cevabı alacağımız arayüz
        Task<string> GenerateResponseAsync(string prompt);
    }
}