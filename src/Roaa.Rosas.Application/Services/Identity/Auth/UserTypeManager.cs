using IdentityModel;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Utilities;
using Roaa.Rosas.Domain.Entities.Identity;
using System.Security.Claims;

namespace Roaa.Rosas.Application.Services.Identity.Auth
{
    public abstract class UserTypeManager : Enumeration<UserTypeManager, UserType>
    {
        #region Props
        public static readonly UserTypeManager SuperAdmin = new SuperAdminUser();
        #endregion

        #region Corts
        protected UserTypeManager(UserType userType) : base(userType)
        {
        }
        #endregion

        #region abst  
        protected const string ClaimType = "specification";
        protected const string WebClaimValue = "web_platform";
        public abstract List<Claim> SetSpecificationsClaims(User user, List<Claim> claims);
        public abstract List<Claim> TrySetScopes(User user, List<Claim> claims);
        public abstract void AddUserMustConfirmTheirEmailAccountDomainEvent(User user, string code, Dictionary<string, string> additionalParameters = null);
        public abstract void AddUserForgotPasswordDomainEvent(User user, string code);

        #endregion

        protected List<Claim> SetVerifiedClaims(User user, List<Claim> claims)
        {

            claims.Add(new Claim(ClaimType, user.UserType.ToString().ToLower()));

            if (!string.IsNullOrWhiteSpace(user.Email) && user.EmailConfirmed)
            {
                claims.Add(new Claim(ClaimType, JwtClaimTypes.EmailVerified));
            }

            if (!string.IsNullOrWhiteSpace(user.PhoneNumber) && user.PhoneNumberConfirmed)
            {
                claims.Add(new Claim(ClaimType, JwtClaimTypes.PhoneNumberVerified));
            }
            return claims;
        }


        #region inners 
        private sealed class SuperAdminUser : UserTypeManager
        {
            #region Corts
            public SuperAdminUser()
                : base(UserType.SuperAdmin)
            { }
            #endregion 

            #region override abst 
            public override List<Claim> SetSpecificationsClaims(User user, List<Claim> claims)
            {
                return claims;
            }

            public override List<Claim> TrySetScopes(User user, List<Claim> claims)
            {
                return claims;
            }

            public override void AddUserMustConfirmTheirEmailAccountDomainEvent(User user, string code, Dictionary<string, string> additionalParameters = null)
            {

            }

            public override void AddUserForgotPasswordDomainEvent(User user, string code)
            {

            }


            #endregion
        }

        #endregion

    }
}
