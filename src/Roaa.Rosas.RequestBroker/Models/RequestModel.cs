using System;
using System.Collections.Generic;
using System.Linq;

namespace Roaa.Rosas.RequestBroker.Models
{
    public class RequestModel<T>
    {
        public T Data { get; set; }
        public List<QueryString> QueryStrings { get; set; }
        //public List<AdvancedQueryStringModel> AdvancedQueryStrings { get; set; }
        public IDictionary<string, object> Header { get; set; }
        public string[] RouteParams { get; set; }
        public string Uri { get; set; }
        public string Scheme { get; set; }
        public string Token { get; set; }
        public string Lang { get; set; }

        public RequestModel(string uri, T data, string scheme, string token, List<QueryString> queryStrings, string[] routeParams, IDictionary<string, object> header, string lang)
        {
            Uri = uri;
            Data = data;
            QueryStrings = queryStrings;
            RouteParams = routeParams;
            Header = header;
            Scheme = scheme;
            Token = token;
            Lang = lang;
        }

        public RequestModel(string uri, T data, RequestAuthorizationModel requestAuthorization)
        {
            Uri = uri;
            Scheme = requestAuthorization.Scheme;
            Token = requestAuthorization.Token;
            Data = data;
        }
        public RequestModel(string uri, T data)
        {
            Uri = uri;
            Data = data;
        }
        public RequestModel(string uri, T data, params string[] routeParams)
        {
            RouteParams = routeParams;
            Uri = uri;
            Data = data;
        }

        public RequestModel(string uri, params (string Key, object Value)[] queryStrings)
        {
            QueryStrings = queryStrings.Select(x => new QueryString(x.Key, x.Value)).ToList();
            Uri = uri;
        }


        //public RequestModel(string uri, List<AdvancedQueryStringModel> advancedQueryStrings)
        //{
        //    AdvancedQueryStrings = advancedQueryStrings;
        //    Uri = uri;
        //}

        public RequestModel(string uri, params string[] routeParams)
        {
            RouteParams = routeParams;
            Uri = uri;
        }
        public RequestModel(string uri)
        {
            Uri = uri;
        }
    }
    public class RequestAuthorizationModel
    {
        public string Scheme { get; set; }
        public string Token { get; set; }

        public RequestAuthorizationModel(string scheme, string token)
        {
            Scheme = scheme;
            Token = token;
        }
    }


    public class QueryString
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public QueryString(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
    public class AdvancedQueryStringModel
    {
        public string Name { get; set; }
        public IDictionary<string, string> Properties { get; set; }

        public AdvancedQueryStringModel(string name, params (string Key, string Value)[] properties)
        {
            Properties = properties.ToDictionary(x => x.Key, x => x.Value);
            Name = name;
        }
    }
}
