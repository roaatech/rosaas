using Roaa.Rosas.RequestBroker.Models;

namespace Roaa.Rosas.RequestBroker
{
    public interface IRequestBroker
    {
        #region Requests
        public Task<RequestResult<TResult>> GetAsync<TResult, TRequest>(RequestModel<TRequest> requestModel, CancellationToken cancellationToken = default);

        public Task<RequestResult<TResult>> GetAsync<TResult>(string uri, CancellationToken cancellationToken = default);

        public Task<RequestResult<TResult>> PostAsync<TResult, TRequest>(RequestModel<TRequest> requestModel, CancellationToken cancellationToken = default);

        public Task<RequestResult<TResult>> PutAsync<TResult, TRequest>(RequestModel<TRequest> requestModel, CancellationToken cancellationToken = default);

        public Task<RequestResult<TResult>> DeleteAsync<TResult, TRequest>(RequestModel<TRequest> requestModel, CancellationToken cancellationToken = default);

        public Task<RequestResult<TResult>> DeleteAsync<TResult>(string uri, CancellationToken cancellationToken = default);
        #endregion



    }

}