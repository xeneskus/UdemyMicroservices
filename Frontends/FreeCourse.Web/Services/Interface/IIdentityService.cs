using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models;
using IdentityModel.Client;

namespace FreeCourse.Web.Services.Interface
{
    public interface IIdentityService
    {
        Task<Response<bool>> SignIn(SignInInput signInInput);
        Task<TokenResponse> GetAcessTokenByRefreshToken(); 
        Task RevokeRefreshToken();
    }
}
