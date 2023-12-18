using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class IdentityServerClientCreatedEvent : BaseInternalEvent
    {
        public ClientCustomDetail CustomDetail { get; set; }
        public IdentityServer4.EntityFramework.Entities.Client Client { get; set; }

        public IdentityServerClientCreatedEvent(IdentityServer4.EntityFramework.Entities.Client client, ClientCustomDetail customDetail)
        {
            CustomDetail = customDetail;
            Client = client;
        }
    }
}
