using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.EntityAdminPrivileges.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Services.Management.EntityAdminPrivileges
{
    public class EntityAdminPrivilegeService : IEntityAdminPrivilegeService
    {
        #region Props 
        private readonly ILogger<EntityAdminPrivilegeService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public EntityAdminPrivilegeService(
            ILogger<EntityAdminPrivilegeService> logger,
            IRosasDbContext dbContext,
            IIdentityContextService identityContextService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
        }

        #endregion


        #region Services  
        public async Task<Result<List<EntityAdminPrivilegeDto>>> GetEntityAdminPrivilegesListByEntityIdAsync(Guid EntityId, CancellationToken cancellationToken = default)
        {
            if (!(_identityContextService.IsSuperAdmin() || await _dbContext.EntityAdminPrivileges
                                                                           .AnyAsync(a =>
                                                                               a.UserId == _identityContextService.UserId &&
                                                                               a.EntityId == EntityId)))
            {
                return Result<List<EntityAdminPrivilegeDto>>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            var userTypes = UserTypeManager.FromKey(_identityContextService.GetUserType()).GetUserTypesForWhichHeIsResponsible();

            var entities = await (from p in _dbContext.EntityAdminPrivileges
                                  join u in _dbContext.Users on p.UserId equals u.Id
                                  where p.EntityId == EntityId && (_identityContextService.IsSuperAdmin() || userTypes.Contains(p.UserType))
                                  select new EntityAdminPrivilegeDto
                                  {
                                      Id = p.Id,
                                      Email = u.Email,
                                      IsMajor = p.IsMajor,
                                      UserType = p.UserType,
                                      CreatedDate = p.CreationDate
                                  }).ToListAsync(cancellationToken);

            return Result<List<EntityAdminPrivilegeDto>>.Successful(entities);
        }

        public async Task<Result<CreatedResult<Guid>>> CreateEntityAdminPrivilegeByUserEmailAsync(CreateEntityAdminPrivilegeByUserEmailModel model, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users
                                        .Where(x => x.NormalizedEmail.Equals(model.Email.ToUpper()))
                                        .SingleOrDefaultAsync();
            if (user is null)
            {
                return Result<CreatedResult<Guid>>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale, nameof(model.Email));
            }

            if (await _dbContext.EntityAdminPrivileges
                                    .Where(x => x.UserId == user.Id &&
                                                x.EntityId == model.EntityId)
                                    .AnyAsync(cancellationToken))
            {
                return Result<CreatedResult<Guid>>.Fail(CommonErrorKeys.ResourceAlreadyExists, _identityContextService.Locale, nameof(model.Email));
            }


            return await CreateEntityAdminPrivilegeAsync(new CreateEntityAdminPrivilegeModel
            {
                EntityId = model.EntityId,
                EntityType = model.EntityType,
                UserId = user.Id,
                UserType = user.UserType,
                IsMajor = model.IsMajor,
            });
        }
        public async Task<Result<CreatedResult<Guid>>> CreateEntityAdminPrivilegeAsync(CreateEntityAdminPrivilegeModel model, CancellationToken cancellationToken = default)
        {
            var date = DateTime.UtcNow;

            var entityAdminPrivilege = new EntityAdminPrivilege
            {
                Id = Guid.NewGuid(),
                EntityId = model.EntityId,
                EntityType = model.EntityType,
                UserId = model.UserId,
                UserType = model.UserType,
                IsMajor = model.IsMajor,
                CreatedByUserId = _identityContextService.UserId,
                ModifiedByUserId = _identityContextService.UserId,
                CreationDate = date,
                ModificationDate = date,
            };

            _dbContext.EntityAdminPrivileges.Add(entityAdminPrivilege);

            var res = await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<CreatedResult<Guid>>.Successful(new CreatedResult<Guid>(entityAdminPrivilege.Id));
        }

        public async Task<Result> CreateEntityAdminPrivilegesAsync(List<CreateEntityAdminPrivilegeModel> models, CancellationToken cancellationToken = default)
        {
            var date = DateTime.UtcNow;

            foreach (var model in models)
            {
                var entityAdminPrivilege = new EntityAdminPrivilege
                {
                    Id = Guid.NewGuid(),
                    EntityId = model.EntityId,
                    EntityType = model.EntityType,
                    UserId = model.UserId,
                    UserType = model.UserType,
                    IsMajor = model.IsMajor,
                    CreatedByUserId = _identityContextService.UserId,
                    ModifiedByUserId = _identityContextService.UserId,
                    CreationDate = date,
                    ModificationDate = date,
                };

                _dbContext.EntityAdminPrivileges.Add(entityAdminPrivilege);
            }

            var res = await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }


        public async Task<Result> DeleteEntityAdminPrivilegeAsync(Guid id, CancellationToken cancellationToken = default)
        {

            #region Validation 

            var entityAdminPrivilege = await _dbContext.EntityAdminPrivileges
                                                        .Where(x => x.Id == id)
                                                        .SingleOrDefaultAsync(cancellationToken);
            if (entityAdminPrivilege is null)
            {
                return Result.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            if (!_identityContextService.IsSuperAdmin() ||
                (entityAdminPrivilege.UserId == _identityContextService.UserId ||
                //  entityAdminPrivilege.CreatedByUserId == _identityContextService.UserId ||
                !(adminPrivileges
                    .Where(x => x.UserType == _identityContextService.GetUserType())
                    .FirstOrDefault() ?? new AdminPrivilege()).TypesAllowedDeleted.Contains(entityAdminPrivilege.UserType)))

            {
                return Result.Fail(CommonErrorKeys.UnAuthorizedAction, _identityContextService.Locale);
            }


            if (!(_identityContextService.IsSuperAdmin() || await _dbContext.EntityAdminPrivileges
                                                                         .AnyAsync(a =>
                                                                             a.UserId == _identityContextService.UserId &&
                                                                             a.Id == id &&
                                                                             a.CreatedByUserId == entityAdminPrivilege.UserId &&
                                                                             a.IsMajor)))
            {
                return Result<List<EntityAdminPrivilegeDto>>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            #endregion

            _dbContext.EntityAdminPrivileges.Remove(entityAdminPrivilege);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Successful();
        }


        public Expression<Func<TEntity, bool>> GenerateExpressionFilter<TEntity>(EntityType entityType, Func<TEntity, Guid> tenantIdSelector)
        {
            var isSuperAdmin = _identityContextService.IsSuperAdmin();
            var userId = _identityContextService.UserId;

            Expression<Func<TEntity, bool>> predicate = x => isSuperAdmin || _dbContext.EntityAdminPrivileges.Any(a =>
                                                                                            a.UserId == userId &&
                                                                                            a.EntityId == tenantIdSelector(x) &&
                                                                                            a.EntityType == entityType);
            return predicate;
        }

        #endregion

        private readonly List<AdminPrivilege> adminPrivileges = new List<AdminPrivilege>
        {
            new AdminPrivilege
            {
                UserType = UserType.TenantAdmin,
                TypesAllowedDeleted = new List<UserType>{ UserType.TenantAdmin },
            },
            new AdminPrivilege
            {
                UserType = UserType.ProductAdmin,
                TypesAllowedDeleted = new List<UserType>{ UserType.TenantAdmin, UserType.ProductAdmin },
            },
            new AdminPrivilege
            {
                UserType = UserType.ClientAdmin,
                TypesAllowedDeleted = new List<UserType>{ UserType.TenantAdmin, UserType.ProductAdmin, UserType.ClientAdmin  },
            },
        };

        public record AdminPrivilege
        {
            public UserType UserType { get; set; }

            public List<UserType> TypesAllowedDeleted { get; set; } = new();
        }
    }
}
