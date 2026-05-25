using NutriZeka.Application.DTOs;

namespace NutriZeka.Application.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(UserProfileDto user);
    }
}