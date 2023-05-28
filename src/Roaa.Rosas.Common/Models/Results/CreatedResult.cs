namespace Roaa.Rosas.Common.Models.Results
{
    public class CreatedResult<T>  
    {
        public CreatedResult()
        {
        }

        public CreatedResult(T id)
        {
            Id = id;
        }

        public T Id { get; set; } = default;
    }  
}
