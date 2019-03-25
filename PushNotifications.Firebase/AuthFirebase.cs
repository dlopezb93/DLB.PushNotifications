using PushNotifications.Firebase.Options;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Linq;

namespace PushNotifications.Firebase
{
    public class AuthFirebase : IAuthenticator
    {
        private readonly FirebaseOptions _firebaseOptions;

        public AuthFirebase(FirebaseOptions firebaseOptions)
        {
            _firebaseOptions = firebaseOptions ?? throw new ArgumentNullException(nameof(firebaseOptions));
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            // only add the Authorization parameter if it hasn't been added by a previous Execute
            if (!request.Parameters.Any(p => p.Type.Equals(ParameterType.HttpHeader) &&
                                         p.Name.Equals("Authorization", StringComparison.OrdinalIgnoreCase)))
            {
                request.AddHeader("Authorization", $"key={_firebaseOptions.SenderId}");
            }            

            if (request.Method != Method.GET && !request.Parameters.Any(p => p.Type.Equals(ParameterType.HttpHeader) &&
                                         p.Name.Equals("Content-Type", StringComparison.OrdinalIgnoreCase)))
            {
                request.AddHeader("Content-Type", "application/json");
            }            
        }
    }
}
