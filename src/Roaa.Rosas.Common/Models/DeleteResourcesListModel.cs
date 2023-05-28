namespace Roaa.Rosas.Common.Models
{
    public record DeleteResourcesListModel<TId>
    {
        public IEnumerable<TId> Ids { get; set; }
    }
}
