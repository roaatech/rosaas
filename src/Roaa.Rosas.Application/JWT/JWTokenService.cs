using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Roaa.Rosas.Application.IdentityServer4;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain;
using Roaa.Rosas.Domain.Entities;
using Roaa.Rosas.Domain.Entities.Identity;
using Roaa.Rosas.Domain.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Roaa.Rosas.Application.JWT
{
    public class JWTokenService : IAuthTokenService
    {
        #region Props     
        private readonly ILogger<JWTokenService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IClientStore _clientStore;
        private readonly IIdentityServerPersistedGrantDbContext _identityServerPersistedGrantDbContext;
        private readonly IIdentityServerConfigurationDbContext _identityServerConfigurationDbContext;
        private readonly IRosasDbContext _identityDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IdentityServerTools _identityServerTools;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly List<Claim> _claims;
        #endregion

        #region Ctors
        public JWTokenService(UserManager<User> userManager,
                               SignInManager<User> signInManager,
                               IClientStore clientStore,
                               IRosasDbContext identityDbContext,
                              IIdentityServerPersistedGrantDbContext identityServerPersistedGrantDbContext,
                               IIdentityServerConfigurationDbContext identityServerConfigurationDbContext,
                               IHttpContextAccessor httpContextAccessor,
                               IdentityServerTools identityServerTools,
                               IRefreshTokenService refreshTokenService,
                               ILogger<JWTokenService> logger
                             )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _clientStore = clientStore;
            _identityDbContext = identityDbContext;
            _identityServerPersistedGrantDbContext = identityServerPersistedGrantDbContext;
            _identityServerConfigurationDbContext = identityServerConfigurationDbContext;
            _httpContextAccessor = httpContextAccessor;
            _identityServerTools = identityServerTools;
            _refreshTokenService = refreshTokenService;
            _logger = logger;
            _claims = new List<Claim>();
        }
        #endregion

        public async Task<Result<TokenModel>> GenerateAsync(Guid userId, string clientId, AuthenticationMethod authenticationMethod)
        {
            var user = await _identityDbContext.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId));
            return await GenerateAsync(user, clientId, authenticationMethod);
        }

        public async Task<Result<TokenModel>> GenerateAsync(User user, string clientId, AuthenticationMethod authenticationMethod)
        {

            var client = await FindClientByIdAsync(clientId);

            // add Jwt Id 
            AddJwtId(out string jwtId);

            // add sub 
            AddSubject(user.Id);

            // add auth_time
            AddAuthenticationTime();

            // add idp
            AddIdentityProvider();

            // add client_id
            AddClientId(client);

            AddAuthMethod(authenticationMethod);

            AddTokenSign(TokenSign.SuperAdminUser);

            TryAddEmail(user.Email);

            TryAddPhoneNumber(user.PhoneNumber);

            // add audiences
            await AddAudiencesAsync(client);

            // add /resources to aud so the client can get user profile info.
            var url = _httpContextAccessor.HttpContext.Request.Host.Host;
            _claims.Add(new Claim(JwtClaimTypes.Audience, $"{url}/resources"));


            //scopes for the user
            AddAllowedScopes(client);

            var token = await IssueJwtAsync(client);

            await CreatePersistedGrantAsync(token, jwtId, clientId, user.Id, GetExpiration(client), _claims, authenticationMethod);

            var refreshToken = await CreateRefreshTokenAsync(client, user);

            var model = new TokenModel
            {
                AccessToken = token,
                RefreshToken = refreshToken,
            };

            return Result<TokenModel>.Successful(model);
        }

        public async Task<Result<TokenModel>> GenerateAsync(string clientId)
        {
            var client = await FindClientByIdAsync(clientId);
            return await GenerateAsync(client);
        }

        public async Task<Result<TokenModel>> GenerateAsync(Client client)
        {
            var Properties = client.Properties.ToList();
            var clientClaims = client.Claims.ToList();

            // add Jwt Id 
            AddJwtId(out string jwtId);

            // add iat
            AddIssuedAt();

            // add idp
            AddIdentityProvider();

            // add client_id
            AddClientId(client);

            var productId = Properties.First(x => x.Key.Equals(SystemConsts.Clients.Properties.RosasProductId, StringComparison.OrdinalIgnoreCase)).Value;
            _claims.Add(new Claim(SystemConsts.Clients.Claims.ClaimProductId, productId));

            AddAuthMethod(AuthenticationMethod.ClientCredentials);

            AddTokenSign(TokenSign.ExternalSystemClient);

            foreach (var claim in clientClaims)
            {
                _claims.Add(new Claim($"{claim.Type}", claim.Value));
            }

            // add audiences
            await AddAudiencesAsync(client);

            //scopes for the user
            AddAllowedScopes(client);

            var token = await IssueJwtAsync(client);

            await CreatePersistedGrantAsync(token, jwtId, client.ClientId, new Guid(productId), GetExpiration(client), _claims, AuthenticationMethod.ClientCredentials);

            var model = new TokenModel
            {
                AccessToken = token,
            };

            return Result<TokenModel>.Successful(model);
        }

        #region

        private async Task CreatePersistedGrantAsync(string jwtoken, string jwtId, string clientId, Guid subjectId, DateTime expiration, List<Claim> claims, AuthenticationMethod authenticationMethod)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(jwtoken);
            var jwtSecurityToken = jsonToken as JwtSecurityToken;


            var json = JsonConvert.SerializeObject(jwtSecurityToken.Payload);
            JObject payload = JObject.Parse(json);


            if (payload.TryGetValue("aud", StringComparison.OrdinalIgnoreCase, out JToken value))
            {
                if (value.Contains("[") && value.Contains("]"))
                {
                    JArray aud = (JArray)payload["aud"];
                    aud.RemoveAll();
                    jwtSecurityToken.Payload.Aud.ToList().ForEach(x => { aud.Add(x); });
                }
            }

            if (payload.TryGetValue("scope", StringComparison.OrdinalIgnoreCase, out JToken value2))
            {
                JArray scope = (JArray)payload["scope"];
                scope.RemoveAll();
                claims.Where(x => x.Type == JwtClaimTypes.Scope).ToList().ForEach(x => { scope.Add(x.Value); });
            }

            PersistedUserGrant persistedGrant = new PersistedUserGrant
            {
                Key = jwtId,
                AuthenticationMethod = authenticationMethod.ToString(),
                UserId = subjectId,
                ClientId = clientId,
                IsActive = true,
                IssuedAt = DateTime.UtcNow,
                Expiration = expiration,
                Type = "access_token",
                MetaData = JsonConvert.SerializeObject(payload),
            };

            _identityServerPersistedGrantDbContext.UserPersistedGrants.Add(persistedGrant);
            await _identityServerPersistedGrantDbContext.SaveChangesAsync();
        }
        private async Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = await _clientStore.FindClientByIdAsync(clientId);
            if (client == null)
                _logger.LogError($"clientId:{clientId} not found while token generating");
            else
                _logger.LogInformation($"Token generating for clientId:{clientId}, clientName:{client.ClientName}");

            return client;
        }
        private async Task<string> CreateRefreshTokenAsync(Client client, User user)
        {
            var principal = await _signInManager.CreateUserPrincipalAsync(user);

            Token t = new Token
            {
                ClientId = client.ClientId,
                Claims = _claims,
            };
            return await _refreshTokenService.CreateRefreshTokenAsync(principal, t, client);
        }
        private async Task<string> IssueJwtAsync(Client client)
        {
            var expiration = DateTime.UtcNow.AddSeconds(client.AccessTokenLifetime);
            TimeSpan lifetime = expiration - DateTime.UtcNow;
            return await _identityServerTools.IssueJwtAsync((int)lifetime.TotalSeconds, _claims);
        }
        private async Task AddAudiencesAsync(Client client)
        {
            // add audiences
            var audiences = client.AllowedScopes.Where(s => s != IdentityServerConstants.StandardScopes.OfflineAccess &&
                                                            s != IdentityServerConstants.StandardScopes.OpenId &&
                                                            s != IdentityServerConstants.StandardScopes.Profile);
            if (audiences.Any())
            {
                var apiResourceScopes = await _identityServerConfigurationDbContext.ApiResourceScopes
                                                                                   .Include(x => x.ApiResource)
                                                                                   .Where(x => audiences.Contains(x.Scope)).ToListAsync();
                foreach (var audValue in apiResourceScopes)
                {
                    _claims.Add(new Claim(JwtClaimTypes.Audience, audValue.ApiResource.Name));
                }
            }
        }
        private void AddAllowedScopes(Client client)
        {
            //scopes for the user
            foreach (var scopeValue in client.AllowedScopes)
            {
                _claims.Add(new Claim(JwtClaimTypes.Scope, scopeValue));
            }
        }
        private void TryAddEmail(string email)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                _claims.Add(new Claim(JwtClaimTypes.Email, email));
            }
        }
        private void TryAddPhoneNumber(string phoneNumber)
        {
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                _claims.Add(new Claim(JwtClaimTypes.PhoneNumber, phoneNumber));
            }
        }
        private void AddTokenSign(TokenSign sign)
        {
            // add client_id
            _claims.Add(new Claim(SystemConsts.Clients.Claims.ClaimSign, $"{(int)sign}"));
        }
        private void AddClientId(Client client)
        {
            // add client_id
            _claims.Add(new Claim(JwtClaimTypes.ClientId, client.ClientId));
        }
        private void AddAuthMethod(AuthenticationMethod authenticationMethod)
        {
            // auth_method
            _claims.Add(new Claim(SystemConsts.Clients.Claims.ClaimAuthenticationMethod, authenticationMethod.ToSnakeCaseNamingStrategy()));
        }
        private void AddIdentityProvider()
        {
            // add idp
            _claims.Add(new Claim(JwtClaimTypes.IdentityProvider, "ROSAS"));
        }
        private void AddAuthenticationTime()
        {
            // add auth_time
            _claims.Add(new Claim(JwtClaimTypes.AuthenticationTime, $"{(int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds}"));
        }
        private void AddIssuedAt()
        {
            // add auth_time
            _claims.Add(new Claim(JwtClaimTypes.IssuedAt, $"{(int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds}"));
        }
        private void AddSubject(Guid sub)
        {
            // add sub 
            _claims.Add(new Claim(JwtClaimTypes.Subject, sub.ToString()));
        }
        private void AddJwtId(out string jwtId)
        {
            jwtId = Guid.NewGuid().ToString().Replace("-", "").ToUpper();

            // add Jwt Id 
            _claims.Add(new Claim(JwtClaimTypes.JwtId, jwtId));
        }
        private DateTime GetExpiration(Client client)
        {
            return DateTime.UtcNow.AddSeconds(client.AccessTokenLifetime);
        }

        #endregion
    }
}
