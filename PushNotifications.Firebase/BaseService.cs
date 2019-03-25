using PushNotifications.Firebase.Options;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace PushNotifications.Firebase
{
    public class BaseService
    {
        protected RestClient _client;
        private AuthFirebase auth;

        public BaseService(FirebaseOptions firebaseOptions,RestClient restClient, AuthFirebase auth)
        {
            this._client = restClient;
            this._client.BaseUrl = new Uri(firebaseOptions.FirebaseEndPoint);
            this._client.Authenticator = auth;
            this.auth = auth;
        }

        public Task<T> ExecuteAsync<T>(IRestRequest request) where T : new()
        {
            var taskCompletionSource = new TaskCompletionSource<T>();

            _client.ExecuteAsync<T>(request, (response) => taskCompletionSource.SetResult(response.Data));
            return taskCompletionSource.Task;
        }
    }
}
