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
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain;
using Roaa.Rosas.Domain.Entities;
using Roaa.Rosas.Domain.Entities.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Roaa.Rosas.Application.Services.Identity.Auth
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
        }
        #endregion


        public async Task<Result<TokenModel>> GenerateAsync(Guid userId, string clientId, AuthenticationMethod authenticationMethod)
        {
            var user = await _identityDbContext.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId));
            return await GenerateAsync(user, clientId, authenticationMethod);
        }
        public async Task<Result<TokenModel>> GenerateAsync(User user, string clientId, AuthenticationMethod authenticationMethod)
        {
            var principal = await _signInManager.CreateUserPrincipalAsync(user);
            var client = await _clientStore.FindClientByIdAsync(clientId);
            if (client == null)
                _logger.LogError($"clientId:{clientId} not found");
            else
                _logger.LogInformation($"clientId:{clientId}, clientName:{client.ClientName}");

            var claims = new List<Claim>();

            var jwtId = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
            // add Jwt Id 
            claims.Add(new Claim(JwtClaimTypes.JwtId, jwtId));
            // add sub 
            claims.Add(new Claim(JwtClaimTypes.Subject, user.Id.ToString()));

            // add auth_time
            claims.Add(new Claim(JwtClaimTypes.AuthenticationTime, $"{(int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds}"));
            // add idp
            claims.Add(new Claim(JwtClaimTypes.IdentityProvider, "Rosas"));

            claims.Add(new Claim("auth_method", authenticationMethod.ToString()));

            // add client_id
            claims.Add(new Claim(JwtClaimTypes.ClientId, clientId));

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
                    claims.Add(new Claim(JwtClaimTypes.Audience, audValue.ApiResource.Name));
                }
            }

            // add /resources to aud so the client can get user profile info.
            var url = _httpContextAccessor.HttpContext.Request.Host.Host;
            claims.Add(new Claim(JwtClaimTypes.Audience, $"{url}/resources"));


            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                claims.Add(new Claim(JwtClaimTypes.Email, user.Email));
            }

            if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                claims.Add(new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber));
            }

            var userTypeManager = UserTypeManager.FromKey(user.UserType);

            userTypeManager.SetSpecificationsClaims(user, claims);

            //scopes for the user
            foreach (var scopeValue in client.AllowedScopes)
            {
                claims.Add(new Claim(JwtClaimTypes.Scope, scopeValue));
            }
            userTypeManager.TrySetScopes(user, claims);

            var expiration = DateTime.UtcNow.AddSeconds(client.AccessTokenLifetime);
            TimeSpan lifetime = expiration - DateTime.UtcNow;
            var token = await _identityServerTools.IssueJwtAsync((int)lifetime.TotalSeconds, claims);



            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var jwtSecurityToken = jsonToken as JwtSecurityToken;


            var json = JsonConvert.SerializeObject(jwtSecurityToken.Payload);
            JObject payload = JObject.Parse(json);


            if (payload.TryGetValue("aud", StringComparison.OrdinalIgnoreCase, out JToken value))
            {
                JArray aud = (JArray)payload["aud"];
                aud.RemoveAll();
                jwtSecurityToken.Payload.Aud.ToList().ForEach(x => { aud.Add(x); });
            }

            if (payload.TryGetValue("scope", StringComparison.OrdinalIgnoreCase, out JToken value2))
            {
                JArray scope = (JArray)payload["scope"];
                scope.RemoveAll();
                claims.Where(x => x.Type == JwtClaimTypes.Scope).ToList().ForEach(x => { scope.Add(x.Value); });
            }

            if (payload.TryGetValue("specification", StringComparison.OrdinalIgnoreCase, out JToken value3))
            {
                JArray specification = (JArray)payload["specification"];
                specification.RemoveAll();
                claims.Where(x => x.Type == "specification").ToList().ForEach(x => { specification.Add(x.Value); });
            }

            PersistedUserGrant persistedGrant = new PersistedUserGrant
            {
                Key = jwtId,
                AuthenticationMethod = authenticationMethod.ToString(),
                UserId = user.Id,
                ClientId = client.ClientId,
                IsActive = true,
                IssuedAt = DateTime.UtcNow,
                Expiration = expiration,
                Type = "access_token",
                MetaData = JsonConvert.SerializeObject(payload),
            };




            _identityServerPersistedGrantDbContext.UserPersistedGrants.Add(persistedGrant);
            await _identityServerPersistedGrantDbContext.SaveChangesAsync();


            Token t = new Token
            {
                ClientId = client.ClientId,
                Claims = claims,
            };
            var refreshToken = await _refreshTokenService.CreateRefreshTokenAsync(principal, t, client);

            var model = new TokenModel
            {
                AccessToken = token,
                RefreshToken = refreshToken,
            };

            return Result<TokenModel>.Successful(model);
        }


    }
}
