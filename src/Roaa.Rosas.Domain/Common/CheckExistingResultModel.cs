namespace Roaa.Rosas.Domain.Common
{
    public record CheckResultModel
    {
        public CheckResultModel(bool result)
        {
            Result = result;
        }

        public bool Result { get; set; }
    }

}
