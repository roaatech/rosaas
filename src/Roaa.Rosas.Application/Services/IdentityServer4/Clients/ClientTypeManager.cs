using Roaa.Rosas.Application.IdentityServer4;
using Roaa.Rosas.Common.Utilities;
using Roaa.Rosas.Domain.Entities;

public abstract class ClientTypeManager : Enumeration<ClientTypeManager, ClientType>
{
    #region Props 
    public static readonly ClientTypeManager ExternalSystem = new ExternalSystemType();
    public static readonly ClientTypeManager ExternalSystemClient = new ExternalSystemClientType();
    #endregion

    #region Corts
    protected ClientTypeManager(ClientType clientType) : base(clientType)
    {
    }
    #endregion

    #region abst  
    public abstract string GetClaimTypeValue();
    #endregion

    #region inners   
    private sealed class ExternalSystemType : ClientTypeManager
    {
        #region Corts
        public ExternalSystemType() : base(ClientType.ExternalSystem) { }
        #endregion

        #region overrides 
        public override string GetClaimTypeValue()
        {
            return SystemConsts.Clients.Claims.ExternalSystem;
        }

        #endregion
    }

    private sealed class ExternalSystemClientType : ClientTypeManager
    {
        #region Corts
        public ExternalSystemClientType() : base(ClientType.ExternalSystemClient) { }
        #endregion

        #region overrides 
        public override string GetClaimTypeValue()
        {
            return SystemConsts.Clients.Claims.ExternalSystemClient;
        }

        #endregion
    }

    #endregion

}