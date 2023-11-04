using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interface;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;

namespace FreeCourse.Web.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClientSettings _clientSettings;
        private readonly ServiceApiSettings _serviceApiSettings;

        public IdentityService(HttpClient httpClient, IHttpContextAccessor contextAccessor, IOptions<ClientSettings> clientSettings, IOptions<ServiceApiSettings> serviceApiSettings)
        {
            _httpClient = httpClient;
            _httpContextAccessor = contextAccessor;
            _clientSettings = clientSettings.Value;
            _serviceApiSettings = serviceApiSettings.Value;
        }

        public async Task<TokenResponse> GetAcessTokenByRefreshToken()
        {
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });

            if (disco.IsError)
            {
                throw disco.Exception;
            }
            var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);//cookiden refresh tokenı aldıdk
            RefreshTokenRequest refreshTokenRequest = new RefreshTokenRequest()
            {
                ClientId = _clientSettings.WebClientForUser.ClientId,
                ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
                RefreshToken = refreshToken,
                Address = disco.TokenEndpoint
            };

            var token = await _httpClient.RequestRefreshTokenAsync(refreshTokenRequest);//yeni bir token alıyoruz
            if (token.IsError) //token bir hatası varsa alamadıysak yani token
            {
                return null; //istersen burada loglama yap
            }
            var authenticationTokens=new List<AuthenticationToken>()
            {
                new AuthenticationToken{Name = OpenIdConnectParameterNames.AccessToken,Value=token.AccessToken},
                new AuthenticationToken{Name = OpenIdConnectParameterNames.RefreshToken,Value=token.AccessToken},
                new AuthenticationToken{Name = OpenIdConnectParameterNames.ExpiresIn,Value=DateTime.Now.AddSeconds(token.ExpiresIn).ToString("O",CultureInfo.InvariantCulture)},
            };

            var authenticationResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();//elimizdekileri alıyoruz
            var properties = authenticationResult.Properties;
            properties.StoreTokens(authenticationTokens);//bizden bir authenticationteken istedi verdik
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, authenticationResult.Principal,properties);//şema ismi istiyor parantezde daha sonra claim istiyor
            return token;
        
        }

        public async Task RevokeRefreshToken()
        {
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });

            if (disco.IsError)
            {
                throw disco.Exception;
            }
            var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            TokenRevocationRequest tokenRevocationRequest = new()
            {
                ClientId = _clientSettings.WebClientForUser.ClientId,
                ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
                Address = disco.RevocationEndpoint,
                Token = refreshToken,
                TokenTypeHint = "refresh_token"
            };
            await _httpClient.RevokeTokenAsync(tokenRevocationRequest);
        }

        public async Task<Response<bool>> SignIn(SignInInput signInInput)
        {
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });

            if (disco.IsError)
            {
                throw disco.Exception;
            }

            var passwordTokenRequest = new PasswordTokenRequest
            {
                ClientId = _clientSettings.WebClientForUser.ClientId,
                ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
                UserName = signInInput.Email,
                Password = signInInput.Password,
                Address = disco.TokenEndpoint //istek nereyaa yapılacagı token end pointe yap
            };

            var token = await _httpClient.RequestPasswordTokenAsync(passwordTokenRequest);
            if (token.IsError)
            {
                var responseContent = await token.HttpResponse.Content.ReadAsStringAsync();
                var errorDto = JsonSerializer.Deserialize<ErrorDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return Response<bool>.Fail(errorDto.Errors, 404);
            }
            var userInfoRequest = new UserInfoRequest
            {
                Token = token.AccessToken,
                Address = disco.UserInfoEndpoint
            };

            var userInfo = await _httpClient.GetUserInfoAsync(userInfoRequest);
            if (userInfo.IsError)
            {

                throw userInfo.Exception;
            }

            ClaimsIdentity claimsIdentity = new ClaimsIdentity
                (userInfo.Claims, CookieAuthenticationDefaults.AuthenticationScheme,"name","role");

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authenticationProperties = new AuthenticationProperties();
            authenticationProperties.StoreTokens(new List<AuthenticationToken>(){
                new AuthenticationToken{Name = OpenIdConnectParameterNames.AccessToken,Value=token.AccessToken},
                new AuthenticationToken{Name = OpenIdConnectParameterNames.RefreshToken,Value=token.AccessToken},
                new AuthenticationToken{Name = OpenIdConnectParameterNames.ExpiresIn,Value=DateTime.Now.AddSeconds(token.ExpiresIn).ToString("O",CultureInfo.InvariantCulture)},
            });

            authenticationProperties.IsPersistent = signInInput.IsRemember;
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme,authenticationProperties);

            return Response<bool>.Success(200);

            
        }
    }
}
