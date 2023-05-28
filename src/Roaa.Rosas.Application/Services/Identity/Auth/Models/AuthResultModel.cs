using Roaa.Rosas.Domain;

namespace Roaa.Rosas.Application.Services.Identity.Auth.Models
{
    public class AuthResultModel
    {
        public UserAccountDto? UserAccount { get; set; }
        public TokenModel? Token { get; set; }
    }


    public class AuthResultModel<T> : AuthResultModel
    {
        //  public T? Details { get; set; } 
    }
}
