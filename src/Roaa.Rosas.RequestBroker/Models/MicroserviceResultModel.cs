

namespace Roaa.Rosas.RequestBroker.Models
{
    public class MicroserviceResultModel<T>
    {
        public MicroserviceResultModel(T data)
        {
            Data = data;
        }

        public T Data { get; set; }
    }
}
