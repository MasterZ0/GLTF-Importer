using Newtonsoft.Json;
using UnityEngine.Networking;

namespace GLTFImporter.API
{
    public class ApiResult
    {
        public string Text { get; }
        public byte[] Data { get; }
        public bool Successful { get; }
        public int StatusHTTP { get; }
        public string Error { get; }

        protected const int OkCode = 200;

        public ApiResult(UnityWebRequest unityWebRequest)
        {
            Successful = unityWebRequest.result == UnityWebRequest.Result.Success;
            StatusHTTP = (int)unityWebRequest.responseCode;
            Data = unityWebRequest.downloadHandler.data;
            Text = unityWebRequest.downloadHandler.text;

            if (!Successful || StatusHTTP != OkCode)
            {
                Error = unityWebRequest.error;
            }
        }

        protected ApiResult(string text, byte[] data, bool successful, int statusHTTP, string error)
        {
            Text = text;
            Data = data;
            Successful = successful;
            StatusHTTP = statusHTTP;
            Error = error;
        }
    }

    public class ApiResult<T> : ApiResult
    {
        public T Value { get; }

        public ApiResult(UnityWebRequest unityWebRequest) : base(unityWebRequest)
        {
            if (Successful && StatusHTTP == OkCode)
            {
                Value = JsonConvert.DeserializeObject<T>(unityWebRequest.downloadHandler.text);
            }
        }

        private ApiResult(string text, byte[] data, bool successful, int statusHTTP, string error) : base(text, data, successful, statusHTTP, error)
        {
            if (Successful && StatusHTTP == OkCode)
            {
                Value = JsonConvert.DeserializeObject<T>(text);
            }
        }

        public static ApiResult<T> TConvert(ApiResult apiResult)
        {
            return new ApiResult<T>(apiResult.Text, apiResult.Data, apiResult.Successful, apiResult.StatusHTTP, apiResult.Error);
        }
    }
}