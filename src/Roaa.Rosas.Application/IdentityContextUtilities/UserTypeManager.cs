using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Utilities;

namespace Roaa.Rosas.Application.IdentityContextUtilities
{
    public abstract class UserTypeManager : Enumeration<UserTypeManager, UserType>
    {
        #region Props
        public static readonly UserTypeManager SuperAdmin = new SuperAdminType();
        public static readonly UserTypeManager ClientAdmin = new ClientAdminType();
        public static readonly UserTypeManager ProductAdmin = new ProductAdminType();
        public static readonly UserTypeManager TenantAdmin = new TenantAdminType();
        public static readonly UserTypeManager ExternalSystem = new ExternalSystemType();
        public static readonly UserTypeManager RosasSystem = new RosasSystemType();
        #endregion

        #region Corts
        protected UserTypeManager(UserType userType) : base(userType)
        {
        }
        #endregion

        #region abst  
        public abstract Guid GetActorId(IIdentityContextService identityContext);
        #endregion

        #region inners  
        private sealed class RosasSystemType : UserTypeManager
        {
            #region Corts
            public RosasSystemType() : base(UserType.RosasSystem) { }
            #endregion

            #region overrides 
            public override Guid GetActorId(IIdentityContextService identityContext)
            {
                return Guid.Empty;
            }
            #endregion
        }

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

        private sealed class ClientAdminType : UserTypeManager
        {
            #region Corts
            public ClientAdminType() : base(UserType.ClientAdmin) { }
            #endregion 

            #region overrides 
            public override Guid GetActorId(IIdentityContextService identityContext)
            {
                return identityContext.UserId;
            }
            #endregion
        }



        private sealed class ProductAdminType : UserTypeManager
        {
            #region Corts
            public ProductAdminType() : base(UserType.ProductAdmin) { }
            #endregion 

            #region overrides 
            public override Guid GetActorId(IIdentityContextService identityContext)
            {
                return identityContext.UserId;
            }
            #endregion
        }


        private sealed class TenantAdminType : UserTypeManager
        {
            #region Corts
            public TenantAdminType() : base(UserType.TenantAdmin) { }
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
