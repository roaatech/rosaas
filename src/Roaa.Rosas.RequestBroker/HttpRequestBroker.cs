using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Roaa.Rosas.RequestBroker.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Roaa.Rosas.RequestBroker
{
    public class HttpRequestBroker : IRequestBroker
    {


        #region Props
        private readonly ILogger<HttpRequestBroker> _logger;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        private const string formUrlEncodedMediaType = "application/x-www-form-urlencoded";
        #endregion

        #region Ctrs
        public HttpRequestBroker(ILogger<HttpRequestBroker> logger, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            };
            _serializerSettings.Converters.Add(new StringEnumConverter());
            _logger = logger;
        }
        #endregion


        #region Requests
        public async Task<RequestResult<TResult>> GetAsync<TResult, TRequest>(RequestModel<TRequest> requestModel, CancellationToken cancellationToken = default)
        {
            HttpClient httpClient = CreateHttpClient(requestModel);

            if (requestModel.Data != null)
                return await SendRequestAsync<TResult, TRequest>(requestModel, httpClient, HttpMethod.Get);
            else
                return await SendRequestAsync<TResult, TRequest>(requestModel, "GET", httpClient.GetAsync);
        }

        public async Task<RequestResult<TResult>> GetAsync<TResult>(string uri, CancellationToken cancellationToken = default)
        {
            RequestModel<dynamic> requestModel = new RequestModel<dynamic>(uri);

            HttpClient httpClient = CreateHttpClient(requestModel);

            return await SendRequestAsync<TResult, dynamic>(requestModel, "GET", httpClient.GetAsync);
        }

        public async Task<RequestResult<TResult>> PostAsync<TResult, TRequest>(RequestModel<TRequest> requestModel, CancellationToken cancellationToken = default)
        {
            HttpClient httpClient = CreateHttpClient(requestModel);

            return await SendRequestAsync<TResult, TRequest>(requestModel, "POST", httpClient.PostAsync);
        }
        public async Task<RequestResult<TResult>> PostAsFormUrlEncodedContentAsync<TResult, TRequest>(RequestModel<TRequest> requestModel, CancellationToken cancellationToken = default)
        {
            HttpClient httpClient = CreateHttpClient(requestModel);

            return await SendRequestAsync<TResult, TRequest>(requestModel, "POST", httpClient.PostAsync, formUrlEncodedMediaType);
        }

        public async Task<RequestResult<TResult>> PutAsync<TResult, TRequest>(RequestModel<TRequest> requestModel, CancellationToken cancellationToken = default)
        {
            HttpClient httpClient = CreateHttpClient(requestModel);

            return await SendRequestAsync<TResult, TRequest>(requestModel, "PUT", httpClient.PutAsync);
        }

        public async Task<RequestResult<TResult>> DeleteAsync<TResult, TRequest>(RequestModel<TRequest> requestModel, CancellationToken cancellationToken = default)
        {
            HttpClient httpClient = CreateHttpClient(requestModel);

            return await SendRequestAsync<TResult, TRequest>(requestModel, "DELETE", httpClient.DeleteAsync);
        }

        public async Task<RequestResult<TResult>> DeleteAsync<TResult>(string uri, CancellationToken cancellationToken = default)
        {
            RequestModel<dynamic> requestModel = new RequestModel<dynamic>(uri);

            HttpClient httpClient = CreateHttpClient(requestModel);

            return await SendRequestAsync<TResult, dynamic>(requestModel, "DELETE", httpClient.DeleteAsync);
        }
        #endregion







        #region Helpers
        private async Task<RequestResult<TResult>> SendRequestAsync<TResult, TRequest>(RequestModel<TRequest> requestModel, HttpClient httpClient, HttpMethod httpMethod)
        {
            string uri = HandleRequestUri(requestModel);

            var JsonData = JsonConvert.SerializeObject(requestModel.Data);

            var content = new StringContent(JsonData, Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json);

            _logger.LogInformation($"{{0}}: Sending HTTP {{1}} request to {{2}}, jsonBody:{{3}}.", "Request Broker", "GET", uri, JsonData);

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = httpMethod,
                RequestUri = new Uri(uri),
                Content = content
            };

            DateTime startTime = DateTime.UtcNow;
            HttpResponseMessage response = await httpClient.SendAsync(request);

            var handledResponse = await HandleResponse<TResult>(response, uri, startTime);

            return handledResponse.Result;
        }

        private async Task<RequestResult<TResult>> SendRequestAsync<TResult, TRequest>(
                                                        RequestModel<TRequest> requestModel,
                                                        string methodName,
                                                        Func<string, HttpContent, Task<HttpResponseMessage>> doRequestAsync,
                                                        string mediaType = System.Net.Mime.MediaTypeNames.Application.Json)
        {
            string uri = HandleRequestUri(requestModel);

            var JsonData = JsonConvert.SerializeObject(requestModel.Data);

            var content = new StringContent(JsonData, Encoding.UTF8, mediaType);

            _logger.LogInformation($"{{0}}: Sending HTTP {{1}} request to {{2}}, jsonBody:{{3}}.", "Request Broker", methodName, uri, JsonData);

            DateTime startTime = DateTime.UtcNow;
            HttpResponseMessage response = await doRequestAsync(uri, content);

            // response.EnsureSuccessStatusCode();

            var handledResponse = await HandleResponse<TResult>(response, uri, startTime);

            return handledResponse.Result;
        }


        private async Task<RequestResult<TResult>> SendRequestAsync<TResult, TRequest>(RequestModel<TRequest> requestModel, string methodName, Func<string, Task<HttpResponseMessage>> doRequestAsync)
        {
            string uri = HandleRequestUri(requestModel);

            _logger.LogInformation($"{{0}}: Sending HTTP {{1}} request to {{2}}.", "Request Broker", methodName, uri);

            DateTime startTime = DateTime.UtcNow;
            HttpResponseMessage response = await doRequestAsync(uri);

            var handledResponse = await HandleResponse<TResult>(response, uri, startTime);

            return handledResponse.Result;
        }

        private HttpClient CreateHttpClient<TRequest>(RequestModel<TRequest> requestModel)
        {
            HttpClient httpClient = CreateHttpClient();

            if (!string.IsNullOrWhiteSpace(requestModel.Scheme) && !string.IsNullOrWhiteSpace(requestModel.Token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(requestModel.Scheme, requestModel.Token);
            }

            if (requestModel.Header != null)
            {
                foreach (var h in requestModel.Header)
                {
                    httpClient.DefaultRequestHeaders.Add(h.Key, h.Value.ToString());
                }
            }

            if (!string.IsNullOrWhiteSpace(requestModel.Lang))
            {
                httpClient.DefaultRequestHeaders.Add("Accept-Language", requestModel.Lang);
            }
            return httpClient;
        }

        private HttpClient CreateHttpClient()
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }

        private async Task<HandledResponse<TResult>> HandleResponse<TResult>(HttpResponseMessage response, string uri, DateTime startTime)
        {
            var content = await response.Content.ReadAsStringAsync();
            HandledResponse<TResult> handledResponse = new HandledResponse<TResult>
            {
                Content = content,
                Result = new RequestResult<TResult>()
                {
                    DurationInMillisecond = (DateTime.UtcNow - startTime).TotalMilliseconds,
                    StatusCode = response.StatusCode,
                    SerializedResponseContent = content,
                },

            };

            _logger.LogInformation("{0}: HTTP {1} request completed in [{2}] ms with status code [{3}={4}], Url {5} and response content {6}",
                                                                                                                        "Request Broker",
                                                                                                                        response.RequestMessage.Method,
                                                                                                                        handledResponse.Result.DurationInMillisecond,
                                                                                                                        (int)response.StatusCode,
                                                                                                                        response.StatusCode.ToString(),
                                                                                                                        uri,
                                                                                                                        handledResponse.Content);


            if (response.IsSuccessStatusCode)
            {
                handledResponse.Result.Success = true;
                handledResponse.Result.Data = JsonConvert.DeserializeObject<TResult>(handledResponse.Content, _serializerSettings);
            }
            else
            {
                handledResponse.Result.Success = false;

                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new Exception(handledResponse.Content);
                }
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    try
                    {
                        handledResponse.Result.Errors = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(handledResponse.Content, _serializerSettings);
                    }
                    catch (Exception exc)
                    {

                    }
                }
            }
            return handledResponse;
        }

        private string HandleRequestUri<TRequest>(RequestModel<TRequest> requestModel)
        {
            string uri = requestModel.Uri;
            if (requestModel.RouteParams != null && requestModel.RouteParams.Length > 0)
            {
                string parameters = "";
                foreach (string param in requestModel.RouteParams)
                {
                    parameters += $"/{param}";
                }
                uri = $"{uri}{parameters}";
            }


            if (requestModel.QueryStrings != null)
            {
                var queries = "";

                foreach (var queryString in requestModel.QueryStrings)
                {
                    if (!string.IsNullOrEmpty(queryString.Name) && queryString.Value != null)
                    {
                        queries += $"{queryString.Name}={queryString.Value}&";
                    }
                }

                if (queries.Length > 0)
                {
                    queries = queries.Remove(queries.Length - 1);
                    uri = $"{uri}?{queries}";
                }
            }

            //if (requestModel.AdvancedQueryStrings != null)
            //{
            //    //  id[0][prop1]=12&id[0][prop2]=2012-02-11&id[0][prop3]=test&
            //    var queries = "";
            //    int counter = 0;
            //    foreach (var model in requestModel.AdvancedQueryStrings)
            //    {
            //        if (!string.IsNullOrEmpty(model.Name) && model.Properties != null)
            //        {
            //            foreach (var prop in model.Properties)
            //            {
            //                if (!string.IsNullOrEmpty(prop.Key) && prop.Value != null)
            //                {
            //                    queries += $"{model.Name}[{counter}][{prop.Key}]={prop.Value}&";
            //                } 
            //            }
            //            counter++;
            //        }
            //    }

            //    if (queries.Length > 0)
            //    {
            //        queries = queries.Remove(queries.Length - 1);
            //        string qm = uri.Contains('?') ? "&" : "?";
            //        uri = $"{uri}{qm}{queries}";
            //    }
            //}

            return uri;
        }
        #endregion
    }

    public class HandledResponse<T>
    {
        public RequestResult<T> Result { get; set; }

        public string Content { get; set; }


    }
}