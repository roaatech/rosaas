namespace Roaa.Rosas.Application.Services.IdentityServer4.Auth.Models
{
    public record AuthResultModel
    {
        public ClientInfoDto? Info { get; set; }
        public TokenModel? Token { get; set; }
    }

    public record TokenModel
    {
        public string AccessToken { get; set; }
    }
}
