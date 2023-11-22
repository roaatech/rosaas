using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Products.Queries.GetProductWarnings
{
    public record GetProductWarningsQuery : IRequest<Result<List<ProductWarningsDto>>>
    {
        public GetProductWarningsQuery(Guid productId)
        {
            ProductId = productId;
        }
        public Guid ProductId { get; set; }
    }
}
