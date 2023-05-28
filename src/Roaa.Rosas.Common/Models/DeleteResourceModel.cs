namespace Roaa.Rosas.Common.Models
{
    public record DeleteResourceModel<TId>
    {
        public TId Id { get; set; }
    } 
}
