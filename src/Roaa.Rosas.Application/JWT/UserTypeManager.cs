using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Utilities;

namespace Roaa.Rosas.Application.JWT
{
    public abstract class UserTypeManager : Enumeration<UserTypeManager, UserType>
    {
        #region Props
        public static readonly UserTypeManager SuperAdmin = new SuperAdminType();
        public static readonly UserTypeManager ExternalSystem = new ExternalSystemType();
        #endregion

        #region Corts
        protected UserTypeManager(UserType sign) : base(sign)
        {
        }
        #endregion

        #region abst  
        public abstract Guid GetActorId(IIdentityContextService identityContext);
        #endregion



        #region inners  

        private sealed class SuperAdminType : UserTypeManager
        {
            #region Corts
            public SuperAdminType() : base(UserType.SuperAdmin) { }
            #endregion 

            #region overrides 
            public override Guid GetActorId(IIdentityContextService identityContext)
            {
                return identityContext.UserId;
            }
            #endregion
        }

        private sealed class ExternalSystemType : UserTypeManager
        {
            #region Corts
            public ExternalSystemType() : base(UserType.ExternalSystem) { }
            #endregion

            #region overrides 
            public override Guid GetActorId(IIdentityContextService identityContext)
            {
                return identityContext.GetProductId();
            }
            #endregion
        }

        #endregion

    }
}
