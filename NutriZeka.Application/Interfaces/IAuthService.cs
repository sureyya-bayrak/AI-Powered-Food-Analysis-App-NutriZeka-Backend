using System.Threading.Tasks;

namespace NutriZeka.Application.Interfaces
{
    public interface IAuthService
    {
        // Google Token takası için
        Task<string> GoogleLoginAsync(string idToken);

        // Dünkü standart e-posta/şifre girişi için
        Task<string> LoginAsync(string email, string password);
    }
}