using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Entities.Identity;
using Roaa.Rosas.Framework.Controllers.Common;
using System.Net.Mail;

namespace Roaa.Rosas.Framework.Controllers.ExternalSystem
{
    [Route("api/management/apps/v1/tempendpoints/admins")]
    public class ExternalSystemAdminsController : BaseExternalSystemApiController
    {
        #region Props  
        private readonly IRosasDbContext _identityDbContext;

        #endregion

        #region Corts
        public ExternalSystemAdminsController(IRosasDbContext identityDbContext)
        {
            _identityDbContext = identityDbContext;
        }
        #endregion






        [HttpPost()]
        public async Task<IActionResult> CreateSuperAdminAsync([FromBody] CreateSuperAdminModel model, CancellationToken cancellationToken = default)
        {

            if (await _identityDbContext.Users
                                   .AsNoTracking()
                                       .Where(x => model.Email.ToUpper().Equals(x.NormalizedEmail.ToUpper()))
                                       .AnyAsync(cancellationToken))
            {
                Message.AddError("The account is already existed");
                return EmptyResult();
            }

            try
            {

                var mail = new MailAddress(model.Email);
            }
            catch (Exception ex)
            {
                Message.AddError("The email is Invalid");
                return EmptyResult();

            }




            var admin = new User
            {
                Id = new Guid(),
                UserName = model.Email,
                NormalizedUserName = model.Email.ToUpper(),
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpper(),
                CreationDate = DateTime.Now,
                ModificationDate = DateTime.Now,
                EmailConfirmed = true,
                IsActive = true,
                Locale = "en",
                Status = UserStatus.Ready,
                UserType = UserType.SuperAdmin,
            };
            // create a new password hasher
            var hasher = new PasswordHasher<User>();

            // hash the password and set it for the user
            var hashedPassword = hasher.HashPassword(admin, model.Password);
            admin.PasswordHash = hashedPassword;

            _identityDbContext.Users.Add(admin);


            await _identityDbContext.SaveChangesAsync(cancellationToken);

            return EmptyResult();
        }




    }


    public record CreateSuperAdminModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
