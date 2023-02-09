using System.Collections.Generic;
using UnityEngine.Networking;

namespace CharacterXYZ.API
{
    public struct RequestHeader
    {
        public string key;
        public string value;

        public RequestHeader(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }

    /// <summary>
    /// Create the request to send
    /// </summary>
    public class RequestBuilder
    {
        private bool hasParams;
        private string queryParams;

        private readonly string url;
        private readonly HTTPVerb httpVerb;
        private readonly List<RequestHeader> headers = new List<RequestHeader>();

        public RequestBuilder(HTTPVerb httpVerb, string urlBase)
        {
            this.httpVerb = httpVerb;
            url = urlBase;
        }

        public RequestBuilder AddRequestHeader(string key, string value)
        {
            headers.Add(new RequestHeader(key, value));
            return this;
        }

        public RequestBuilder WithParams(string key, string value)
        {
            char plus = hasParams ? '&' : '?';
            hasParams = true;

            queryParams += plus + key;

            if (!string.IsNullOrEmpty(value))
            {
                queryParams += "=" + value;
            }

            return this;
        }

        public UnityWebRequest Build()
        {
            UnityWebRequest unityWebRequest = GetRequest();

            foreach (RequestHeader header in headers)
            {
                unityWebRequest.SetRequestHeader(header.key, header.value);
            }

            return unityWebRequest;
        }

        private UnityWebRequest GetRequest()
        {
            switch (httpVerb)
            {
                case HTTPVerb.Get:
                    return UnityWebRequest.Get(url + queryParams);
                case HTTPVerb.Post:
                    // Unity bug: https://stackoverflow.com/questions/68156230/unitywebrequest-post-not-sending-body
                    // return UnityWebRequest.Post(url, body); 

                    UnityWebRequest unityWebRequest = UnityWebRequest.Put(url, queryParams); 
                    unityWebRequest.method = UnityWebRequest.kHttpVerbPOST;
                    return unityWebRequest;
                default:
                    throw new System.NotImplementedException();
            }
        }

        public static implicit operator UnityWebRequest(RequestBuilder requestBuilder) => requestBuilder.Build();
    }
}
